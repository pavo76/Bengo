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
using Microsoft.AspNet.Identity;

namespace Bengo.Controllers
{
    [Authorize]
    public class VocabularyController : Controller
    {
        private BengoContext db = new BengoContext();

        // GET: Vocabulary
        public ActionResult Index()
        {
            return View();
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
            string userName = User.Identity.GetUserName();

            var UserData = db.UserData.Where(ud => ud.UserName == userName).
                                    Select(ud => ud.VocabularyList).First();

            List<Vocabulary> result = new List<Vocabulary>();
            if (UserData != null)
            {
                List<string> UserVocabList = UserData.Split(',').ToList();


                if (UserVocabList.Count > 0)
                {
                    result = (from vocab in db.Vocabulary
                              where (!UserVocabList.Contains(vocab.ID.ToString()))
                              select vocab).Take(5).ToList();
                }
            }
            else
            {
                result = (from vocab in db.Vocabulary
                          select vocab).Take(5).ToList();
            }

            List<LearnViewModel> model = new List<LearnViewModel>();
            foreach (var vocab in result)
            {
                List<string> answers = (from word in db.Vocabulary
                                        where word.Meaning != vocab.Meaning
                                        select word.Meaning).ToList();

                model.Add(new LearnViewModel(vocab.ID, vocab.VocabularyUnit, vocab.Meaning, answers[0], answers[1], answers[2]));
            }
            String json = "{'items':[";
            String IDList = "";
            for (int i = 0; i < model.Count; i++)
            {
                if (i < model.Count - 1)
                {
                    json += "{'id':'" + model[i].Id + "','word':'" + model[i].Word + "','meaning':'" + model[i].Meaning + "','option1':'" + model[i].Ans1 + "','option2':'" + model[i].Ans2 + "','option3':'" + model[i].Ans3 + "','score':0" + "},";
                    IDList += model[i].Id + ",";
                }
                else if (i == model.Count - 1)
                {
                    json += "{'id':'" + model[i].Id + "','word':'" + model[i].Word + "','meaning':'" + model[i].Meaning + "','option1':'" + model[i].Ans1 + "','option2':'" + model[i].Ans2 + "','option3':'" + model[i].Ans3 + "','score':0" + "}";
                    IDList += model[i].Id;
                }
            }
            json += "]}";
            ViewBag.IDList = IDList;
            return View("Learn", null, json);
        }


        public ActionResult FinishLearning(string IDList)
        {
            UserDatasController UDController = new UserDatasController();
            Vocabulary_PracticeController VPController = new Vocabulary_PracticeController();
            string userName = User.Identity.GetUserName();
            var userID = db.UserData.Where(ud => ud.UserName == userName).
                                    Select(ud => ud.ID).First();
            UserData userData = db.UserData.Find(userID);
            if (userData.VocabularyList == "" || userData.VocabularyList == "")
            {
                userData.VocabularyList += IDList;
            }
            else
            {
                userData.VocabularyList += "," + IDList;
            }
            UDController.Edit(userData);

            List<string> IDtoList = IDList.Split(',').ToList();
            foreach (var vocabID in IDtoList)
            {
                Vocabulary_Practice vpData = new Vocabulary_Practice
                {
                    UserName = userName,
                    VocabularyID = Int32.Parse(vocabID),
                    LastPracticed = DateTime.Now,
                    RepeatInterval = 1
                };

                VPController.Create(vpData);
            }
            return RedirectToAction("Index");
        }

        public ActionResult Practice()
        {
            string userName = User.Identity.GetUserName();
            //Get list of vocabulary IDs due for practice
            List<int> vocabIDList=db.Vocabulary_Practice.Where(vp => vp.LastPracticed <= DateTime.Now && vp.UserName==userName).Select(vp=>vp.VocabularyID).ToList();
            //Get the list of vocabularies based on the list above
            List<Vocabulary> vocabList = db.Vocabulary.Where(v => vocabIDList.Contains(v.ID)).ToList();

            //Turn vocabulary list into apropriate JSON
            List<LearnViewModel> model = new List<LearnViewModel>();
            foreach (var vocab in vocabList)
            {
                List<string> answers = vocabList.Where(v => v.ID != vocab.ID).Select(v=>v.Meaning).Take(3).ToList();

                model.Add(new LearnViewModel(vocab.ID, vocab.VocabularyUnit, vocab.Meaning, answers[0], answers[1], answers[2]));
            }
            String json = "{'items':[";
            String IDList = "";
            for (int i = 0; i < model.Count; i++)
            {
                if (i < model.Count - 1)
                {
                    json += "{'id':'" + model[i].Id + "','word':'" + model[i].Word + "','meaning':'" + model[i].Meaning + "','option1':'" + model[i].Ans1 + "','option2':'" + model[i].Ans2 + "','option3':'" + model[i].Ans3 + "','score':0" + "},";
                    IDList += model[i].Id + ",";
                }
                else if (i == model.Count - 1)
                {
                    json += "{'id':'" + model[i].Id + "','word':'" + model[i].Word + "','meaning':'" + model[i].Meaning + "','option1':'" + model[i].Ans1 + "','option2':'" + model[i].Ans2 + "','option3':'" + model[i].Ans3 + "','score':0" + "}";
                    IDList += model[i].Id;
                }
            }
            json += "]}";
            ViewBag.IDList = IDList;
            return View("Practice", null, json);
        }
    }
}
