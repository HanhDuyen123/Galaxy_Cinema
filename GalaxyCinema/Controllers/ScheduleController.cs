using GalaxyCinema.Attribute;
using GalaxyCinema.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GalaxyCinema.Controllers
{
    [CustomAuthorize("Admin")]
    public class ScheduleController : Controller
    {
        private GalaxyCinemaEntities db = new GalaxyCinemaEntities();

        // GET: Showtime
        public ActionResult Index(long? theaterId, DateTime? selectedDate)
        {
            var query = db.Set<SHOWTIME>()
                .Include(s => s.MOVIE)
                .Include(s => s.THEATER)
                .AsQueryable();

            // Lọc theo rạp nếu có
            if (theaterId.HasValue && theaterId.Value > 0)
            {
                query = query.Where(s => s.THEATERID == theaterId.Value);
            }

            // Lọc theo ngày nếu có
            if (selectedDate.HasValue)
            {
                var startDate = selectedDate.Value.Date;
                var endDate = startDate.AddDays(1);
                query = query.Where(s => s.STARTTIME >= startDate && s.STARTTIME < endDate);
            }
            else
            {
                // Mặc định hiển thị suất chiếu từ hôm nay trở đi
                var today = DateTime.Today;
                query = query.Where(s => s.STARTTIME >= today);
            }

            var showtimes = query.OrderBy(s => s.STARTTIME).ToList();

            // Lấy danh sách rạp để hiển thị dropdown
            ViewBag.Theaters = db.Set<THEATER>()
                .OrderBy(t => t.THEATERNAME)
                .Select(t => new SelectListItem
                {
                    Value = t.THEATERID.ToString(),
                    Text = t.THEATERNAME,
                    Selected = theaterId.HasValue && t.THEATERID == theaterId.Value
                }).ToList();

            ViewBag.SelectedTheaterId = theaterId;
            ViewBag.SelectedDate = selectedDate?.ToString("yyyy-MM-dd") ?? DateTime.Today.ToString("yyyy-MM-dd");

            return View(showtimes);
        }


        // GET: Showtime/Create
        public ActionResult Create()
        {
            // Tạo dropdown list cho Theater
            ViewBag.THEATERID = new SelectList(
                db.THEATERs.OrderBy(t => t.THEATERNAME),
                "THEATERID",
                "THEATERNAME"
            );

            // Tạo dropdown list cho Movie (chỉ lấy phim đang chiếu)
            ViewBag.MOVIEID = new SelectList(
                db.MOVIEs
                    .Where(m => m.MOVIESTATUS == "Released")
                    .OrderBy(m => m.MOVIENAME),
                "MOVIEID",
                "MOVIENAME"
            );

            return View();
        }

        // POST: Showtime/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SHOWTIME showtime)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Lấy thông tin phim để tính thời gian kết thúc
                    var movie = db.MOVIEs.Find(showtime.MOVIEID);
                    if (movie == null)
                    {
                        ModelState.AddModelError("MOVIEID", "Phim không tồn tại.");
                        PopulateDropDownLists();
                        return View(showtime);
                    }

                    // Tự động tính thời gian kết thúc = thời gian bắt đầu + thời lượng phim
                    showtime.ENDTIME = showtime.STARTTIME.AddMinutes(movie.DURATION);

                    // Kiểm tra xung đột lịch chiếu
                    var conflictCheck = CheckScheduleConflictInternal(
                        showtime.THEATERID,
                        showtime.STARTTIME,
                        showtime.MOVIEID
                    );

                    if (conflictCheck.HasConflict)
                    {
                        ModelState.AddModelError("", conflictCheck.Message);
                        PopulateDropDownLists();
                        return View(showtime);
                    }

                    // Kiểm tra thời gian chiếu phải trong tương lai
                    if (showtime.STARTTIME <= DateTime.Now)
                    {
                        ModelState.AddModelError("STARTTIME", "Thời gian chiếu phải trong tương lai.");
                        PopulateDropDownLists();
                        return View(showtime);
                    }

                    // Lưu vào database
                    db.SHOWTIMEs.Add(showtime);
                    db.SaveChanges();

                    TempData["SuccessMessage"] = "Tạo suất chiếu thành công!";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                Debug.WriteLine("Lỗi là: " + ex.Message);

            }

            PopulateDropDownLists();
            return View(showtime);
        }

        // AJAX: Lấy thông tin phim
        [HttpGet]
        public JsonResult GetMovieInfo(long id)
        {
            try
            {
                var movie = db.MOVIEs
                    .Include(m => m.DIRECTOR)
                    .Include(m => m.GENREs)
                    .FirstOrDefault(m => m.MOVIEID == id);

                if (movie == null)
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }

                var movieInfo = new
                {
                    MovieName = movie.MOVIENAME,
                    Duration = movie.DURATION,
                    Director = movie.DIRECTOR?.DIRECTORNAME ?? "N/A",
                    Genres = movie.GENREs.Any()
                        ? string.Join(", ", movie.GENREs.Select(g => g.GENRENAME))
                        : "N/A",
                    Description = movie.DESCRIPTION,
                    ReleaseDate = movie.RELEASEDATE.ToString("dd/MM/yyyy")
                };

                return Json(movieInfo, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // AJAX: Kiểm tra xung đột lịch chiếu
        [HttpGet]
        public JsonResult CheckScheduleConflict(long theaterId, DateTime startTime, long movieId)
        {
            try
            {
                var result = CheckScheduleConflictInternal(theaterId, startTime, movieId);
                return Json(new
                {
                    hasConflict = result.HasConflict,
                    message = result.Message
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    hasConflict = true,
                    message = "Có lỗi khi kiểm tra lịch chiếu: " + ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // Phương thức private để kiểm tra xung đột lịch chiếu
        private ConflictCheckResult CheckScheduleConflictInternal(long theaterId, DateTime startTime, long movieId)
        {
            try
            {
                // Lấy thời lượng phim
                var movie = db.MOVIEs.Find(movieId);
                if (movie == null)
                {
                    return new ConflictCheckResult
                    {
                        HasConflict = true,
                        Message = "Không tìm thấy thông tin phim."
                    };
                }

                var endTime = startTime.AddMinutes(movie.DURATION);

                // Kiểm tra xung đột với các suất chiếu khác trong cùng rạp
                var conflictingShowtimes = db.SHOWTIMEs
                    .Where(s => s.THEATERID == theaterId)
                    .Where(s =>
                        // Suất chiếu mới bắt đầu trong khoảng thời gian của suất chiếu khác
                        (startTime >= s.STARTTIME && startTime < s.ENDTIME) ||
                        // Suất chiếu mới kết thúc trong khoảng thời gian của suất chiếu khác
                        (endTime > s.STARTTIME && endTime <= s.ENDTIME) ||
                        // Suất chiếu mới bao trùm suất chiếu khác
                        (startTime <= s.STARTTIME && endTime >= s.ENDTIME)
                    )
                    .Include(s => s.MOVIE)
                    .ToList();

                if (conflictingShowtimes.Any())
                {
                    var conflictShow = conflictingShowtimes.First();
                    return new ConflictCheckResult
                    {
                        HasConflict = true,
                        Message = $"Xung đột với suất chiếu phim '{conflictShow.MOVIE.MOVIENAME}' " +
                                 $"từ {conflictShow.STARTTIME:HH:mm} đến {conflictShow.ENDTIME:HH:mm} " +
                                 $"ngày {conflictShow.STARTTIME:dd/MM/yyyy}."
                    };
                }

                // Kiểm tra khoảng cách tối thiểu giữa các suất chiếu (15 phút)
                var minGap = 15; // phút
                var nearbyShowtimes = db.SHOWTIMEs
                    .Where(s => s.THEATERID == theaterId)
                    .Where(s =>
                        // Suất chiếu kết thúc trước suất mới nhưng quá gần
                        (s.ENDTIME <= startTime &&
                         DbFunctions.DiffMinutes(s.ENDTIME, startTime) < minGap) ||
                        // Suất chiếu bắt đầu sau suất mới nhưng quá gần
                        (s.STARTTIME >= endTime &&
                         DbFunctions.DiffMinutes(endTime, s.STARTTIME) < minGap)
                    )
                    .Include(s => s.MOVIE)
                    .ToList();

                if (nearbyShowtimes.Any())
                {
                    return new ConflictCheckResult
                    {
                        HasConflict = true,
                        Message = $"Cần có khoảng cách tối thiểu {minGap} phút giữa các suất chiếu " +
                                 "để chuẩn bị và dọn dẹp rạp."
                    };
                }

                return new ConflictCheckResult
                {
                    HasConflict = false,
                    Message = "Không có xung đột lịch chiếu."
                };
            }
            catch (Exception ex)
            {
                return new ConflictCheckResult
                {
                    HasConflict = true,
                    Message = "Có lỗi khi kiểm tra xung đột: " + ex.Message
                };
            }
        }

        // GET: Showtime/Edit/5
        public ActionResult Edit(long id)
        {
            var showtime = db.SHOWTIMEs.Find(id);
            if (showtime == null)
            {
                return HttpNotFound();
            }

            PopulateDropDownLists(showtime);
            return View(showtime);
        }

        // POST: Showtime/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SHOWTIME showtime)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Lấy thông tin phim để tính thời gian kết thúc
                    var movie = db.MOVIEs.Find(showtime.MOVIEID);
                    if (movie == null)
                    {
                        ModelState.AddModelError("MOVIEID", "Phim không tồn tại.");
                        PopulateDropDownLists(showtime);
                        return View(showtime);
                    }

                    // Tự động tính thời gian kết thúc
                    showtime.ENDTIME = showtime.STARTTIME.AddMinutes(movie.DURATION);

                    // Kiểm tra xung đột (loại trừ chính suất chiếu đang sửa)
                    var conflictCheck = CheckScheduleConflictForEdit(
                        showtime.SHOWTIMEID,
                        showtime.THEATERID,
                        showtime.STARTTIME,
                        showtime.MOVIEID
                    );

                    if (conflictCheck.HasConflict)
                    {
                        ModelState.AddModelError("", conflictCheck.Message);
                        PopulateDropDownLists(showtime);
                        return View(showtime);
                    }

                    db.Entry(showtime).State = EntityState.Modified;
                    db.SaveChanges();

                    TempData["SuccessMessage"] = "Cập nhật suất chiếu thành công!";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
            }

            PopulateDropDownLists(showtime);
            return View(showtime);
        }

        // GET: Showtime/Delete/5
        public ActionResult Delete(long id)
        {
            var showtime = db.SHOWTIMEs
                .Include(s => s.MOVIE)
                .Include(s => s.THEATER)
                .FirstOrDefault(s => s.SHOWTIMEID == id);

            if (showtime == null)
            {
                return HttpNotFound();
            }

            return View(showtime);
        }

        // POST: Showtime/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            var result = new { success = false, message = "" };
            var showtime = db.SHOWTIMEs.Find(id);
            if (showtime == null)
            {
                result = new { success = false, message = "Suất chiếu không tồn tại." };
                return Json(result);
            }
            if (db.TICKETs.Any(t => t.SHOWTIMEID == id))
            {
                result = new { success = false, message = "Không thể xóa, đã có vé được bán." };
                return Json(result);
            }
            db.SHOWTIMEs.Remove(showtime);
            db.SaveChanges();
            result = new { success = true, message = "Xóa suất chiếu thành công!" };
            return Json(result);
        }

        // Phương thức kiểm tra xung đột khi edit (loại trừ chính suất chiếu đang sửa)
        private ConflictCheckResult CheckScheduleConflictForEdit(long showtimeId, long theaterId, DateTime startTime, long movieId)
        {
            try
            {
                var movie = db.MOVIEs.Find(movieId);
                if (movie == null)
                {
                    return new ConflictCheckResult
                    {
                        HasConflict = true,
                        Message = "Không tìm thấy thông tin phim."
                    };
                }

                var endTime = startTime.AddMinutes(movie.DURATION);

                var conflictingShowtimes = db.SHOWTIMEs
                    .Where(s => s.THEATERID == theaterId && s.SHOWTIMEID != showtimeId)
                    .Where(s =>
                        (startTime >= s.STARTTIME && startTime < s.ENDTIME) ||
                        (endTime > s.STARTTIME && endTime <= s.ENDTIME) ||
                        (startTime <= s.STARTTIME && endTime >= s.ENDTIME)
                    )
                    .Include(s => s.MOVIE)
                    .ToList();

                if (conflictingShowtimes.Any())
                {
                    var conflictShow = conflictingShowtimes.First();
                    return new ConflictCheckResult
                    {
                        HasConflict = true,
                        Message = $"Xung đột với suất chiếu phim '{conflictShow.MOVIE.MOVIENAME}' " +
                                 $"từ {conflictShow.STARTTIME:HH:mm} đến {conflictShow.ENDTIME:HH:mm}."
                    };
                }

                return new ConflictCheckResult { HasConflict = false };
            }
            catch (Exception ex)
            {
                return new ConflictCheckResult
                {
                    HasConflict = true,
                    Message = "Có lỗi khi kiểm tra xung đột: " + ex.Message
                };
            }
        }

        // Phương thức helper để tạo dropdown lists
        private void PopulateDropDownLists(SHOWTIME showtime = null)
        {
            ViewBag.THEATERID = new SelectList(
                db.THEATERs.OrderBy(t => t.THEATERNAME),
                "THEATERID",
                "THEATERNAME",
                showtime?.THEATERID
            );

            ViewBag.MOVIEID = new SelectList(
                db.MOVIEs
                    .Where(m => m.MOVIESTATUS == "Active" || m.MOVIESTATUS == "Đang chiếu")
                    .OrderBy(m => m.MOVIENAME),
                "MOVIEID",
                "MOVIENAME",
                showtime?.MOVIEID
            );
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    // Class helper để trả về kết quả kiểm tra xung đột
    public class ConflictCheckResult
    {
        public bool HasConflict { get; set; }
        public string Message { get; set; }
    }
}