using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Bengo.DAL;
using Bengo.Models;

namespace Bengo.Controllers
{
    public class Kanji_PracticeController : Controller
    {
        private BengoContext db = new BengoContext();

        // GET: Kanji_Practice
        public ActionResult Index()
        {
            var kanji_Practice = db.Kanji_Practice.Include(k => k.Kanji);
            return View(kanji_Practice.ToList());
        }

        // GET: Kanji_Practice/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kanji_Practice kanji_Practice = db.Kanji_Practice.Find(id);
            if (kanji_Practice == null)
            {
                return HttpNotFound();
            }
            return View(kanji_Practice);
        }

        // GET: Kanji_Practice/Create
        public ActionResult Create()
        {
            ViewBag.KanjiID = new SelectList(db.Kanji, "ID", "KanjiUnit");
            return View();
        }

        // POST: Kanji_Practice/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,UserName,KanjiID,LastPracticed,RepeatInterval")] Kanji_Practice kanji_Practice)
        {
            if (ModelState.IsValid)
            {
                db.Kanji_Practice.Add(kanji_Practice);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.KanjiID = new SelectList(db.Kanji, "ID", "KanjiUnit", kanji_Practice.KanjiID);
            return View(kanji_Practice);
        }

        // GET: Kanji_Practice/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kanji_Practice kanji_Practice = db.Kanji_Practice.Find(id);
            if (kanji_Practice == null)
            {
                return HttpNotFound();
            }
            ViewBag.KanjiID = new SelectList(db.Kanji, "ID", "KanjiUnit", kanji_Practice.KanjiID);
            return View(kanji_Practice);
        }

        // POST: Kanji_Practice/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,UserName,KanjiID,LastPracticed,RepeatInterval")] Kanji_Practice kanji_Practice)
        {
            if (ModelState.IsValid)
            {
                db.Entry(kanji_Practice).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.KanjiID = new SelectList(db.Kanji, "ID", "KanjiUnit", kanji_Practice.KanjiID);
            return View(kanji_Practice);
        }

        // GET: Kanji_Practice/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kanji_Practice kanji_Practice = db.Kanji_Practice.Find(id);
            if (kanji_Practice == null)
            {
                return HttpNotFound();
            }
            return View(kanji_Practice);
        }

        // POST: Kanji_Practice/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Kanji_Practice kanji_Practice = db.Kanji_Practice.Find(id);
            db.Kanji_Practice.Remove(kanji_Practice);
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
