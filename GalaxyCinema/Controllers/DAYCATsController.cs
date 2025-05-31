using GalaxyCinema.Attribute;
using GalaxyCinema.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GalaxyCinema.Controllers
{
    [CustomAuthorize("Admin")]
    public class DAYCATsController : Controller
    {
        // GET: DAYCATs
        private GalaxyCinemaEntities db = new GalaxyCinemaEntities();
        // GET: Partial view modal chỉnh sửa loại ngày
        [HttpGet]
        public ActionResult EditDayCategory()
        {
            // Tạo ViewModel khởi tạo cho modal từ PARAMETERs
            var vm = new DayTypeParameterModel
            {
                NgayThuong = db.PARAMETERs
                                 .Where(p => p.PARAMETERDESCRIPTION == "REGULAR_DAYS" && p.APPLYCATION)
                                 .OrderBy(p => p.VALUE)
                                 .Select(p => p.VALUE)
                                 .Distinct()
                                 .ToList(),
                HappyDay = db.PARAMETERs
                                 .Where(p => p.PARAMETERDESCRIPTION == "HAPPY_DAY" && p.APPLYCATION)
                                 .OrderBy(p => p.VALUE)
                                 .Select(p => p.VALUE)
                                 .Distinct()
                                 .ToList(),
                NgayCuoiTuan = db.PARAMETERs
                                 .Where(p => p.PARAMETERDESCRIPTION == "WEEKEND" && p.APPLYCATION)
                                 .OrderBy(p => p.VALUE)
                                 .Select(p => p.VALUE)
                                 .Distinct()
                                 .ToList(),
                Holidays = db.PARAMETERs
                                 .Where(p => p.PARAMETERDESCRIPTION == "HOLIDAY" && p.APPLYCATION)
                                 .OrderBy(p => p.VALUE)
                                 .Select(p => p.VALUE)
                                 .Distinct()
                                 .ToList()
            };
            return PartialView(
                "~/Views/Booking/PartialViews/_EditDayCategoryModal.cshtml", vm);
        }

        // GET: /DayType/GetDayTypeSettings
        [HttpGet]
        public JsonResult GetDayTypeSettings()
        {
            // Trả về JSON dữ liệu cho JS load dynamic nếu cần
            var data = new
            {
                NgayThuong = db.PARAMETERs
                                  .Where(p => p.PARAMETERDESCRIPTION == "REGULAR_DAYS" && p.APPLYCATION)
                                  .Select(p => p.VALUE)
                                  .Distinct()
                                  .ToList(),
                HappyDay = db.PARAMETERs
                                  .Where(p => p.PARAMETERDESCRIPTION == "HAPPY_DAY" && p.APPLYCATION)
                                  .Select(p => p.VALUE)
                                  .Distinct()
                                  .ToList(),
                NgayCuoiTuan = db.PARAMETERs
                                  .Where(p => p.PARAMETERDESCRIPTION == "WEEKEND" && p.APPLYCATION)
                                  .Select(p => p.VALUE)
                                  .Distinct()
                                  .ToList(),
                NgayLe = db.PARAMETERs
                                  .Where(p => p.PARAMETERDESCRIPTION == "HOLIDAY" && p.APPLYCATION)
                                  .Select(p => p.VALUE)
                                  .Distinct()
                                  .ToList()
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }



        // POST: /DayType/AddHoliday
        [HttpPost]
        public JsonResult AddHoliday(string holidayValue)
        {
            try
            {
                if (string.IsNullOrEmpty(holidayValue))
                    return Json(new { success = false, message = "Ngày lễ không hợp lệ" });

                bool exists = db.PARAMETERs.Any(p => p.PARAMETERDESCRIPTION == "HOLIDAY"
                                                    && p.VALUE == holidayValue
                                                    && p.APPLYCATION);
                if (exists)
                    return Json(new { success = false, message = "Ngày lễ đã tồn tại" });

                db.PARAMETERs.Add(new PARAMETER
                {
                    PARAMETERDESCRIPTION = "HOLIDAY",
                    VALUE = holidayValue,
                    UNITOFMEASUREMENT = "Ngày",
                    APPLYCATION = true
                });
                db.SaveChanges();
                return Json(new { success = true, message = "Thêm ngày lễ thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: /DayType/DeleteHoliday
        [HttpPost]
        public JsonResult DeleteHoliday(string holidayValue)
        {
            try
            {
                var entries = db.PARAMETERs
                                .Where(p => p.PARAMETERDESCRIPTION == "HOLIDAY"
                                         && p.VALUE == holidayValue
                                         && p.APPLYCATION);
                foreach (var p in entries)
                {
                    p.APPLYCATION = false;
                    db.Entry(p).State = EntityState.Modified;
                }
                db.SaveChanges();
                return Json(new { success = true, message = "Xóa ngày lễ thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: /DayType/UpdateDayTypeSettings
        [HttpPost]
        public JsonResult UpdateDayTypeSettings(DayTypeParameterModel model)
        {
            try
            {
                // Xử lý xung đột
                var allSelected = new List<string>();
                allSelected.AddRange(model.NgayThuong);
                allSelected.AddRange(model.HappyDay);
                allSelected.AddRange(model.NgayCuoiTuan);
                var duplicate = allSelected.GroupBy(x => x)
                                           .Where(g => g.Count() > 1)
                                           .Select(g => g.Key)
                                           .ToList();
                if (duplicate.Any())
                    return Json(new { success = false, message = "Xung đột giá trị: " + string.Join(",", duplicate) });

                // Xóa cũ
                var keys = new[] { "REGULAR_DAYS", "HAPPY_DAY", "WEEKEND", "HOLIDAY" };
                var old = db.PARAMETERs.Where(p => keys.Contains(p.PARAMETERDESCRIPTION));
                foreach (var p in old)
                    p.APPLYCATION = false;
                db.SaveChanges();

                // Thêm mới unique
                void Add(string desc, IEnumerable<string> vals, string unit)
                {
                    foreach (var v in vals.Distinct())
                    {
                        db.PARAMETERs.Add(new PARAMETER
                        {
                            PARAMETERDESCRIPTION = desc,
                            VALUE = v,
                            UNITOFMEASUREMENT = unit,
                            APPLYCATION = true
                        });
                    }
                }

                Add("REGULAR_DAYS", model.NgayThuong, "Thứ");
                Add("HAPPY_DAY", model.HappyDay, "Thứ");
                Add("WEEKEND", model.NgayCuoiTuan, "Thứ");
                Add("HOLIDAY", model.Holidays, "Ngày");

                db.SaveChanges();
                return Json(new { success = true, message = "Cập nhật thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}