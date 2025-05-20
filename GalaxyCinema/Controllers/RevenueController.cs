using GalaxyCinema.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;

namespace GalaxyCinema.Controllers
{
    public class RevenueController : Controller
    {
        private GalaxyCinemaEntities db = new GalaxyCinemaEntities();

        // GET: Revenue
        public ActionResult Index(DateTime? startDate, DateTime? endDate, long? movieId)
        {
            // Lấy danh sách phim để hiển thị dropdown
            ViewBag.Movies = new SelectList(db.MOVIEs.ToList(), "MOVIEID", "MOVIENAME");

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

            return View(result);
        }
    }
}
