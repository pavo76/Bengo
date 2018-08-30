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
    public class GrammarController : Controller
    {
        private BengoContext db = new BengoContext();

        // GET: Grammar
        public ActionResult Index()
        {

            string userName = User.Identity.GetUserName();
            int duePracticeCount = db.Grammar_Practice.Where(gp => gp.LastPracticed <= DateTime.Now && gp.UserName == userName).Count();

            ViewBag.DuePracticeCount = duePracticeCount;
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

        // POST: LEarn
        [HttpPost]
        public ActionResult Learn(FormCollection formData)
        {
            string userName = User.Identity.GetUserName();

            var UserGrammarData = db.UserData.Where(ud => ud.UserName == userName).
                                    Select(ud => ud.GrammarList).First();
            int GoalID = db.UserData.Where(ud => ud.UserName == userName).
                                    Select(ud => ud.GoalID).First();
            int itemNumber = Int32.Parse(formData.GetValues("LearnItemNumber")[0]);
            string learnForGoal = formData.GetValues("LearnFor")[0];

            List<Grammar> result = new List<Grammar>();
            if (UserGrammarData != null)
            {
                List<string> UserGrammarList = UserGrammarData.Split(',').ToList();


                if (UserGrammarList.Count > 0)
                {
                    if (learnForGoal == "Default")
                    {
                        result = (from unit in db.Grammar
                                  where (!UserGrammarList.Contains(unit.ID.ToString()))
                                  select unit).Take(itemNumber).ToList();
                    }
                    else if (learnForGoal == "Goal" && GoalID != 0)
                    {
                        List<string> GoalGrammarList = db.Text.Select(text => text.Grammar_List).First().Split(',').ToList();
                        result = (from unit in db.Grammar
                                  where (!UserGrammarList.Contains(unit.ID.ToString()) && GoalGrammarList.Contains(unit.ID.ToString()))
                                  select unit).Take(itemNumber).ToList();
                    }
                }
            }
            else
            {
                if (learnForGoal == "Default")
                {
                    result = (from unit in db.Grammar
                              select unit).Take(itemNumber).ToList();
                }
                else if (learnForGoal == "Goal" && GoalID != 0)
                {
                    List<string> GoalGrammarList = db.Text.Select(text => text.Grammar_List).First().Split(',').ToList();
                    result = (from unit in db.Grammar
                              where (GoalGrammarList.Contains(unit.ID.ToString()))
                              select unit).Take(itemNumber).ToList();
                }
            }

            if (result.Count > 0)
            {
                List<List<LearnViewModel>> tempModel = new List<List<LearnViewModel>>();
                string ExplanationList = "";
                var lastUint = result.LastOrDefault();
                String IDList = "";

                foreach (var unit in result)
                {
                    List<GrammarTask> taskList = db.GrammarTask.Where(gt => gt.GrammarID == unit.ID).Take(4).ToList();
                    List<LearnViewModel> unitTaskModels = new List<LearnViewModel>();
                    foreach(GrammarTask task in taskList)
                    {
                        unitTaskModels.Add(new LearnViewModel(unit.ID, task.Question, task.CorrectAnswer, task.WrongAnswer1, task.WrongAnswer2, task.WrongAnswer3));
                    }
                    tempModel.Add(unitTaskModels);


                    
                    if (ExplanationList == "")
                    {
                        ExplanationList += $"{unit.GrammarUnit}:{unit.Explanation}";
                    }
                    else
                    {
                        ExplanationList += $"${unit.GrammarUnit}:{unit.Explanation}";
                    }

                    if(!unit.Equals(lastUint))
                    {
                        IDList += unit.ID + ",";
                    }
                    else
                    {
                        IDList += unit.ID;
                    }

                }

                //Create model with proper task order which alternates between grammar units
                List<LearnViewModel> model = new List<LearnViewModel>();
                for(int i=0;i<4;i++)
                {
                    foreach(var unitTaskList in tempModel)
                    {
                        model.Add(unitTaskList.ElementAt(i));
                    }
                }

                String json = "{'items':[";
                for (int i = 0; i < model.Count; i++)
                {
                    if (i < model.Count - 1)
                    {
                        json += "{'id':'" + model[i].Id + "','word':'" + model[i].Item + "','meaning':'" + model[i].Meaning + "','option1':'" + model[i].Ans1 + "','option2':'" + model[i].Ans2 + "','option3':'" + model[i].Ans3 + "','score':0" + "},";
                    }
                    else if (i == model.Count - 1)
                    {
                        json += "{'id':'" + model[i].Id + "','word':'" + model[i].Item + "','meaning':'" + model[i].Meaning + "','option1':'" + model[i].Ans1 + "','option2':'" + model[i].Ans2 + "','option3':'" + model[i].Ans3 + "','score':0" + "}";
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
            Grammar_PracticeController GPController = new Grammar_PracticeController();
            string userName = User.Identity.GetUserName();
            var userID = db.UserData.Where(ud => ud.UserName == userName).
                                    Select(ud => ud.ID).First();
            UserData userData = db.UserData.Find(userID);
            if (userData.GrammarList == "")
            {
                userData.GrammarList += IDList;
            }
            else
            {
                userData.GrammarList += "," + IDList;
            }
            UDController.Edit(userData);

            List<string> IDtoList = IDList.Split(',').ToList();
            foreach (var grammarId in IDtoList)
            {
                Grammar_Practice gpData = new Grammar_Practice
                {
                    UserName = userName,
                    GrammarID = Int32.Parse(grammarId),
                    LastPracticed = DateTime.Now.AddDays(1),
                    RepeatInterval = 1
                };

                GPController.Create(gpData);
            }
            return RedirectToAction("Index");
        }

        public ActionResult Practice()
        {
            string userName = User.Identity.GetUserName();
            //Get list of Grammar IDs due for practice
            List<int> grammarIDList = db.Grammar_Practice.Where(gp => gp.LastPracticed <= DateTime.Now && gp.UserName == userName).Select(gp => gp.GrammarID).ToList();
            //Get the list of Grammar practice data IDs
            List<int> practiceDataIDList = db.Grammar_Practice.Where(gp => gp.LastPracticed <= DateTime.Now && gp.UserName == userName).Select(kp => kp.ID).ToList();
            //Get the list of vocabularies based on the list above
            List<Grammar> grammarList = db.Grammar.Where(g => grammarIDList.Contains(g.ID)).ToList();

            //Turn vocabulary list into apropriate JSON
            List<LearnViewModel> model = new List<LearnViewModel>();
            foreach (var grammar in grammarList)
            {
                int index = new Random().Next(0, 4);
                GrammarTask task = db.GrammarTask.Where(gt => gt.GrammarID == grammar.ID).ToList().ElementAt(index);
                model.Add(new LearnViewModel(grammar.ID, task.Question, task.CorrectAnswer, task.WrongAnswer1, task.WrongAnswer2, task.WrongAnswer3));
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
            Grammar_PracticeController KPController = new Grammar_PracticeController();

            List<Grammar_Practice> grammar_PracticeList = db.Grammar_Practice.Where(gp => IDList.Contains(gp.ID)).ToList();

            foreach (var grammarPractice in grammar_PracticeList)
            {
                if (grammarPractice.RepeatInterval <= db.RepeatInterval.Count() + 1)
                {
                    if (GoodList.Contains(grammarPractice.GrammarID))
                    {
                        grammarPractice.RepeatInterval += 1;
                        double interval = db.RepeatInterval.Where(ri => ri.ID == grammarPractice.RepeatInterval).Select(ri => ri.Interval).First();
                        grammarPractice.LastPracticed = DateTime.Now.AddDays(interval);
                        //TODO fix multiple instances of EntityChanger
                        db.SaveChanges();
                        //VPController.Edit(vocabularyPractice);
                    }
                    if (OkayList.Contains(grammarPractice.GrammarID))
                    {
                        double interval = db.RepeatInterval.Where(ri => ri.ID == grammarPractice.RepeatInterval).Select(ri => ri.Interval).First();
                        grammarPractice.LastPracticed = DateTime.Now.AddDays(interval);
                        //TODO fix multiple instances of EntityChanger
                        db.SaveChanges();
                        //VPController.Edit(vocabularyPractice);
                    }
                    if (BadList.Contains(grammarPractice.GrammarID))
                    {
                        if (grammarPractice.RepeatInterval > 1)
                        {
                            grammarPractice.RepeatInterval -= 1;
                        }
                        double interval = db.RepeatInterval.Where(ri => ri.ID == grammarPractice.RepeatInterval).Select(ri => ri.Interval).First();
                        grammarPractice.LastPracticed = DateTime.Now.AddDays(interval);
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
