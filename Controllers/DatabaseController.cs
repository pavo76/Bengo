using Bengo.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bengo.Controllers
{
    public class DatabaseController : Controller
    {
        private BengoContext db = new BengoContext();

        // GET: Database
        public ActionResult Index()
        {
            return View();
        }

        // GET: Database/Vocabulary
        public ActionResult Vocabulary()
        {
            return View(db.Vocabulary.ToList());
        }

        // GET: Database/Kanji
        public ActionResult Kanji()
        {
            return View(db.Kanji.ToList());
        }

        // GET: Database/Grammar
        public ActionResult Grammar()
        {
            return View(db.Grammar.ToList());
        }


        // GET: Database/Vocabulary
        public ActionResult Kana()
        {
            return View(db.Kana.ToList());
        }

        // GET: Database/Vocabulary
        public ActionResult UserData()
        {
            return View(db.UserData.ToList());
        }

        // GET: Database/Vocabulary
        public ActionResult Texts()
        {
            return View(db.Text.ToList());
        }

        // GET: Database/Vocabulary
        public ActionResult VocabularyPractice()
        {
            return View(db.Vocabulary_Practice.ToList());
        }
    }
}