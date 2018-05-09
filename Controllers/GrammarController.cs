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
    public class GrammarController : Controller
    {
        private BengoContext db = new BengoContext();

        // GET: Grammar
        public ActionResult Index()
        {
            return View();
        }

        // GET: Grammar/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Grammar grammar = db.Grammar.Find(id);
            if (grammar == null)
            {
                return HttpNotFound();
            }
            return View(grammar);
        }

        // GET: Grammar/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Grammar/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,GrammarUnit,Explanation")] Grammar grammar)
        {
            if (ModelState.IsValid)
            {
                db.Grammar.Add(grammar);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(grammar);
        }

        // GET: Grammar/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Grammar grammar = db.Grammar.Find(id);
            if (grammar == null)
            {
                return HttpNotFound();
            }
            return View(grammar);
        }

        // POST: Grammar/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,GrammarUnit,Explanation")] Grammar grammar)
        {
            if (ModelState.IsValid)
            {
                db.Entry(grammar).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(grammar);
        }

        // GET: Grammar/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Grammar grammar = db.Grammar.Find(id);
            if (grammar == null)
            {
                return HttpNotFound();
            }
            return View(grammar);
        }

        // POST: Grammar/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Grammar grammar = db.Grammar.Find(id);
            db.Grammar.Remove(grammar);
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

        // GET: Grammar/Learn
        public ActionResult Learn()
        {
            return View();
        }

        // GET: Grammar/Practice
        public ActionResult Practice()
        {
            return View();
        }
    }
}
