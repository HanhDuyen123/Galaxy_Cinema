using GalaxyCinema.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Collections.Generic;
using GalaxyCinema.Attribute;

namespace GalaxyCinema.Controllers
{
    [CustomAuthorize("Admin")]
    public class RevenueController : Controller
    {
        private GalaxyCinemaEntities db = new GalaxyCinemaEntities();

        // GET: Revenue
        public ActionResult Index(DateTime? startDate, DateTime? endDate, long? movieId)
        {
            // Lấy danh sách phim để hiển thị dropdown
            ViewBag.Movies = new SelectList(db.MOVIEs.ToList(), "MOVIEID", "MOVIENAME");

            if (!startDate.HasValue && !endDate.HasValue)
            {
                startDate = DateTime.Now.Date;
                endDate = DateTime.Now.Date;

                // Cập nhật ViewBag để hiển thị trên form
                ViewBag.StartDate = startDate.Value.ToString("yyyy-MM-dd");
                ViewBag.EndDate = endDate.Value.ToString("yyyy-MM-dd");
            }

            // Validation ngày tháng
            if (startDate.HasValue && endDate.HasValue && startDate.Value > endDate.Value)
            {
                ViewBag.ErrorMessage = "Ngày bắt đầu không được lớn hơn ngày kết thúc!";
                return View(new List<Revenue>());
            }

            // Validation ngày không được trong tương lai
            if (startDate.HasValue && startDate.Value.Date > DateTime.Now.Date)
            {
                ViewBag.ErrorMessage = "Ngày bắt đầu không được là ngày trong tương lai!";
                return View(new List<Revenue>());
            }

            if (endDate.HasValue && endDate.Value.Date > DateTime.Now.Date)
            {
                ViewBag.ErrorMessage = "Ngày kết thúc không được là ngày trong tương lai!";
                return View(new List<Revenue>());
            }

            // Validation khoảng cách thời gian (không quá 1 năm)
            if (startDate.HasValue && endDate.HasValue)
            {
                var daysDifference = (endDate.Value - startDate.Value).Days;
                if (daysDifference > 365)
                {
                    ViewBag.ErrorMessage = "Khoảng thời gian thống kê không được vượt quá 1 năm!";
                    return View(new List<Revenue>());
                }
            }

            // Truy vấn dữ liệu thống kê doanh thu
            var query = from ticket in db.TICKETs
                        join showtime in db.SHOWTIMEs on ticket.SHOWTIMEID equals showtime.SHOWTIMEID
                        join movie in db.MOVIEs on showtime.MOVIEID equals movie.MOVIEID
                        where ticket.TICKETSTATUS == true // Lọc vé đã bán
                        select new
                        {
                            ticket.SELLDATE,
                            movie.MOVIENAME,
                            movie.MOVIEID,
                            ticket.TOTALTICKETS,
                            ticket.TOTALAMOUT
                        };

            // Lọc theo ngày bắt đầu và ngày kết thúc nếu có
            if (startDate.HasValue)
                query = query.Where(x => DbFunctions.TruncateTime(x.SELLDATE) >= DbFunctions.TruncateTime(startDate.Value));

            if (endDate.HasValue)
                query = query.Where(x => DbFunctions.TruncateTime(x.SELLDATE) <= DbFunctions.TruncateTime(endDate.Value));

            // Lọc theo phim nếu có
            if (movieId.HasValue)
                query = query.Where(x => x.MOVIEID == movieId.Value);

            // Nhóm theo ngày và phim, sau đó tính tổng số vé và doanh thu
            var result = query
                .GroupBy(x => new { Date = DbFunctions.TruncateTime(x.SELLDATE), x.MOVIENAME })
                .Select(g => new Revenue
                {
                    Date = g.Key.Date,  // Trả về DateTime không bao gồm thời gian
                    MovieName = g.Key.MOVIENAME,
                    Tickets = g.Sum(x => x.TOTALTICKETS),
                    Amount = g.Sum(x => x.TOTALAMOUT)
                })
                .OrderBy(x => x.Date)
                .ToList();

            // Tính tổng số vé và doanh thu
            ViewBag.TotalTickets = result.Sum(x => x.Tickets);
            ViewBag.TotalRevenue = result.Sum(x => x.Amount);

            // Thông báo thành công nếu có dữ liệu
            if (result.Count > 0)
            {
                ViewBag.SuccessMessage = $"Tìm thấy {result.Count} bản ghi thống kê doanh thu.";
            }
            else if (startDate.HasValue || endDate.HasValue || movieId.HasValue)
            {
                ViewBag.WarningMessage = "Không tìm thấy dữ liệu thống kê theo tiêu chí đã chọn.";
            }

            return View(result);
        }

        // Action để xuất báo cáo PDF
        public ActionResult ExportToPdf(DateTime? startDate, DateTime? endDate, long? movieId)
        {
            // Validation trước khi xuất PDF
            if (startDate.HasValue && endDate.HasValue && startDate.Value > endDate.Value)
            {
                TempData["ErrorMessage"] = "Không thể xuất báo cáo: Ngày bắt đầu không được lớn hơn ngày kết thúc!";
                return RedirectToAction("Index", new { startDate, endDate, movieId });
            }

            if (startDate.HasValue && startDate.Value.Date > DateTime.Now.Date)
            {
                TempData["ErrorMessage"] = "Không thể xuất báo cáo: Ngày bắt đầu không được là ngày trong tương lai!";
                return RedirectToAction("Index", new { startDate, endDate, movieId });
            }

            if (endDate.HasValue && endDate.Value.Date > DateTime.Now.Date)
            {
                TempData["ErrorMessage"] = "Không thể xuất báo cáo: Ngày kết thúc không được là ngày trong tương lai!";
                return RedirectToAction("Index", new { startDate, endDate, movieId });
            }
            // Truy vấn lại dữ liệu như ở trong action Index
            var query = from ticket in db.TICKETs
                        join showtime in db.SHOWTIMEs on ticket.SHOWTIMEID equals showtime.SHOWTIMEID
                        join movie in db.MOVIEs on showtime.MOVIEID equals movie.MOVIEID
                        where ticket.TICKETSTATUS == true
                        select new
                        {
                            ticket.SELLDATE,
                            movie.MOVIENAME,
                            movie.MOVIEID,
                            ticket.TOTALTICKETS,
                            ticket.TOTALAMOUT
                        };

            if (startDate.HasValue)
                query = query.Where(x => DbFunctions.TruncateTime(x.SELLDATE) >= DbFunctions.TruncateTime(startDate.Value));

            if (endDate.HasValue)
                query = query.Where(x => DbFunctions.TruncateTime(x.SELLDATE) <= DbFunctions.TruncateTime(endDate.Value));

            if (movieId.HasValue)
                query = query.Where(x => x.MOVIEID == movieId.Value);

            var result = query
                .GroupBy(x => new { Date = DbFunctions.TruncateTime(x.SELLDATE), x.MOVIENAME })
                .Select(g => new Revenue
                {
                    Date = g.Key.Date,
                    MovieName = g.Key.MOVIENAME,
                    Tickets = g.Sum(x => x.TOTALTICKETS),
                    Amount = g.Sum(x => x.TOTALAMOUT)
                })
                .OrderBy(x => x.Date)
                .ToList();

            // Tính tổng số vé và doanh thu
            ViewBag.TotalTickets = result.Sum(x => x.Tickets);
            ViewBag.TotalRevenue = result.Sum(x => x.Amount);

            // Kiểm tra có dữ liệu để xuất không
            if (result.Count == 0)
            {
                TempData["WarningMessage"] = "Không có dữ liệu để xuất báo cáo theo tiêu chí đã chọn!";
                return RedirectToAction("Index", new { startDate, endDate, movieId });
            }

            // Truyền thông tin ngày tháng
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.ReportDate = DateTime.Now;

            // Tạo PDF từ View riêng cho PDF
            var pdfResult = new Rotativa.ViewAsPdf("RevenuePdf", result)
            {
                PageSize = Rotativa.Options.Size.A4,
                PageOrientation = Rotativa.Options.Orientation.Portrait,
                PageMargins = new Rotativa.Options.Margins(10, 10, 10, 10)
            };

            return pdfResult;
        }
    }
}