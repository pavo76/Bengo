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
    public class Grammar_PracticeController : Controller
    {
        private BengoContext db = new BengoContext();

        // GET: Grammar_Practice
        public ActionResult Index()
        {
            var grammar_Practice = db.Grammar_Practice.Include(g => g.Grammar);
            return View(grammar_Practice.ToList());
        }

        // GET: Grammar_Practice/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Grammar_Practice grammar_Practice = db.Grammar_Practice.Find(id);
            if (grammar_Practice == null)
            {
                return HttpNotFound();
            }
            return View(grammar_Practice);
        }

        // GET: Grammar_Practice/Create
        public ActionResult Create()
        {
            ViewBag.GrammarID = new SelectList(db.Grammar, "ID", "GrammarUnit");
            return View();
        }

        // POST: Grammar_Practice/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,UserName,GrammarID,LastPracticed,RepeatInterval")] Grammar_Practice grammar_Practice)
        {
            if (ModelState.IsValid)
            {
                db.Grammar_Practice.Add(grammar_Practice);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.GrammarID = new SelectList(db.Grammar, "ID", "GrammarUnit", grammar_Practice.GrammarID);
            return View(grammar_Practice);
        }

        // GET: Grammar_Practice/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Grammar_Practice grammar_Practice = db.Grammar_Practice.Find(id);
            if (grammar_Practice == null)
            {
                return HttpNotFound();
            }
            ViewBag.GrammarID = new SelectList(db.Grammar, "ID", "GrammarUnit", grammar_Practice.GrammarID);
            return View(grammar_Practice);
        }

        // POST: Grammar_Practice/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,UserName,GrammarID,LastPracticed,RepeatInterval")] Grammar_Practice grammar_Practice)
        {
            if (ModelState.IsValid)
            {
                db.Entry(grammar_Practice).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.GrammarID = new SelectList(db.Grammar, "ID", "GrammarUnit", grammar_Practice.GrammarID);
            return View(grammar_Practice);
        }

        // GET: Grammar_Practice/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Grammar_Practice grammar_Practice = db.Grammar_Practice.Find(id);
            if (grammar_Practice == null)
            {
                return HttpNotFound();
            }
            return View(grammar_Practice);
        }

        // POST: Grammar_Practice/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Grammar_Practice grammar_Practice = db.Grammar_Practice.Find(id);
            db.Grammar_Practice.Remove(grammar_Practice);
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
