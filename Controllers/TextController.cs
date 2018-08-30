using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bengo.DAL;
using Bengo.Models;
using Bengo.ViewModels;
using Microsoft.AspNet.Identity;

namespace Bengo.Controllers
{
    [Authorize]
    public class TextController : Controller
    {
        private BengoContext db = new BengoContext();
        // GET: Text
        public ActionResult Index()
        {
            List<Text> textList = db.Text.Select(text=>text).ToList();
            List<TextViewModel> model = new List<TextViewModel>();
            string userName = User.Identity.GetUserName();
            UserData userData = db.UserData.Where(ud => ud.UserName == userName).First();

            foreach(var text in textList)
            {
                TextViewModel modelItem = new TextViewModel();
                modelItem.Text = new HtmlString(text.Script);
                modelItem.Title = text.Title;
                modelItem.ForLearningKana = text.ForLearningKana;
                var textVocabularyList = text.World_List.Split(',');
                var textGrammarList = text.Grammar_List.Split(',');
                var userVocabularyList = userData.VocabularyList.Split(',');
                var userGrammarList = userData.GrammarList.Split(',');
                int knownVocabularyCount = 0;
                int knownGrammarCount = 0;
                foreach(var word in textVocabularyList)
                {
                    if(userVocabularyList.Contains(word))
                    {
                        knownVocabularyCount += 1;
                    }
                }

                foreach (var grammar in textGrammarList)
                {
                    if (userGrammarList.Contains(grammar))
                    {
                        knownGrammarCount += 1;
                    }
                }

                if(text.ForLearningKana)
                {
                    int knownKanaCount = 0;
                    var textKanaList = text.Kana_List.Split(',');
                    var userKanaList = userData.KanaList.Split(',');
                    foreach (var kana in textKanaList)
                    {
                        if (userKanaList.Contains(kana))
                        {
                            knownKanaCount += 1;
                        }
                    }
                    modelItem.KanaPercentage = (double)knownKanaCount / textKanaList.Count() * 100;
                }

                else
                {
                    int knownKanjiCount = 0;
                    var textKanjiList = text.Kanji_List.Split(',');
                    var userKanjiList = userData.KanjiList.Split(',');
                    foreach (var kanji in textKanjiList)
                    {
                        if (userKanjiList.Contains(kanji))
                        {
                            knownKanjiCount += 1;
                        }
                    }
                    modelItem.KanjiPercentage = (double)knownKanjiCount / textKanjiList.Count() * 100;
                }
                modelItem.VocabularyPercentage = (double)knownVocabularyCount / textVocabularyList.Count()*100;
                modelItem.GrammarPercentage = (double)knownGrammarCount / textGrammarList.Count() * 100;
                modelItem.GoalID = text.ID;
                model.Add(modelItem);
            }
            model=model.OrderByDescending(m => m.GrammarPercentage + m.VocabularyPercentage + m.KanaPercentage+ m.KanjiPercentage).ToList();
            ViewBag.GoalID = userData.GoalID;
            return View(model);
        }

        public ActionResult SetGoal(int goalID)
        {
            UserDatasController userDatasController = new UserDatasController();

            string userName = User.Identity.GetUserName();
            UserData userData = db.UserData.Where(ud => ud.UserName == userName).First();

            userData.GoalID = goalID;

            userDatasController.Edit(userData);
            return RedirectToAction("Index", "Text");
        }

        public ActionResult Text(int textID)
        {
            Text text = db.Text.Find(textID);
            TextViewModel model = new TextViewModel{
                Title = text.Title,
                Text = new HtmlString(text.Script)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title,Script,Word_List,Kanji_List,Grammar_List,Kana_List,ForLearningKana")] Text text)
        {
            if (ModelState.IsValid)
            {
                db.Entry(text).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(text);
        }

        public ActionResult AddKanaList()
        {
            List<Text> TextList = db.Text.ToList();
            foreach (Text text in TextList)
            {
                if (text.ForLearningKana)
                {
                    if (text.Kana_List == null)
                    {
                        List<int> KanaIDList = new List<int>();
                        foreach (char character in text.Script)
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

                        text.Kana_List = KanaIDListString;

                        this.Edit(text);
                        db.SaveChanges();
                    }
                }
            }
            return View("Index", "Home");
        }
    }

}