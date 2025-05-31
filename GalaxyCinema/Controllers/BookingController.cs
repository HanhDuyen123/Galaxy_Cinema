using GalaxyCinema.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using GalaxyCinema.Attribute;

namespace GalaxyCinema.Controllers
{
    [CustomAuthorize("Admin", "Employee")]
    public class BookingController : Controller
    {
        private GalaxyCinemaEntities db = new GalaxyCinemaEntities();

        // GET: SHOWTIMEs
        public ActionResult Index(DateTime? date = null)
        {
            DateTime selectedDate = date ?? DateTime.Today;
            ViewBag.DateRange = GetDateRange();
            ViewBag.SelectedDate = selectedDate;
            ViewBag.Theaters = db.THEATERs.ToList();


            var moviesWithShowtimes = GetMoviesWithShowtimes(selectedDate, DateTime.Now);
            return View(moviesWithShowtimes);
        }

        // AJAX: Lấy danh sách phim và suất chiếu theo ngày

        [HttpGet]
        public ActionResult GetShowtimesByDate(DateTime date, DateTime? currentTime = null)
        {
            // Nếu client không gửi currentTime thì mặc định lấy DateTime.Now
            var now = currentTime ?? DateTime.Now;

            // Truyền cả now vào để lọc trong ngày hôm nay
            var moviesWithShowtimes = GetMoviesWithShowtimes(date, now);

            // Gán lại ViewBag cho partial view nếu cần
            ViewBag.Theaters = db.THEATERs.ToList();

            return PartialView("PartialViews/_MovieShowtimes", moviesWithShowtimes);
        }


        // Phương thức lấy danh sách phim có suất chiếu trong ngày
        private List<MovieShowtimesViewModel> GetMoviesWithShowtimes(DateTime date, DateTime now)
        {
            // Nếu là hôm nay thì bắt đầu từ giờ hiện tại, ngược lại từ đầu ngày
            DateTime startDate = date.Date == now.Date
                ? now
                : date.Date;

            // Kết thúc vào cuối ngày
            DateTime endDate = date.Date.AddDays(1).AddSeconds(-1);

            // Lấy danh sách suất chiếu trong khoảng thời gian
            var showtimes = db.SHOWTIMEs
                .Include(s => s.MOVIE)
                .Include(s => s.MOVIE.AGERATING)
                .Include(s => s.MOVIE.GENREs)
                .Include(s => s.THEATER)
                .Where(s => s.STARTTIME >= startDate && s.STARTTIME <= endDate)
                .ToList();

            // Gom nhóm theo phim, xây ViewModel, đếm số suất, và sắp xếp giảm dần
            var movieGroups = showtimes
                .GroupBy(s => s.MOVIEID)
                .Select(g => new MovieShowtimesViewModel
                {
                    Movie = g.First().MOVIE,
                    Showtimes = g.OrderBy(s => s.STARTTIME).ToList(),
                    Genres = string.Join(", ", g.First().MOVIE.GENREs.Select(x => x.GENRENAME)),
                    ShowtimesCount = g.Count()
                })
                .OrderByDescending(vm => vm.ShowtimesCount)
                .ToList();

            return movieGroups;
        }
        private int XacDinhLoaiNgay(DateTime ngayDat)
        {
            // Chuyển Sunday (0) thành 8 để thống nhất với bảng PARAMETER
            int thu = ngayDat.DayOfWeek == DayOfWeek.Sunday
          ? 8
          : ((int)ngayDat.DayOfWeek + 1); // Monday=1...Saturday=6
            Debug.WriteLine("Đây là thứ: " + thu);
            string ngayThang = ngayDat.ToString("MM-dd");

            // Lấy tất cả tham số đang apply
            var dsParam = db.PARAMETERs
                            .Where(p => p.APPLYCATION)
                            .ToList();

            // 1) Holiday?
            bool isHoliday = dsParam.Any(p =>
                p.PARAMETERDESCRIPTION == "HOLIDAY"
                && p.UNITOFMEASUREMENT == "Ngày"
                && p.VALUE == ngayThang);

            if (isHoliday)
                return 3;

            // 2) Happy day?
            bool isHappy = dsParam.Any(p =>
                p.PARAMETERDESCRIPTION == "HAPPY_DAY"
                && p.UNITOFMEASUREMENT == "Thứ"
                && p.VALUE == thu.ToString());

            if (isHappy)
                return 4;

            // 3) Weekend?
            bool isWeekend = dsParam.Any(p =>
                p.PARAMETERDESCRIPTION == "WEEKEND"
                && p.UNITOFMEASUREMENT == "Thứ"
                && p.VALUE == thu.ToString());

            if (isWeekend)
            {
                Debug.WriteLine("Cuối tuần");
                return 2;
            }

            Debug.WriteLine("Trong tuần");
            // 4) Mặc định weekday
            return 1;
        }

        // Phương thức lấy danh sách ngày từ ngày hiện tại đến 7 ngày sau
        private List<DateTime> GetDateRange()
        {
            List<DateTime> dateRange = new List<DateTime>();
            DateTime currentDate = DateTime.Today;

            for (int i = 0; i < 7; i++)
            {
                dateRange.Add(currentDate.AddDays(i));
            }

            return dateRange;
        }
        // GET: SeatSelection
        // 2) Details: trả về SeatSelectionViewModel, không dùng ViewBag nữa
        public ActionResult Details(long? showtimeId)
        {
            if (!showtimeId.HasValue)
                return RedirectToAction("Index", "Home");

            // a) Load SHOWTIME + liên quan
            var showtime = db.SHOWTIMEs
                .Include("MOVIE.AGERATING")
                .Include("THEATER.SEATs")
                .FirstOrDefault(s => s.SHOWTIMEID == showtimeId.Value);

            if (showtime == null)
                return HttpNotFound();
            // b) Xác định ngày của suất chiếu và thời điểm hiện tại
            DateTime selectedDate = showtime.STARTTIME.Date; // Chỉ lấy phần ngày
            DateTime now = DateTime.Now;
            DateTime currentDate = now.Date; // Chỉ lấy phần ngày của hiện tại

            // c) Lấy các suất chiếu khác cùng phim (tất cả rạp/phòng) trong cùng ngày
            var otherShowtimes = GetOtherShowtimes(showtime.MOVIEID, selectedDate, now, showtimeId.Value);

            // Debug để kiểm tra
            Debug.WriteLine($"Selected Date: {selectedDate}");
            Debug.WriteLine($"Current Date: {currentDate}");
            Debug.WriteLine($"Is same day: {selectedDate == currentDate}");
            foreach (var st in otherShowtimes)
                Debug.WriteLine($"ShowtimeID={st.SHOWTIMEID}, Time={st.STARTTIME}");
            // d) Tất cả ghế của rạp
            var allSeats = showtime.THEATER.SEATs.ToList();

            // e) Ghế đã bán
            var soldIds = db.TICKETSEATs
                .Where(ts => ts.TICKETDETAIL.TICKET.SHOWTIMEID == showtimeId.Value)
                .Select(ts => ts.SEATID)
                .Distinct()
                .ToList();

            // f) Build availability dict
            var seatAvailability = allSeats
                .ToDictionary(se => se.SEATID, se => !soldIds.Contains(se.SEATID));

            // g) Nhóm theo hàng
            var seatsByRow = allSeats
                .GroupBy(se => se.SEATNAME.Substring(0, 1))
                .OrderBy(g => g.Key)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderBy(se => ExtractSeatNumber(se.SEATNAME)).ToList()
                );

            // chuẩn bị danh sách loại vé và day category
            var ticketTypes = db.TICKETTYPEs.OrderBy(tt => tt.TICKETTYPEID).ToList();
            int dayCatId = XacDinhLoaiNgay(showtime.STARTTIME);

            // h) Build ViewModel
            var vm = new SeatSelectionViewModel
            {
                Showtime = showtime,
                OtherShowtimes = otherShowtimes,
                SeatsByRow = seatsByRow,
                SeatAvailability = seatAvailability,
                SelectedSeatIds = new List<long>(),
                TicketTypes = ticketTypes,
                CurrentDayCatId = dayCatId,
                CurrentDayCatName = db.DAYCATs.Find(dayCatId).DAYCATNAME
            };

            return View("Details", vm);
        }

        private List<SHOWTIME> GetOtherShowtimes(long movieId, DateTime selectedDate, DateTime now, long currentShowtimeId)
        {
            DateTime currentDate = now.Date;

            // Luôn lấy tất cả suất chiếu cùng phim trong ngày (tất cả rạp/phòng)
            DateTime startDate = selectedDate; // Từ đầu ngày
            DateTime endDate = selectedDate.AddDays(1).AddTicks(-1); // Đến cuối ngày

            // Debug để kiểm tra khoảng thời gian
            Debug.WriteLine($"Start Date: {startDate}");
            Debug.WriteLine($"End Date: {endDate}");
            Debug.WriteLine($"Query params - MovieID: {movieId} (all theaters)");

            // Query tất cả suất chiếu cùng phim trong ngày (tất cả rạp)
            var allShowtimesInDay = db.SHOWTIMEs
                .Include(s => s.THEATER)
                .Include(s => s.MOVIE)
                .Where(s =>
                    s.MOVIEID == movieId &&
                    s.STARTTIME >= startDate &&
                    s.STARTTIME <= endDate
                )
                .OrderBy(s => s.STARTTIME)
                .ThenBy(s => s.THEATER.THEATERNAME) // Sắp xếp theo tên rạp nếu cùng giờ
                .ToList();

            Debug.WriteLine($"All showtimes for this movie today: {allShowtimesInDay.Count}");

            List<SHOWTIME> result;

            if (selectedDate == currentDate) // Nếu là hôm nay
            {
                // Lọc: bao gồm suất hiện tại + các suất sau thời điểm hiện tại
                result = allShowtimesInDay
                    .Where(s => s.SHOWTIMEID == currentShowtimeId || s.STARTTIME > now)
                    .ToList();

                Debug.WriteLine($"Today - filtered to {result.Count} showtimes (current + future)");
            }
            else // Nếu là ngày khác
            {
                // Lấy tất cả suất trong ngày
                result = allShowtimesInDay;
                Debug.WriteLine($"Other day - all {result.Count} showtimes");
            }

            // Debug chi tiết từng suất
            foreach (var st in result)
            {
                Debug.WriteLine($"  ShowtimeID={st.SHOWTIMEID}, Time={st.STARTTIME}, Theater={st.THEATER.THEATERNAME}");
            }

            return result;
        }


        // Lấy giá vé dựa vào loại ngày và loại tuổi
        public JsonResult GetTicketPrice(int dayCatId, int ticketTypeId)
        {
            Debug.WriteLine($"Mã ngày và mã loại người đặt: {dayCatId}, {ticketTypeId}\n");
            var price = db.TICKETPRICEs
                      .Where(tp => tp.TICKETTYPEID == ticketTypeId
                                && tp.DAYCATID == dayCatId)
                      .Select(tp => tp.PRICETTICKET)
                      .FirstOrDefault();
            return Json(new { success = true, price });
        }

        // Helper method to extract the number part from seat name (e.g., "A1" returns 1)
        private int ExtractSeatNumber(string name)
        {
            var digits = new string(name
                .SkipWhile(c => !char.IsDigit(c))
                .TakeWhile(char.IsDigit)
                .ToArray()
            );
            return int.TryParse(digits, out var n) ? n : 0;
        }




        // AJAX: SeatSelection/CheckSeatAvailability

        // GET: SeatSelection/ChangeShowtime
        public ActionResult ChangeShowtime(long? newShowtimeId)
        {
            if (!newShowtimeId.HasValue)
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Details", new { showtimeId = newShowtimeId.Value });
        }

        [HttpPost]
        public JsonResult ProcessBooking(BookingRequest request)
        {
            try
            {
                // 1. Validation cơ bản  
                if (request.ShowtimeId <= 0 || request.SelectedSeats == null || !request.SelectedSeats.Any())
                {
                    return Json(new { success = false, message = "Thông tin đặt vé không hợp lệ" });
                }

                // 2. Kiểm tra showtime còn hợp lệ
                var showtime = db.SHOWTIMEs
                    .Include(s => s.MOVIE)
                    .Include(s => s.THEATER)
                    .FirstOrDefault(s => s.SHOWTIMEID == request.ShowtimeId);

                if (showtime == null || showtime.STARTTIME <= DateTime.Now)
                {
                    return Json(new { success = false, message = "Suất chiếu không hợp lệ hoặc đã qua giờ" });
                }

                // 3. Kiểm tra ghế có còn trống không
                var seatIds = request.SelectedSeats.Select(s => s.SeatId).ToList();
                var soldSeats = db.TICKETSEATs
                    .Where(ts => ts.TICKETDETAIL.TICKET.SHOWTIMEID == request.ShowtimeId &&
                               seatIds.Contains(ts.SEATID))
                    .Select(ts => ts.SEATID)
                    .ToList();



                // 4. Tính toán giá vé và nhóm theo TICKETPRICEID
                decimal totalAmount = 0;
                var ticketPriceGroups = new Dictionary<int, List<long>>(); // ticketPriceId -> list of seatIds

                foreach (var seatRequest in request.SelectedSeats)
                {
                    // Tìm TICKETPRICE dựa trên TICKETTYPEID và DAYCATID
                    var ticketPrice = db.TICKETPRICEs
                        .FirstOrDefault(tp => tp.TICKETTYPEID == seatRequest.TicketTypeId &&
                                            tp.DAYCATID == request.DayCatId);

                    if (ticketPrice == null)
                    {
                        return Json(new { success = false, message = "Không tìm thấy giá vé phù hợp cho loại vé này" });
                    }

                    totalAmount += ticketPrice.PRICETTICKET;

                    // Nhóm ghế theo TICKETPRICEID
                    if (!ticketPriceGroups.ContainsKey(ticketPrice.TICKETPRICEID))
                    {
                        ticketPriceGroups[ticketPrice.TICKETPRICEID] = new List<long>();
                    }
                    ticketPriceGroups[ticketPrice.TICKETPRICEID].Add(seatRequest.SeatId);
                }

                // 5. Tạo ticket transaction
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        // Tạo TICKET
                        var ticket = new TICKET
                        {
                            SHOWTIMEID = request.ShowtimeId,
                            CUSTOMERID = request.CustomerId, // Có thể null nếu guest
                            EMPLOYEEID = request.StaffId != 0 ? request.StaffId : 1L,
                            SELLDATE = DateTime.Now,
                            TOTALAMOUT = totalAmount,
                            TOTALTICKETS = request.SelectedSeats.Count,
                            TICKETSTATUS = true, // true = confirmed
                            PAYMENTID = 1 // Tạm thời set default, có thể cần logic khác
                        };
                        db.TICKETs.Add(ticket);
                        db.SaveChanges(); // Để lấy TICKETID

                        // Tạo TICKETDETAIL cho từng nhóm giá
                        foreach (var group in ticketPriceGroups)
                        {
                            int ticketPriceId = group.Key;
                            var seatIdsInGroup = group.Value;

                            // Lấy thông tin giá
                            var ticketPrice = db.TICKETPRICEs.Find(ticketPriceId);
                            decimal groupTotalPrice = ticketPrice.PRICETTICKET * seatIdsInGroup.Count;

                            var ticketDetail = new TICKETDETAIL
                            {
                                TICKETID = ticket.TICKETID,
                                TICKETPRICEID = ticketPriceId,
                                QUANTITY = seatIdsInGroup.Count,
                                TOTALPRICE = groupTotalPrice
                            };
                            db.TICKETDETAILs.Add(ticketDetail);
                            db.SaveChanges(); // Để lấy TICKETDETAILID

                            // Tạo TICKETSEAT cho từng ghế trong nhóm
                            foreach (var seatId in seatIdsInGroup)
                            {
                                var ticketSeat = new TICKETSEAT
                                {
                                    TICKETDETAILID = ticketDetail.TICKETDETAILID,
                                    SEATID = seatId
                                };
                                db.TICKETSEATs.Add(ticketSeat);
                            }
                        }



                        // Tạo thông tin chi tiết cho response
                        var ticketInfo = new
                        {
                            success = true,
                            message = "Đặt vé thành công!",
                            ticketId = ticket.TICKETID,
                            totalAmount = totalAmount,
                            // Thông tin chi tiết để hiển thị
                            ticketDetails = new
                            {
                                TicketId = ticket.TICKETID,
                                MovieName = showtime.MOVIE.MOVIENAME,
                                TheaterName = showtime.THEATER.THEATERNAME,
                                ShowDate = showtime.STARTTIME.ToString("dd/MM/yyyy"),
                                ShowTime = showtime.STARTTIME.ToString("HH:mm"),
                                DayType = GetDayTypeName(request.DayCatId),
                                Seats = GetSeatDetails(request.SelectedSeats, request.DayCatId),
                                TotalTickets = request.SelectedSeats.Count,
                                TotalAmount = totalAmount,
                                SellDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                                StaffName = GetStaffName(request.StaffId),
                                PaymentMethod = "Tiền mặt" // Default, có thể customize
                            }
                        };
                        db.SaveChanges();
                        transaction.Commit();

                        return Json(ticketInfo);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return Json(new { success = false, message = "Lỗi khi lưu dữ liệu: " + ex.Message });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        // Helper methods
        private string GetDayTypeName(int dayCatId)
        {
            var daycat = db.DAYCATs.Find(dayCatId);
            return daycat?.DAYCATNAME ?? "Thường/Cuối tuần/Lễ/Happy Day";
        }

        private string GetStaffName(long staffId)
        {
            var staff = db.EMPLOYEEs.Find(staffId);
            return staff?.EFIRSTNAME + " " + staff.ELASTNAME ?? "Nhân viên";
        }

        private List<object> GetSeatDetails(List<SeatBookingRequest> selectedSeats, int dayCatId)
        {
            var seatDetails = new List<object>();

            foreach (var seatRequest in selectedSeats)
            {
                var seat = db.SEATs.Find(seatRequest.SeatId);
                var ticketType = db.TICKETTYPEs.Find(seatRequest.TicketTypeId);
                var ticketPrice = db.TICKETPRICEs
                    .FirstOrDefault(tp => tp.TICKETTYPEID == seatRequest.TicketTypeId &&
                                         tp.DAYCATID == dayCatId);

                if (seat != null && ticketType != null && ticketPrice != null)
                {
                    seatDetails.Add(new
                    {
                        SeatName = seat.SEATNAME,
                        TicketType = ticketType.TICKETTYPENAME,
                        Price = ticketPrice.PRICETTICKET
                    });
                }
            }

            return seatDetails;
        }
        // Validate real-time seat availability
        [HttpPost]
        public JsonResult ValidateSeats(long showtimeId, List<long> seatIds)
        {
            try
            {
                var soldSeats = db.TICKETSEATs
                    .Where(ts => ts.TICKETDETAIL.TICKET.SHOWTIMEID == showtimeId &&
                               seatIds.Contains(ts.SEATID))
                    .Select(ts => ts.SEATID)
                    .ToList();

                return Json(new
                {
                    success = true,
                    unavailableSeats = soldSeats
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}