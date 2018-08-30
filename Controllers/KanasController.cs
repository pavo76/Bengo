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
    public class KanasController : Controller
    {
        private BengoContext db = new BengoContext();

        // GET: Kanas
        public ActionResult Index()
        {
            string userName = User.Identity.GetUserName();
            int duePracticeCount = db.Kana_Practice.Where(kp => kp.LastPracticed <= DateTime.Now && kp.UserName == userName).Count();

            ViewBag.DuePracticeCount = duePracticeCount;
            return View(db.Kana.ToList());
        }

        // GET: Kanas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kana kana = db.Kana.Find(id);
            if (kana == null)
            {
                return HttpNotFound();
            }
            return View(kana);
        }

        // GET: Kanas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Kanas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Letter,Reading")] Kana kana)
        {
            if (ModelState.IsValid)
            {
                db.Kana.Add(kana);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(kana);
        }

        // GET: Kanas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kana kana = db.Kana.Find(id);
            if (kana == null)
            {
                return HttpNotFound();
            }
            return View(kana);
        }

        // POST: Kanas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Letter,Reading")] Kana kana)
        {
            if (ModelState.IsValid)
            {
                db.Entry(kana).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(kana);
        }

        // GET: Kanas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kana kana = db.Kana.Find(id);
            if (kana == null)
            {
                return HttpNotFound();
            }
            return View(kana);
        }

        // POST: Kanas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Kana kana = db.Kana.Find(id);
            db.Kana.Remove(kana);
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

        // POST: LEarn
        [HttpPost]
        public ActionResult Learn(FormCollection formData)
        {
            string userName = User.Identity.GetUserName();

            var UserKanaData = db.UserData.Where(ud => ud.UserName == userName).
                                    Select(ud => ud.KanaList).First();
            int GoalID = db.UserData.Where(ud => ud.UserName == userName).
                                    Select(ud => ud.GoalID).First();
            int itemNumber = Int32.Parse(formData.GetValues("LearnItemNumber")[0]);
            string learnForGoal = formData.GetValues("LearnFor")[0];

            List<Kana> result = new List<Kana>();
            if (UserKanaData != null)
            {
                List<string> UserKanaList = UserKanaData.Split(',').ToList();


                if (UserKanaList.Count > 0)
                {
                    if (learnForGoal == "Default")
                    {
                        result = (from character in db.Kana
                                  where (!UserKanaList.Contains(character.ID.ToString()))
                                  select character).Take(itemNumber).ToList();
                    }
                    else if (learnForGoal == "Goal" && GoalID != 0)
                    {
                        List<string> GoalKanaList = db.Text.Select(text => text.Kana_List).First().Split(',').ToList();
                        result = (from character in db.Kana
                                  where (!UserKanaList.Contains(character.ID.ToString()) && GoalKanaList.Contains(character.ID.ToString()))
                                  select character).Take(itemNumber).ToList();
                    }
                }
            }
            else
            {
                if (learnForGoal == "Default")
                {
                    result = (from character in db.Kana
                              select character).Take(itemNumber).ToList();
                }
                else if (learnForGoal == "Goal" && GoalID != 0)
                {
                    List<string> GoalKanaList = db.Text.Select(text => text.Kana_List).First().Split(',').ToList();
                    result = (from character in db.Kana
                              where (GoalKanaList.Contains(character.ID.ToString()))
                              select character).Take(itemNumber).ToList();
                }
            }

            if (result.Count > 0)
            {
                List<LearnViewModel> model = new List<LearnViewModel>();
                string ExplanationList = "";
                foreach (var character in result)
                {
                    List<string> potentialAnswers = (from kana in db.Kana
                                                     where kana.Reading != character.Reading
                                                     select kana.Reading).ToList();
                    List<string> answers = new List<string>();
                    for (var i = 0; i < 3; i++)
                    {
                        var randomIndex = new Random().Next(0, potentialAnswers.Count - 1);
                        answers.Add(potentialAnswers[randomIndex]);
                        potentialAnswers.RemoveAt(randomIndex);
                    }
                    
                    model.Add(new LearnViewModel(character.ID, character.Letter, character.Reading, answers[0], answers[1], answers[2]));
                    if (ExplanationList == "")
                    {
                        ExplanationList += $"{character.Letter}:{character.Reading}";
                    }
                    else
                    {
                        ExplanationList += $",{character.Letter}:{character.Reading}";
                    }

                }
                String json = "{'items':[";
                String IDList = "";
                for (int i = 0; i < model.Count; i++)
                {
                    if (i < model.Count - 1)
                    {
                        json += "{'id':'" + model[i].Id + "','word':'" + model[i].Item + "','meaning':'" + model[i].Meaning + "','option1':'" + model[i].Ans1 + "','option2':'" + model[i].Ans2 + "','option3':'" + model[i].Ans3 + "','score':0" + "},";
                        IDList += model[i].Id + ",";
                    }
                    else if (i == model.Count - 1)
                    {
                        json += "{'id':'" + model[i].Id + "','word':'" + model[i].Item + "','meaning':'" + model[i].Meaning + "','option1':'" + model[i].Ans1 + "','option2':'" + model[i].Ans2 + "','option3':'" + model[i].Ans3 + "','score':0" + "}";
                        IDList += model[i].Id;
                    }
                }
                json += "]}";
                ViewBag.IDList = IDList;
                ViewBag.ExplanationList = ExplanationList;
                return View("Learn", null, json);
            }
            else
            {
                return View("LearnEnd");
            }
        }

        public ActionResult FinishLearning(string IDList)
        {
            UserDatasController UDController = new UserDatasController();
            Kanas_PracticeController KPController = new Kanas_PracticeController();
            string userName = User.Identity.GetUserName();
            var userID = db.UserData.Where(ud => ud.UserName == userName).
                                    Select(ud => ud.ID).First();
            UserData userData = db.UserData.Find(userID);
            if (userData.KanaList == "")
            {
                userData.KanaList += IDList;
            }
            else
            {
                userData.KanaList += "," + IDList;
            }
            UDController.Edit(userData);

            List<string> IDtoList = IDList.Split(',').ToList();
            foreach (var kanaId in IDtoList)
            {
                Kana_Practice kpData = new Kana_Practice
                {
                    UserName = userName,
                    KanaID = Int32.Parse(kanaId),
                    LastPracticed = DateTime.Now.AddDays(1),
                    RepeatInterval = 1
                };

                KPController.Create(kpData);
            }
            return RedirectToAction("Index");
        }

        public ActionResult Practice()
        {
            string userName = User.Identity.GetUserName();
            //Get list of kana IDs due for practice
            List<int> kanaIDList = db.Kana_Practice.Where(kp => kp.LastPracticed <= DateTime.Now && kp.UserName == userName).Select(kp => kp.KanaID).ToList();
            //Get the list of kana practice data IDs
            List<int> practiceDataIDList = db.Kana_Practice.Where(kp => kp.LastPracticed <= DateTime.Now && kp.UserName == userName).Select(kp => kp.ID).ToList();
            //Get the list of vocabularies based on the list above
            List<Kana> kanaList = db.Kana.Where(v => kanaIDList.Contains(v.ID)).ToList();

            //Turn vocabulary list into apropriate JSON
            List<LearnViewModel> model = new List<LearnViewModel>();
            foreach (var kana in kanaList)
            {
                List<string> potentialAnswers = (from character in db.Kana
                                                 where character.Reading != kana.Reading
                                                 select character.Reading).ToList();
                List<string> answers = new List<string>();
                for (var i = 0; i < 3; i++)
                {
                    var randomIndex = new Random().Next(0, potentialAnswers.Count - 1);
                    answers.Add(potentialAnswers[randomIndex]);
                    potentialAnswers.RemoveAt(randomIndex);
                }

                model.Add(new LearnViewModel(kana.ID, kana.Letter, kana.Reading, answers[0], answers[1], answers[2]));
            }
            String json = "{'items':[";
            for (int i = 0; i < model.Count; i++)
            {
                if (i < model.Count - 1)
                {
                    json += "{'id':'" + model[i].Id + "','word':'" + model[i].Item + "','meaning':'" + model[i].Meaning + "','option1':'" + model[i].Ans1 + "','option2':'" + model[i].Ans2 + "','option3':'" + model[i].Ans3 + "','score':0,'finished':0" + "},";
                }
                else if (i == model.Count - 1)
                {
                    json += "{'id':'" + model[i].Id + "','word':'" + model[i].Item + "','meaning':'" + model[i].Meaning + "','option1':'" + model[i].Ans1 + "','option2':'" + model[i].Ans2 + "','option3':'" + model[i].Ans3 + "','score':0,'finished':0" + "}";
                }
            }
            json += "]}";
            ViewBag.IDList = String.Join(",", practiceDataIDList);
            return View("Practice", null, json);
        }


        public ActionResult FinishPracticing(string IDString, string goodString, string okayString, string badString)
        {
            List<int> IDList = IDString.Split(',').Select(id => Int32.Parse(id)).ToList();
            List<int> GoodList = new List<int>();
            List<int> OkayList = new List<int>();
            List<int> BadList = new List<int>();

            if (goodString != "_Good" && goodString.Contains(','))
            {
                GoodList = goodString.Split(',').Select(id => Int32.Parse(id)).ToList();
            }
            else if (goodString != "_Good")
            {
                GoodList.Add(Int32.Parse(goodString));
            }
            if (okayString != "_Okay" && okayString.Contains(','))
            {
                OkayList = okayString.Split(',').Select(id => Int32.Parse(id)).ToList();
            }
            else if (okayString != "_Okay")
            {
                OkayList.Add(Int32.Parse(okayString));
            }
            if (badString != "_Bad" && badString.Contains(','))
            {
                BadList = badString.Split(',').Select(id => Int32.Parse(id)).ToList();
            }
            else if (badString != "_Bad")
            {
                BadList.Add(Int32.Parse(badString));
            }
            Kanas_PracticeController KPController = new Kanas_PracticeController();

            List<Kana_Practice> kana_PracticeList = db.Kana_Practice.Where(kp => IDList.Contains(kp.ID)).ToList();

            foreach (var kanaPractice in kana_PracticeList)
            {
                if (kanaPractice.RepeatInterval <= db.RepeatInterval.Count() + 1)
                {
                    if (GoodList.Contains(kanaPractice.KanaID))
                    {
                        kanaPractice.RepeatInterval += 1;
                        double interval = db.RepeatInterval.Where(ri => ri.ID == kanaPractice.RepeatInterval).Select(ri => ri.Interval).First();
                        kanaPractice.LastPracticed = DateTime.Now.AddDays(interval);
                        //TODO fix multiple instances of EntityChanger
                        db.SaveChanges();
                        //VPController.Edit(vocabularyPractice);
                    }
                    if (OkayList.Contains(kanaPractice.KanaID))
                    {
                        double interval = db.RepeatInterval.Where(ri => ri.ID == kanaPractice.RepeatInterval).Select(ri => ri.Interval).First();
                        kanaPractice.LastPracticed = DateTime.Now.AddDays(interval);
                        //TODO fix multiple instances of EntityChanger
                        db.SaveChanges();
                        //VPController.Edit(vocabularyPractice);
                    }
                    if (BadList.Contains(kanaPractice.KanaID))
                    {
                        if (kanaPractice.RepeatInterval > 1)
                        {
                            kanaPractice.RepeatInterval -= 1;
                        }
                        double interval = db.RepeatInterval.Where(ri => ri.ID == kanaPractice.RepeatInterval).Select(ri => ri.Interval).First();
                        kanaPractice.LastPracticed = DateTime.Now.AddDays(interval);
                        //TODO fix multiple instances of EntityChanger
                        db.SaveChanges();
                        //VPController.Edit(vocabularyPractice);
                    }
                }
            }
            return RedirectToAction("Index");
        }
    }
}
