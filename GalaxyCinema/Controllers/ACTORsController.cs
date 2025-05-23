﻿using System;
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
    public class ACTORsController : Controller
    {
        private GalaxyCinemaEntities db = new GalaxyCinemaEntities();

        // GET: ACTORs
        public ActionResult Index()
        {
            return View(db.ACTORs.ToList());
        }

        // GET: ACTORs/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ACTOR aCTOR = db.ACTORs.Find(id);
            if (aCTOR == null)
            {
                return HttpNotFound();
            }
            return View(aCTOR);
        }

        // GET: ACTORs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ACTORs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ACTORID,ACTORNAME")] ACTOR aCTOR)
        {
            if (ModelState.IsValid)
            {
                db.ACTORs.Add(aCTOR);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(aCTOR);
        }

        // GET: ACTORs/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ACTOR aCTOR = db.ACTORs.Find(id);
            if (aCTOR == null)
            {
                return HttpNotFound();
            }
            return View(aCTOR);
        }

        // POST: ACTORs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ACTORID,ACTORNAME")] ACTOR aCTOR)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aCTOR).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(aCTOR);
        }

        // GET: ACTORs/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ACTOR aCTOR = db.ACTORs.Find(id);
            if (aCTOR == null)
            {
                return HttpNotFound();
            }
            return View(aCTOR);
        }

        // POST: ACTORs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ACTOR aCTOR = db.ACTORs.Find(id);
            db.ACTORs.Remove(aCTOR);
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
