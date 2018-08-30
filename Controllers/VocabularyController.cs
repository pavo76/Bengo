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

            string userName = User.Identity.GetUserName();
            int duePracticeCount = db.Vocabulary_Practice.Where(vp => vp.LastPracticed <= DateTime.Now && vp.UserName == userName).Count();

            ViewBag.DuePracticeCount = duePracticeCount;
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


        // POST: LEarn
        [HttpPost]
        public ActionResult Learn(FormCollection formData)
        {
            string userName = User.Identity.GetUserName();

            var UserVocabData = db.UserData.Where(ud => ud.UserName == userName).
                                    Select(ud => ud.VocabularyList).First();
            var UserKanjiData = db.UserData.Where(ud => ud.UserName == userName).
                                    Select(ud => ud.KanjiList).First();
            var UserKanaData = db.UserData.Where(ud => ud.UserName == userName).
                                    Select(ud => ud.KanaList).First();
            int GoalID = db.UserData.Where(ud => ud.UserName == userName).
                                    Select(ud => ud.GoalID).First();
            int itemNumber = Int32.Parse(formData.GetValues("LearnItemNumber")[0]);
            string learnForGoal = formData.GetValues("LearnFor")[0];

            List<Vocabulary> result = new List<Vocabulary>();
            if (UserVocabData != null)
            {
                List<string> UserVocabList = UserVocabData.Split(',').ToList();


                if (UserVocabList.Count > 0)
                {
                    if (learnForGoal=="Default")
                    {
                        result = (from vocab in db.Vocabulary
                                  where (!UserVocabList.Contains(vocab.ID.ToString()))
                                  select vocab).Take(itemNumber).ToList();
                    }
                    else if (learnForGoal == "Goal" && GoalID!=0)
                    {
                        List<string> GoalVocabList = db.Text.Select(text => text.World_List).First().Split(',').ToList();
                        result = (from vocab in db.Vocabulary
                                  where (!UserVocabList.Contains(vocab.ID.ToString()) && GoalVocabList.Contains(vocab.ID.ToString()))
                                  select vocab).Take(itemNumber).ToList();
                    }
                }
            }
            else
            {
                if (learnForGoal == "Default")
                {
                    result = (from vocab in db.Vocabulary
                                select vocab).Take(itemNumber).ToList();
                }
                else if(learnForGoal == "Goal" && GoalID!=0)
                {
                    List<string> GoalVocabList = db.Text.Select(text => text.World_List).First().Split(',').ToList();
                    result = (from vocab in db.Vocabulary
                                where (GoalVocabList.Contains(vocab.ID.ToString()))
                                select vocab).Take(itemNumber).ToList();
                }
            }
            
            if (result.Count > 0)
            {
                List<LearnViewModel> model = new List<LearnViewModel>();
                string ExplanationList = "";
                foreach (var vocab in result)
                {
                    List<string> potentialAnswers = (from word in db.Vocabulary
                                                     where word.Meaning != vocab.Meaning
                                                     select word.Meaning).ToList();
                    List<string> answers = new List<string>();
                    for (var i = 0; i < 3; i++)
                    {
                        var randomIndex = new Random().Next(0, potentialAnswers.Count - 1);
                        answers.Add(potentialAnswers[randomIndex]);
                        potentialAnswers.RemoveAt(randomIndex);
                    }
                    bool userKnowsKanji=false;
                    bool userKnowsKana = false;
                    if(UserKanjiData!=null)
                    {
                        var UserKanjiList = UserKanjiData.Split(',').ToList();
                        var VocabKanjiList = vocab.KanjiList.Split(',').ToList();
                        userKnowsKanji = !VocabKanjiList.Except(UserKanjiList).Any();
                    }
                    if (UserKanaData != null)
                    {
                        var UserKanaList = UserKanaData.Split(',').ToList();
                        var VocabKanaList = vocab.KanaList.Split(',').ToList();
                        userKnowsKana = !VocabKanaList.Except(UserKanaList).Any();
                    }
                    if (userKnowsKanji)
                    {
                        model.Add(new LearnViewModel(vocab.ID, vocab.VocabularyUnit, vocab.Meaning, answers[0], answers[1], answers[2]));
                        if(ExplanationList=="")
                        {
                            ExplanationList += $"{vocab.VocabularyUnit.Replace("\\", "")}:{vocab.Meaning.Replace("\\", "")}";
                        }
                        else
                        {
                            ExplanationList += $",{vocab.VocabularyUnit.Replace("\\", "")}:{vocab.Meaning.Replace("\\", "")}";
                        }
                    }
                    else if (userKnowsKana)
                    {
                        model.Add(new LearnViewModel(vocab.ID, vocab.Kana, vocab.Meaning, answers[0], answers[1], answers[2]));
                        if (ExplanationList == "")
                        {
                            ExplanationList += $"{vocab.Kana.Replace("\\", "")}:{vocab.Meaning.Replace("\\", "")}";
                        }
                        else
                        {
                            ExplanationList += $",{vocab.Kana.Replace("\\", "")}:{vocab.Meaning.Replace("\\", "")}";
                        }
                    }
                    else
                    {
                        model.Add(new LearnViewModel(vocab.ID, vocab.Romaji, vocab.Meaning, answers[0], answers[1], answers[2]));
                        if (ExplanationList == "")
                        {
                            ExplanationList += $"{vocab.Romaji.Replace("\\", "")}:{vocab.Meaning.Replace("\\", "")}";
                        }
                        else
                        {
                            ExplanationList += $",{vocab.Romaji.Replace("\\", "")}:{vocab.Meaning.Replace("\\", "")}";
                        }
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
            //Get the list of voabulary practice data IDs
            List<int> practiceDataIDList = db.Vocabulary_Practice.Where(vp => vp.LastPracticed <= DateTime.Now && vp.UserName == userName).Select(vp => vp.ID).ToList();
            //Get the list of vocabularies based on the list above
            List<Vocabulary> vocabList = db.Vocabulary.Where(v => vocabIDList.Contains(v.ID)).ToList();

            //Turn vocabulary list into apropriate JSON
            List<LearnViewModel> model = new List<LearnViewModel>();
            foreach (var vocab in vocabList)
            {
                var UserKanjiData = db.UserData.Where(ud => ud.UserName == userName).
                                    Select(ud => ud.KanjiList).First();
                var UserKanaData = db.UserData.Where(ud => ud.UserName == userName).
                                        Select(ud => ud.KanaList).First();
                List<string> potentialAnswers = (from word in db.Vocabulary
                                                 where word.Meaning != vocab.Meaning
                                                 select word.Meaning).ToList();
                List<string> answers = new List<string>();
                for (var i = 0; i < 3; i++)
                {
                    var randomIndex = new Random().Next(0, potentialAnswers.Count - 1);
                    answers.Add(potentialAnswers[randomIndex]);
                    potentialAnswers.RemoveAt(randomIndex);
                }

                bool userKnowsKanji = false;
                bool userKnowsKana = false;
                //if (UserKanjiData != null)
                //{
                //    var UserKanjiList = UserKanjiData.Split(',').ToList();
                //    var VocabKanjiList = vocab.KanjiList.Split(',').ToList();
                //    userKnowsKanji = !VocabKanjiList.Except(UserKanjiList).Any();
                //}
                //if (UserKanaData != null)
                //{
                //    var UserKanaList = UserKanaData.Split(',').ToList();
                //    var VocabKanaList = vocab.KanaList.Split(',').ToList();
                //    userKnowsKana = !VocabKanaList.Except(UserKanaList).Any();
                //}
                //if (userKnowsKanji)
                //{
                //    model.Add(new LearnViewModel(vocab.ID, vocab.VocabularyUnit, vocab.Meaning, answers[0], answers[1], answers[2]));
                //}
                //else if (userKnowsKana)
                //{
                //    model.Add(new LearnViewModel(vocab.ID, vocab.Kana, vocab.Meaning, answers[0], answers[1], answers[2]));
                //}
                //else
                //{
                //    model.Add(new LearnViewModel(vocab.ID, vocab.Romaji, vocab.Meaning, answers[0], answers[1], answers[2]));
                //}
                model.Add(new LearnViewModel(vocab.ID, vocab.VocabularyUnit, vocab.Meaning, answers[0], answers[1], answers[2]));
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
            List<int> IDList = IDString.Split(',').Select(id=>Int32.Parse(id)).ToList();
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
            if(okayString !="_Okay" && okayString.Contains(','))
            { 
            OkayList = okayString.Split(',').Select(id => Int32.Parse(id)).ToList();
            }
            else if(okayString!= "_Okay")
            {
                OkayList.Add(Int32.Parse(okayString));
            }
            if (badString!="_Bad" && badString.Contains(','))
            { 
            BadList = badString.Split(',').Select(id => Int32.Parse(id)).ToList();
            }
            else if (badString != "_Bad")
            {
                BadList.Add(Int32.Parse(badString));
            }
            Vocabulary_PracticeController VPController = new Vocabulary_PracticeController();

            List<Vocabulary_Practice> vocabulary_PracticeList = db.Vocabulary_Practice.Where(vp => IDList.Contains(vp.ID)).ToList();

            foreach(var vocabularyPractice in vocabulary_PracticeList)
            {
                if(vocabularyPractice.RepeatInterval<=db.RepeatInterval.Count()+1)
                {
                    if (GoodList.Contains(vocabularyPractice.VocabularyID))
                    {
                        vocabularyPractice.RepeatInterval += 1;
                        double interval = db.RepeatInterval.Where(ri => ri.ID == vocabularyPractice.RepeatInterval).Select(ri => ri.Interval).First();
                        vocabularyPractice.LastPracticed = DateTime.Now.AddDays(interval);
                        //TODO fix multiple instances of EntityChanger
                        db.SaveChanges();
                        //VPController.Edit(vocabularyPractice);
                    }
                    if (OkayList.Contains(vocabularyPractice.VocabularyID))
                    {
                        double interval = db.RepeatInterval.Where(ri => ri.ID == vocabularyPractice.RepeatInterval).Select(ri => ri.Interval).First();
                        vocabularyPractice.LastPracticed = DateTime.Now.AddDays(interval);
                        //TODO fix multiple instances of EntityChanger
                        db.SaveChanges();
                        //VPController.Edit(vocabularyPractice);
                    }
                    if (BadList.Contains(vocabularyPractice.VocabularyID))
                    {
                        if (vocabularyPractice.RepeatInterval > 1)
                        {
                            vocabularyPractice.RepeatInterval -= 1;
                        }
                        double interval = db.RepeatInterval.Where(ri => ri.ID == vocabularyPractice.RepeatInterval).Select(ri => ri.Interval).First();
                        vocabularyPractice.LastPracticed = DateTime.Now.AddDays(interval);
                        //TODO fix multiple instances of EntityChanger
                        db.SaveChanges();
                        //VPController.Edit(vocabularyPractice);
                    }
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult AddKanaList()
        {
            List<Vocabulary> VocabularyList = db.Vocabulary.ToList();
            foreach(Vocabulary vocabulary in VocabularyList)
            {
                if (vocabulary.KanaList == null)
                {
                    List<int> KanaIDList = new List<int>();
                    foreach (char character in vocabulary.Kana)
                    {
                        string characterString = new string(character, 1);
                        int KanaID = db.Kana.Where(k => k.Letter == characterString).Select(k => k.ID).FirstOrDefault();
                        if (!KanaIDList.Contains(KanaID) && KanaID != 0)
                        {
                            KanaIDList.Add(KanaID);
                        }
                    }
                    KanaIDList = KanaIDList.OrderBy(k => k).ToList();
                    string KanaIDListString = "";
                    for (int i = 0; i < KanaIDList.Count; i++)
                    {
                        if (i != KanaIDList.Count - 1)
                        {
                            KanaIDListString += KanaIDList.ElementAt(i) + ",";
                        }
                        else
                        {
                            KanaIDListString += KanaIDList.ElementAt(i);
                        }
                    }

                    vocabulary.KanaList = KanaIDListString;

                    this.Edit(vocabulary);
                    db.SaveChanges();
                }
            }
            return View("Index");
        }
    }
}
