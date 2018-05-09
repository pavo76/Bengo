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
    [Authorize]
    public class KanjiController : Controller
    {
        private BengoContext db = new BengoContext();

        // GET: Kanji
        public ActionResult Index()
        {
            return View();
        }

        // GET: Kanji/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kanji kanji = db.Kanji.Find(id);
            if (kanji == null)
            {
                return HttpNotFound();
            }
            return View(kanji);
        }

        // GET: Kanji/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Kanji/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,KanjiUnit,Meaning,OnReading,KunReading,SRC")] Kanji kanji)
        {
            if (ModelState.IsValid)
            {
                db.Kanji.Add(kanji);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(kanji);
        }

        // GET: Kanji/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kanji kanji = db.Kanji.Find(id);
            if (kanji == null)
            {
                return HttpNotFound();
            }
            return View(kanji);
        }

        // POST: Kanji/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,KanjiUnit,Meaning,OnReading,KunReading,SRC")] Kanji kanji)
        {
            if (ModelState.IsValid)
            {
                db.Entry(kanji).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(kanji);
        }

        // GET: Kanji/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kanji kanji = db.Kanji.Find(id);
            if (kanji == null)
            {
                return HttpNotFound();
            }
            return View(kanji);
        }

        // POST: Kanji/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Kanji kanji = db.Kanji.Find(id);
            db.Kanji.Remove(kanji);
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


        // GET: Kanji/Learn
        public ActionResult Learn()
        {
            return View();
        }

        // GET: Kanji/Practice
        public ActionResult Practice()
        {
            return View();
        }
    }
}
