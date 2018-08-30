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
    public class KanjiController : Controller
    {
        private BengoContext db = new BengoContext();

        // GET: Kanji
        public ActionResult Index()
        {
            string userName = User.Identity.GetUserName();
            int duePracticeCount = db.Grammar_Practice.Where(kp => kp.LastPracticed <= DateTime.Now && kp.UserName == userName).Count();

            ViewBag.DuePracticeCount = duePracticeCount;
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


        // POST: Kanji/Learn
        [HttpPost]
        public ActionResult Learn(FormCollection formData)
        {
            string userName = User.Identity.GetUserName();

            var UserKanjiData = db.UserData.Where(ud => ud.UserName == userName).
                                    Select(ud => ud.KanjiList).First();
            int GoalID = db.UserData.Where(ud => ud.UserName == userName).
                                    Select(ud => ud.GoalID).First();

            int itemNumber = Int32.Parse(formData.GetValues("LearnItemNumber")[0]);
            string learnForGoal = formData.GetValues("LearnFor")[0];

            List<Kanji> result = new List<Kanji>();
            if (UserKanjiData != null)
            {
                List<string> UserKanjiList = UserKanjiData.Split(',').ToList();


                if (UserKanjiList.Count > 0)
                {
                    if (learnForGoal == "Default")
                    {
                        result = (from kanjis in db.Kanji
                                  where (!UserKanjiList.Contains(kanjis.ID.ToString()))
                                  select kanjis).Take(itemNumber).ToList();
                    }
                    else if (learnForGoal == "Goal" && GoalID != 0)
                    {
                        List<string> GoalVocabList = db.Text.Select(text => text.World_List).First().Split(',').ToList();
                        result = (from kanjis in db.Kanji
                                  where (!UserKanjiList.Contains(kanjis.ID.ToString()) && GoalVocabList.Contains(kanjis.ID.ToString()))
                                  select kanjis).Take(itemNumber).ToList();
                    }
                }
            }
            else
            {
                if (learnForGoal == "Default")
                {
                    result = (from kanjis in db.Kanji
                              select kanjis).Take(itemNumber).ToList();
                }
                else if (learnForGoal == "Goal" && GoalID != 0)
                {
                    List<string> GoalVocabList = db.Text.Select(text => text.World_List).First().Split(',').ToList();
                    result = (from kanjis in db.Kanji
                              where (GoalVocabList.Contains(kanjis.ID.ToString()))
                              select kanjis).Take(itemNumber).ToList();
                }
            }

            if (result.Count > 0)
            {
                List<LearnViewModel> model = new List<LearnViewModel>();
                string ExplanationList = "";
                foreach (var kanji in result)
                {
                    List<Kanji> potentialAnswers = (from kanjis in db.Kanji
                                                     where kanjis.Meaning != kanji.Meaning
                                                     select kanjis).ToList();
                    List<string> answers = new List<string>();
                    for (var i = 0; i < 3; i++)
                    {
                        var randomIndex = new Random().Next(0, potentialAnswers.Count - 1);
                        string answerReading = "";
                        if (potentialAnswers[randomIndex].KunReading != null)
                        {
                            answerReading = potentialAnswers[randomIndex].KunReading.ToUpper();
                        }
                        if (potentialAnswers[randomIndex].OnReading != null)
                        {
                            if(answerReading!="")
                            {
                                answerReading+= ","+ potentialAnswers[randomIndex].OnReading.ToLower();
                            }
                            else
                            {
                                answerReading = potentialAnswers[randomIndex].OnReading.ToLower();
                            }
                        }
                        answerReading = answerReading.Replace(",", ", ");
                            
                        answers.Add(answerReading);
                        potentialAnswers.RemoveAt(randomIndex);
                    }
                    string kanjiReading = "";
                    if(kanji.KunReading!=null)
                    {
                        kanjiReading = kanji.KunReading.ToUpper();
                    }
                    if(kanji.OnReading != null)
                    {
                        if(kanjiReading!="")
                        {
                            kanjiReading += "," + kanji.OnReading.ToLower();
                        }
                        else
                        {
                            kanjiReading =kanji.OnReading.ToLower();
                        }
                    }
                    kanjiReading = kanjiReading.Replace(",", ", ");
                    model.Add(new LearnViewModel(kanji.ID, kanji.KanjiUnit, kanjiReading, answers[0], answers[1], answers[2]));
                    if(ExplanationList=="")
                    {
                        ExplanationList = $"{kanji.KanjiUnit}:{kanji.Meaning}:{kanji.OnReading}: {kanji.KunReading}";
                    }
                    else
                    {
                        ExplanationList += $";{kanji.KanjiUnit}:{kanji.Meaning}:{kanji.OnReading}: {kanji.KunReading}";
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
            Kanji_PracticeController KPController = new Kanji_PracticeController();
            string userName = User.Identity.GetUserName();
            var userID = db.UserData.Where(ud => ud.UserName == userName).
                                    Select(ud => ud.ID).First();
            UserData userData = db.UserData.Find(userID);
            if (userData.KanjiList == "")
            {
                userData.KanjiList += IDList;
            }
            else
            {
                userData.KanjiList += "," + IDList;
            }
            UDController.Edit(userData);

            List<string> IDtoList = IDList.Split(',').ToList();
            foreach (var kanjiId in IDtoList)
            {
                Kanji_Practice kpData = new Kanji_Practice
                {
                    UserName = userName,
                    KanjiID = Int32.Parse(kanjiId),
                    LastPracticed = DateTime.Now,
                    RepeatInterval = 1
                };

                KPController.Create(kpData);
            }
            return RedirectToAction("Index");
        }


        public ActionResult Practice()
        {
            string userName = User.Identity.GetUserName();
            //Get list of kanji IDs due for practice
            List<int> kanjiIDList = db.Kanji_Practice.Where(kp => kp.LastPracticed <= DateTime.Now && kp.UserName == userName).Select(kp => kp.KanjiID).ToList();
            //Get the list of kanji practice data IDs
            List<int> practiceDataIDList = db.Kanji_Practice.Where(kp => kp.LastPracticed <= DateTime.Now && kp.UserName == userName).Select(kp => kp.ID).ToList();
            //Get the list of kanji based on the list above
            List<Kanji> kanjiList = db.Kanji.Where(k => kanjiIDList.Contains(k.ID)).ToList();

            //Turn vocabulary list into apropriate JSON
            List<LearnViewModel> model = new List<LearnViewModel>();
            foreach (var kanji in kanjiList)
            {
                List<Kanji> potentialAnswers = (from character in db.Kanji
                                                 where character.ID != kanji.ID
                                                 select character).ToList();
                List<string> answers = new List<string>();
                for (var i = 0; i < 3; i++)
                {
                    var randomIndex = new Random().Next(0, potentialAnswers.Count - 1);
                    string answerReading = "";
                    if (potentialAnswers[randomIndex].KunReading != null)
                    {
                        answerReading = potentialAnswers[randomIndex].KunReading.ToUpper();
                    }
                    if (potentialAnswers[randomIndex].OnReading != null)
                    {
                        if (answerReading != "")
                        {
                            answerReading += "," + potentialAnswers[randomIndex].OnReading.ToLower();
                        }
                        else
                        {
                            answerReading = potentialAnswers[randomIndex].OnReading.ToLower();
                        }
                    }
                    answerReading = answerReading.Replace(",", ", ");

                    answers.Add(answerReading);
                    potentialAnswers.RemoveAt(randomIndex);
                }
                string kanjiReading = "";
                if (kanji.KunReading != null)
                {
                    kanjiReading = kanji.KunReading.ToUpper();
                }
                if (kanji.OnReading != null)
                {
                    if (kanjiReading != "")
                    {
                        kanjiReading += "," + kanji.OnReading.ToLower();
                    }
                    else
                    {
                        kanjiReading = "" + kanji.OnReading.ToLower();
                    }
                }
                kanjiReading = kanjiReading.Replace(",", ", ");
                model.Add(new LearnViewModel(kanji.ID, kanji.KanjiUnit, kanjiReading, answers[0], answers[1], answers[2]));
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
            Kanji_PracticeController KPController = new Kanji_PracticeController();

            List<Kanji_Practice> kanji_PracticeList = db.Kanji_Practice.Where(kp => IDList.Contains(kp.ID)).ToList();

            foreach (var kanjiPractice in kanji_PracticeList)
            {
                if (kanjiPractice.RepeatInterval <= db.RepeatInterval.Count() + 1)
                {
                    if (GoodList.Contains(kanjiPractice.KanjiID))
                    {
                        kanjiPractice.RepeatInterval += 1;
                        double interval = db.RepeatInterval.Where(ri => ri.ID == kanjiPractice.RepeatInterval).Select(ri => ri.Interval).First();
                        kanjiPractice.LastPracticed = DateTime.Now.AddDays(interval);
                        //TODO fix multiple instances of EntityChanger
                        db.SaveChanges();
                        //VPController.Edit(vocabularyPractice);
                    }
                    if (OkayList.Contains(kanjiPractice.KanjiID))
                    {
                        double interval = db.RepeatInterval.Where(ri => ri.ID == kanjiPractice.RepeatInterval).Select(ri => ri.Interval).First();
                        kanjiPractice.LastPracticed = DateTime.Now.AddDays(interval);
                        //TODO fix multiple instances of EntityChanger
                        db.SaveChanges();
                        //VPController.Edit(vocabularyPractice);
                    }
                    if (BadList.Contains(kanjiPractice.KanjiID))
                    {
                        if (kanjiPractice.RepeatInterval > 1)
                        {
                            kanjiPractice.RepeatInterval -= 1;
                        }
                        double interval = db.RepeatInterval.Where(ri => ri.ID == kanjiPractice.RepeatInterval).Select(ri => ri.Interval).First();
                        kanjiPractice.LastPracticed = DateTime.Now.AddDays(interval);
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
