using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using GalaxyCinema.Models;
using Antlr.Runtime.Misc;
using GalaxyCinema.Attribute;

namespace GalaxyCinema.Controllers
{
    [CustomAuthorize("Admin", "Employee")]
    public class MoviesController : Controller
    {
        private GalaxyCinemaEntities db = new GalaxyCinemaEntities();

        // GET: Movies
        //public ActionResult Index()
        //{
        //    var mOVIEs = db.MOVIEs.Include(m => m.AGERATING).Include(m => m.DIRECTOR).Include(m => m.NATION);
        //    return View(mOVIEs.ToList());
        //}

        // GET: Movies
        public ActionResult Index(string movieName, int? genreId, string directorName, string status)
        {
            var query = db.MOVIEs.Include(m => m.AGERATING).Include(m => m.DIRECTOR).Include(m => m.NATION).Include(m => m.GENREs);

            if (!string.IsNullOrWhiteSpace(movieName))
            {
                query = query.Where(m => m.MOVIENAME.Contains(movieName));
            }

            if (genreId.HasValue)
            {
                query = query.Where(m => m.GENREs.Any(g => g.GENREID == genreId.Value));
            }

            if (!string.IsNullOrWhiteSpace(directorName))
            {
                query = query.Where(m => m.DIRECTOR.DIRECTORNAME.Contains(directorName));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                switch (status)
                {
                    case "Sắp chiếu":
                        query = query.Where(m => m.MOVIESTATUS == "Coming Soon");
                        break;
                    case "Đang chiếu":
                        query = query.Where(m => m.MOVIESTATUS == "Released");
                        break;
                    case "Đã chiếu":
                        query = query.Where(m => m.MOVIESTATUS == "Archived");
                        break;
                }
            }
            ViewBag.Genres = new SelectList(db.GENREs, "GENREID", "GENRENAME");
            ViewBag.MovieName = movieName;
            ViewBag.DirectorName = directorName;
            ViewBag.Status = status;
            ViewBag.GenreId = genreId;

            return View(query.OrderByDescending(m => m.MOVIEID).ToList());
        }

        public static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var normalized = text.Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder();

            foreach (var c in normalized)
            {
                var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
                    builder.Append(c);
            }

            return builder.ToString().Normalize(NormalizationForm.FormC);
        }
        //tên phim
        [HttpGet]
        public JsonResult GetMovieSuggestions(string term)
        {
            if (string.IsNullOrEmpty(term))
                return Json(new List<string>(), JsonRequestBehavior.AllowGet);

            string keyword = RemoveDiacritics(term).ToLower();

            var allMovies = db.MOVIEs.ToList();
            var movieNames = allMovies
                .Where(m => RemoveDiacritics(m.MOVIENAME).ToLower().Contains(keyword))
                .Select(m => m.MOVIENAME)
                .Distinct()
                .Take(10)
                .ToList();


            return Json(movieNames, JsonRequestBehavior.AllowGet);
        }
        //tên đạo diễn
        [HttpGet]
        public JsonResult GetDirectorSuggestions(string term)
        {
            if (string.IsNullOrEmpty(term))
                return Json(new List<string>(), JsonRequestBehavior.AllowGet);

            string keyword = RemoveDiacritics(term).ToLower();

            var directors = db.MOVIEs
                .Where(m => m.DIRECTOR != null)
                .ToList()
                .Where(m => !string.IsNullOrEmpty(m.DIRECTOR.DIRECTORNAME) &&
                            RemoveDiacritics(m.DIRECTOR.DIRECTORNAME).ToLower().Contains(keyword))
                .Select(m => m.DIRECTOR.DIRECTORNAME)
                .Distinct()
                .Take(10)
                .ToList();

            return Json(directors, JsonRequestBehavior.AllowGet);
        }

        // GET: Movies/Details/5
        //public ActionResult Details(long? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    MOVIE mOVIE = db.MOVIEs.Find(id);
        //    if (mOVIE == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(mOVIE);
        //}
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var mOVIE = db.MOVIEs
                .Include(m => m.AGERATING)
                .Include(m => m.DIRECTOR)
                .Include(m => m.NATION)
                .Include(m => m.GENREs)
                .Include(m => m.ACTORs)
                .Include(m => m.REVIEWs.Select(r => r.CUSTOMER))
                .FirstOrDefault(m => m.MOVIEID == id);

            if (mOVIE == null)
            {
                return HttpNotFound();
            }

            return View(mOVIE);
        }

        // GET: Movies/Create
        public ActionResult Create()
        {
            ViewBag.AGEID = new SelectList(db.AGERATINGs, "AGEID", "AGERATING1");
            ViewBag.DIRECTORID = new SelectList(db.DIRECTORs, "DIRECTORID", "DIRECTORNAME");
            ViewBag.NATIONID = new SelectList(db.NATIONs, "NATIONID", "NATIONNAME");
            ViewBag.Genres = new MultiSelectList(db.GENREs, "GENREID", "GENRENAME");
            ViewBag.Actors = new MultiSelectList(db.ACTORs, "ACTORID", "ACTORNAME");

            return View();
        }

        // POST: Movies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MOVIE movie, int[] selectedGenres, int[] selectedActors, HttpPostedFileBase uploadImage)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Xử lý hình ảnh
                    if (uploadImage != null && uploadImage.ContentLength > 0)
                    {
                        // Validate file type
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        var fileExtension = Path.GetExtension(uploadImage.FileName).ToLower();

                        if (!allowedExtensions.Contains(fileExtension))
                        {
                            ModelState.AddModelError("", "Chỉ cho phép upload file hình ảnh (.jpg, .jpeg, .png, .gif)");
                            PopulateViewBags(movie);
                            return View(movie);
                        }

                        // Tạo tên file duy nhất
                        var fileName = Path.GetFileName(uploadImage.FileName);
                        var uniqueFileName = Guid.NewGuid() + "_" + fileName;
                        var uploadDir = Server.MapPath("~/Content/Img");

                        if (!Directory.Exists(uploadDir))
                        {
                            Directory.CreateDirectory(uploadDir);
                        }

                        var filePath = Path.Combine(uploadDir, uniqueFileName);
                        uploadImage.SaveAs(filePath);

                        // Lưu tên file (không có đường dẫn)
                        movie.POSTER = uniqueFileName;
                    }
                    // Thêm thể loại
                    if (selectedGenres != null)
                    {
                        foreach (var genreId in selectedGenres)
                        {
                            var genre = db.GENREs.Find(genreId);
                            if (genre != null)
                            {
                                movie.GENREs.Add(genre);
                            }
                        }
                    }

                    // Thêm diễn viên
                    if (selectedActors != null)
                    {
                        foreach (var actorId in selectedActors)
                        {
                            var actor = db.ACTORs.Find(actorId);
                            if (actor != null)
                            {
                                movie.ACTORs.Add(actor);
                            }
                        }
                    }

                    db.MOVIEs.Add(movie);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var eve in ex.EntityValidationErrors)
                    {
                        foreach (var ve in eve.ValidationErrors)
                        {
                            ModelState.AddModelError(ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi lưu phim: " + ex.Message);
                }
            }

            PopulateViewBags(movie);
            return View(movie);
        }

        // GET: Movies/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var movie = db.MOVIEs
                .Include(m => m.GENREs)
                .Include(m => m.ACTORs)
                .FirstOrDefault(m => m.MOVIEID == id);

            if (movie == null)
            {
                return HttpNotFound();
            }

            PopulateViewBags(movie);
            return View(movie);
        }

        // POST: Movies/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MOVIE movie, int[] selectedGenres, int[] selectedActors, HttpPostedFileBase uploadImage)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingMovie = db.MOVIEs
                        .Include(m => m.GENREs)
                        .Include(m => m.ACTORs)
                        .FirstOrDefault(m => m.MOVIEID == movie.MOVIEID);

                    if (existingMovie == null)
                    {
                        return HttpNotFound();
                    }

                    // Update movie properties
                    existingMovie.MOVIENAME = movie.MOVIENAME;
                    existingMovie.DESCRIPTION = movie.DESCRIPTION;
                    existingMovie.DURATION = movie.DURATION;
                    existingMovie.RELEASEDATE = movie.RELEASEDATE;
                    existingMovie.MOVIESTATUS = movie.MOVIESTATUS;
                    existingMovie.AGEID = movie.AGEID;
                    existingMovie.DIRECTORID = movie.DIRECTORID;
                    existingMovie.NATIONID = movie.NATIONID;

                    // Handle image upload
                    if (uploadImage != null && uploadImage.ContentLength > 0)
                    {
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        var fileExtension = Path.GetExtension(uploadImage.FileName).ToLower();

                        if (!allowedExtensions.Contains(fileExtension))
                        {
                            ModelState.AddModelError("", "Chỉ cho phép upload file hình ảnh (.jpg, .jpeg, .png, .gif)");
                            PopulateViewBags(movie);
                            return View(movie);
                        }

                        // Delete old image if exists
                        if (!string.IsNullOrEmpty(existingMovie.POSTER))
                        {
                            var oldImagePath = Server.MapPath("~" + existingMovie.POSTER);
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        var fileName = Path.GetFileName(uploadImage.FileName);
                        var uniqueFileName = Guid.NewGuid() + "_" + fileName;
                        var uploadsPath = Server.MapPath("~/Content/Images/Movies");

                        if (!Directory.Exists(uploadsPath))
                        {
                            Directory.CreateDirectory(uploadsPath);
                        }

                        var filePath = Path.Combine(uploadsPath, uniqueFileName);
                        uploadImage.SaveAs(filePath);
                        existingMovie.POSTER = "/Content/Images/Movies/" + uniqueFileName;
                    }

                    // Update genres
                    existingMovie.GENREs.Clear();
                    if (selectedGenres != null && selectedGenres.Length > 0)
                    {
                        foreach (var genreId in selectedGenres)
                        {
                            var genre = db.GENREs.Find(genreId);
                            if (genre != null)
                            {
                                existingMovie.GENREs.Add(genre);
                            }
                        }
                    }

                    // Update actors
                    existingMovie.ACTORs.Clear();
                    if (selectedActors != null && selectedActors.Length > 0)
                    {
                        foreach (var actorId in selectedActors)
                        {
                            var actor = db.ACTORs.Find(actorId);
                            if (actor != null)
                            {
                                existingMovie.ACTORs.Add(actor);
                            }
                        }
                    }

                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi cập nhật phim: " + ex.Message);
                }
            }

            PopulateViewBags(movie);
            return View(movie);
        }
        //public ActionResult Edit(long? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    MOVIE mOVIE = db.MOVIEs.Find(id);
        //    if (mOVIE == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.AGEID = new SelectList(db.AGERATINGs, "AGEID", "AGERATING1", mOVIE.AGEID);
        //    ViewBag.DIRECTORID = new SelectList(db.DIRECTORs, "DIRECTORID", "DIRECTORNAME", mOVIE.DIRECTORID);
        //    ViewBag.NATIONID = new SelectList(db.NATIONs, "NATIONID", "NATIONNAME", mOVIE.NATIONID);
        //    return View(mOVIE);
        //}

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MOVIEID,DIRECTORID,NATIONID,AGEID,MOVIENAME,DURATION,DESCRIPTION,RELEASEDATE,MOVIESTATUS,POSTER")] MOVIE mOVIE)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mOVIE).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AGEID = new SelectList(db.AGERATINGs, "AGEID", "AGERATING1", mOVIE.AGEID);
            ViewBag.DIRECTORID = new SelectList(db.DIRECTORs, "DIRECTORID", "DIRECTORNAME", mOVIE.DIRECTORID);
            ViewBag.NATIONID = new SelectList(db.NATIONs, "NATIONID", "NATIONNAME", mOVIE.NATIONID);
            return View(mOVIE);
        }


        // GET: Movies/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MOVIE mOVIE = db.MOVIEs.Find(id);
            if (mOVIE == null)
            {
                return HttpNotFound();
            }
            return View(mOVIE);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            MOVIE mOVIE = db.MOVIEs.Find(id);
            db.MOVIEs.Remove(mOVIE);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // Helper method to populate ViewBags
        private void PopulateViewBags(MOVIE movie = null)
        {
            ViewBag.AGEID = new SelectList(db.AGERATINGs, "AGEID", "AGERATING1", movie?.AGEID);
            ViewBag.DIRECTORID = new SelectList(db.DIRECTORs, "DIRECTORID", "DIRECTORNAME", movie?.DIRECTORID);
            ViewBag.NATIONID = new SelectList(db.NATIONs, "NATIONID", "NATIONNAME", movie?.NATIONID);

            if (movie != null)
            {
                ViewBag.Genres = new MultiSelectList(db.GENREs, "GENREID", "GENRENAME",
                    movie.GENREs?.Select(g => g.GENREID).ToArray());
                ViewBag.Actors = new MultiSelectList(db.ACTORs, "ACTORID", "ACTORNAME",
                    movie.ACTORs?.Select(a => a.ACTORID).ToArray());
            }
            else
            {
                ViewBag.Genres = new MultiSelectList(db.GENREs, "GENREID", "GENRENAME");
                ViewBag.Actors = new MultiSelectList(db.ACTORs, "ACTORID", "ACTORNAME");
            }
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
}
