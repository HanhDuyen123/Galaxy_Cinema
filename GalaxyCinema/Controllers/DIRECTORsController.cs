using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GalaxyCinema.Models;

namespace GalaxyCinema.Controllers
{
    public class DIRECTORsController : Controller
    {
        private GalaxyCinemaEntities db = new GalaxyCinemaEntities();

        // GET: DIRECTORs
        public ActionResult Index()
        {
            return View(db.DIRECTORs.ToList());
        }

        // GET: DIRECTORs/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DIRECTOR dIRECTOR = db.DIRECTORs.Find(id);
            if (dIRECTOR == null)
            {
                return HttpNotFound();
            }
            return View(dIRECTOR);
        }

        // GET: DIRECTORs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DIRECTORs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DIRECTORID,DIRECTORNAME")] DIRECTOR dIRECTOR)
        {
            if (ModelState.IsValid)
            {
                db.DIRECTORs.Add(dIRECTOR);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dIRECTOR);
        }

        // GET: DIRECTORs/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DIRECTOR dIRECTOR = db.DIRECTORs.Find(id);
            if (dIRECTOR == null)
            {
                return HttpNotFound();
            }
            return View(dIRECTOR);
        }

        // POST: DIRECTORs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DIRECTORID,DIRECTORNAME")] DIRECTOR dIRECTOR)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dIRECTOR).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dIRECTOR);
        }

        // GET: DIRECTORs/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DIRECTOR dIRECTOR = db.DIRECTORs.Find(id);
            if (dIRECTOR == null)
            {
                return HttpNotFound();
            }
            return View(dIRECTOR);
        }

        // POST: DIRECTORs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            DIRECTOR dIRECTOR = db.DIRECTORs.Find(id);
            db.DIRECTORs.Remove(dIRECTOR);
            db.SaveChanges();
            return RedirectToAction("Index");
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
