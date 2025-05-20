using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GalaxyCinema.Models;

namespace GalaxyCinema.Controllers
{
    public class MoviesController : Controller
    {
        private GalaxyCinemaEntities db = new GalaxyCinemaEntities();

        // GET: Movies
        public ActionResult Index()
        {
            var mOVIEs = db.MOVIEs.Include(m => m.AGERATING).Include(m => m.DIRECTOR).Include(m => m.NATION);

            ViewBag.Genres = new SelectList(db.GENREs, "GENREID", "GENRENAME");

            return View(mOVIEs.ToList());
        }

        // GET: Movies/Search
        [HttpGet]
        public ActionResult Search()
        {
            ViewBag.Genres = new SelectList(db.GENREs, "GENREID", "GENRENAME");
            return View();
        }

        // POST: Movies/Search
        [HttpPost]
        public ActionResult Search(string movieName, int? genreId, string directorName, string status)
        {
            // Kiểm tra nếu ko nhập bất kỳ thông tin tìm kiếm nào
            if (string.IsNullOrWhiteSpace(movieName) && !genreId.HasValue &&
                string.IsNullOrWhiteSpace(directorName) && string.IsNullOrWhiteSpace(status))
            {
                ViewBag.SearchError = "Vui lòng nhập thông tin phim cần tìm";
                ViewBag.Genres = new SelectList(db.GENREs, "GENREID", "GENRENAME");
                return View();
            }

            var query = db.MOVIEs.AsQueryable();

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
                if (status == "Đang chiếu")
                {
                    query = db.MOVIEs
                        .Where(m => m.MOVIESTATUS == "Đang chiếu")
                        .Include(m => m.DIRECTOR)
                        .Include(m => m.GENREs);

                    var resultNowShowing = query.ToList();
                    ViewBag.Genres = new SelectList(db.GENREs, "GENREID", "GENRENAME");
                    return View(resultNowShowing);
                }
                else
                {
                    query = query.Where(m => m.MOVIESTATUS == status);
                }
            }

            var result = query.Include(m => m.DIRECTOR).Include(m => m.GENREs).ToList();

            ViewBag.Genres = new SelectList(db.GENREs, "GENREID", "GENRENAME");
            return View(result);
        }

        // GET: Movies/Details/5
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
            ViewBag.AgeRatings = new SelectList(db.AGERATINGs, "AGEID", "AGERATING1");

            return View();
        }

        // POST: Movies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MOVIE movie, string[] genreNames, string[] actorNames, HttpPostedFileBase uploadImage)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // xử lý hình
                    if (uploadImage != null && uploadImage.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(uploadImage.FileName);
                        var uniqueFileName = Guid.NewGuid() + "_" + fileName;
                        var path = Path.Combine(Server.MapPath("~/Content/Images/Movies"), uniqueFileName);
                        Directory.CreateDirectory(Path.GetDirectoryName(path));
                        uploadImage.SaveAs(path);
                        movie.POSTER = "/Content/Images/Movies/" + uniqueFileName;
                    }

                    db.MOVIEs.Add(movie);

                    db.SaveChanges();

                    if (genreNames != null && genreNames.Length > 0)
                    {
                        foreach (var genreName in genreNames)
                        {
                            var genre = db.GENREs.FirstOrDefault(g => g.GENRENAME.Trim().ToLower() == genreName.Trim().ToLower());

                            if (genre == null)
                            {
                                genre = new GENRE { GENRENAME = genreName.Trim() };
                                db.GENREs.Add(genre);
                                db.SaveChanges();
                            }

                            movie.GENREs.Add(genre);
                        }
                    }

                    if (actorNames != null && actorNames.Length > 0)
                    {
                        foreach (var actorName in actorNames)
                        {
                            var actor = db.ACTORs.FirstOrDefault(a => a.ACTORNAME.Trim().ToLower() == actorName.Trim().ToLower());

                            if (actor == null)
                            {
                                actor = new ACTOR { ACTORNAME = actorName.Trim() };
                                db.ACTORs.Add(actor);
                                db.SaveChanges();
                            }

                            movie.ACTORs.Add(actor);
                        }
                    }

                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error saving the movie: " + ex.Message);
                }
            }

            ViewBag.AGEID = new SelectList(db.AGERATINGs, "AGEID", "AGERATING1", movie.AGEID);
            ViewBag.DIRECTORID = new SelectList(db.DIRECTORs, "DIRECTORID", "DIRECTORNAME", movie.DIRECTORID);
            ViewBag.NATIONID = new SelectList(db.NATIONs, "NATIONID", "NATIONNAME", movie.NATIONID);
            ViewBag.Countries = new SelectList(db.NATIONs, "NATIONID", "NATIONNAME", movie.NATIONID);
            ViewBag.AgeRatings = new SelectList(db.AGERATINGs, "AGEID", "AGERATING1", movie.AGEID);

            return View(movie);
        }

    }
}