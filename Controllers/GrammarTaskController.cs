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
    public class GrammarTaskController : Controller
    {
        private BengoContext db = new BengoContext();

        // GET: GrammarTask
        public ActionResult Index()
        {
            var grammarTask = db.GrammarTask.Include(g => g.Grammar);
            return View(grammarTask.ToList());
        }

        // GET: GrammarTask/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrammarTask grammarTask = db.GrammarTask.Find(id);
            if (grammarTask == null)
            {
                return HttpNotFound();
            }
            return View(grammarTask);
        }

        // GET: GrammarTask/Create
        public ActionResult Create()
        {
            ViewBag.GrammarID = new SelectList(db.Grammar, "ID", "GrammarUnit");
            return View();
        }

        // POST: GrammarTask/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,GrammarID,Question,CorrectAnswer,WrongAnswer1,WrongAnswer2,WrongAnswer3")] GrammarTask grammarTask)
        {
            if (ModelState.IsValid)
            {
                db.GrammarTask.Add(grammarTask);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.GrammarID = new SelectList(db.Grammar, "ID", "GrammarUnit", grammarTask.GrammarID);
            return View(grammarTask);
        }

        // GET: GrammarTask/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrammarTask grammarTask = db.GrammarTask.Find(id);
            if (grammarTask == null)
            {
                return HttpNotFound();
            }
            ViewBag.GrammarID = new SelectList(db.Grammar, "ID", "GrammarUnit", grammarTask.GrammarID);
            return View(grammarTask);
        }

        // POST: GrammarTask/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,GrammarID,Question,CorrectAnswer,WrongAnswer1,WrongAnswer2,WrongAnswer3")] GrammarTask grammarTask)
        {
            if (ModelState.IsValid)
            {
                db.Entry(grammarTask).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.GrammarID = new SelectList(db.Grammar, "ID", "GrammarUnit", grammarTask.GrammarID);
            return View(grammarTask);
        }

        // GET: GrammarTask/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrammarTask grammarTask = db.GrammarTask.Find(id);
            if (grammarTask == null)
            {
                return HttpNotFound();
            }
            return View(grammarTask);
        }

        // POST: GrammarTask/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GrammarTask grammarTask = db.GrammarTask.Find(id);
            db.GrammarTask.Remove(grammarTask);
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
