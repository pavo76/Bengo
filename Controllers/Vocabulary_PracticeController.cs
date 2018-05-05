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
    public class Vocabulary_PracticeController : Controller
    {
        private BengoContext db = new BengoContext();

        // GET: Vocabulary_Practice
        public ActionResult Index()
        {
            var vocabulary_Practice = db.Vocabulary_Practice.Include(v => v.Vocabulary);
            return View(vocabulary_Practice.ToList());
        }

        // GET: Vocabulary_Practice/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vocabulary_Practice vocabulary_Practice = db.Vocabulary_Practice.Find(id);
            if (vocabulary_Practice == null)
            {
                return HttpNotFound();
            }
            return View(vocabulary_Practice);
        }

        // GET: Vocabulary_Practice/Create
        public ActionResult Create()
        {
            ViewBag.VocabularyID = new SelectList(db.Vocabulary, "ID", "VocabularyUnit");
            return View();
        }

        // POST: Vocabulary_Practice/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,UserName,VocabularyID,LastPracticed,RepeatInterval")] Vocabulary_Practice vocabulary_Practice)
        {
            if (ModelState.IsValid)
            {
                db.Vocabulary_Practice.Add(vocabulary_Practice);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.VocabularyID = new SelectList(db.Vocabulary, "ID", "VocabularyUnit", vocabulary_Practice.VocabularyID);
            return View(vocabulary_Practice);
        }

        // GET: Vocabulary_Practice/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vocabulary_Practice vocabulary_Practice = db.Vocabulary_Practice.Find(id);
            if (vocabulary_Practice == null)
            {
                return HttpNotFound();
            }
            ViewBag.VocabularyID = new SelectList(db.Vocabulary, "ID", "VocabularyUnit", vocabulary_Practice.VocabularyID);
            return View(vocabulary_Practice);
        }

        // POST: Vocabulary_Practice/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,UserName,VocabularyID,LastPracticed,RepeatInterval")] Vocabulary_Practice vocabulary_Practice)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vocabulary_Practice).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.VocabularyID = new SelectList(db.Vocabulary, "ID", "VocabularyUnit", vocabulary_Practice.VocabularyID);
            return View(vocabulary_Practice);
        }

        // GET: Vocabulary_Practice/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vocabulary_Practice vocabulary_Practice = db.Vocabulary_Practice.Find(id);
            if (vocabulary_Practice == null)
            {
                return HttpNotFound();
            }
            return View(vocabulary_Practice);
        }

        // POST: Vocabulary_Practice/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Vocabulary_Practice vocabulary_Practice = db.Vocabulary_Practice.Find(id);
            db.Vocabulary_Practice.Remove(vocabulary_Practice);
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
