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
    public class VocabularyController : Controller
    {
        private BengoContext db = new BengoContext();

        // GET: Vocabulary
        public ActionResult Index()
        {
            var vocabulary = db.Vocabulary.Include(v => v.Category);
            return View(vocabulary.ToList());
        }

        // GET: Vocabulary/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vocabulary vocabulary = db.Vocabulary.Find(id);
            if (vocabulary == null)
            {
                return HttpNotFound();
            }
            return View(vocabulary);
        }

        // GET: Vocabulary/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Category, "ID", "CategoryName");
            return View();
        }

        // POST: Vocabulary/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,VocabularyUnit,Kana,Romaji,Meaning,POS,CategoryID")] Vocabulary vocabulary)
        {
            if (ModelState.IsValid)
            {
                db.Vocabulary.Add(vocabulary);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Category, "ID", "CategoryName", vocabulary.CategoryID);
            return View(vocabulary);
        }

        // GET: Vocabulary/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vocabulary vocabulary = db.Vocabulary.Find(id);
            if (vocabulary == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Category, "ID", "CategoryName", vocabulary.CategoryID);
            return View(vocabulary);
        }

        // POST: Vocabulary/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,VocabularyUnit,Kana,Romaji,Meaning,POS,CategoryID")] Vocabulary vocabulary)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vocabulary).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Category, "ID", "CategoryName", vocabulary.CategoryID);
            return View(vocabulary);
        }

        // GET: Vocabulary/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vocabulary vocabulary = db.Vocabulary.Find(id);
            if (vocabulary == null)
            {
                return HttpNotFound();
            }
            return View(vocabulary);
        }

        // POST: Vocabulary/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Vocabulary vocabulary = db.Vocabulary.Find(id);
            db.Vocabulary.Remove(vocabulary);
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


        // GET: Learn
        public ActionResult Learn()
        {
            var VocabList = db.Vocabulary.ToList().Take(5);
            return View(VocabList);
        }

        public ActionResult Practice()
        {

            return View();
        }
    }
}
