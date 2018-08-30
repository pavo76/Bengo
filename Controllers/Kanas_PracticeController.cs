using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bengo.DAL;
using Bengo.Models;

namespace Bengo.Controllers
{
    public class Kanas_PracticeController : Controller
    {
        private BengoContext db = new BengoContext();

        // GET: Kanas_Practice
        public ActionResult Index()
        {
            return View();
        }

        // GET: Kanas_Practice/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Kanas_Practice/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Kanas_Practice/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Kanas_Practice/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Kanas_Practice/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,UserName,VocabularyID,LastPracticed,RepeatInterval")] Kana_Practice kanas_Practice)
        {
            if (ModelState.IsValid)
            {
                db.Kana_Practice.Add(kanas_Practice);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.KanaID = new SelectList(db.Kana, "ID", "KanaUnit", kanas_Practice.Kana);
            return View(kanas_Practice);
        }

        // GET: Kanas_Practice/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Kanas_Practice/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
