using Bengo.DAL;
using Bengo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace Bengo.Controllers
{
    public class HomeController : Controller
    {
        private BengoContext db = new BengoContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Design()
        {
            return View();
        }

        public ActionResult FinishLearning(string IDList)
        {
            UserDatasController UDController = new UserDatasController();
            Vocabulary_PracticeController VPController = new Vocabulary_PracticeController();
            string userName = User.Identity.GetUserName();
            //var userID =(from user in db.UserData
            //                where user.UserName == userName
            //                select user.ID).First();
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
            foreach(var vocabID in IDtoList)
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

        [Authorize]
        public ActionResult Test()
        {           
            string userName = User.Identity.GetUserName();
            //var UserData = from user in db.UserData
            //                  where user.UserName == userName
            //                  select user.VocabularyList;

            var UserData = db.UserData.Where(ud => ud.UserName == userName).
                                    Select(ud => ud.VocabularyList).First();

            List<Vocabulary> result=new List<Vocabulary>();
            if (UserData != null)
            {
                List<string> UserVocabList = UserData.Split(',').ToList();
            

                if (UserVocabList.Count>0)
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

            List<Test> test = new List<Test>();
            foreach (var vocab in result)
            {
                List<string> answers = (from word in db.Vocabulary
                               where word.Meaning != vocab.Meaning
                               select word.Meaning).ToList();

                test.Add(new Test(vocab.ID, vocab.VocabularyUnit, vocab.Meaning, answers[0], answers[1], answers[2]));
            }
            String json = "{'items':[";
            String IDList="";
            for(int i=0; i<test.Count; i++)
            {
                if (i < test.Count - 1)
                {
                    json += "{'id':'" + test[i].Id + "','word':'" + test[i].Word + "','meaning':'" + test[i].Meaning + "','option1':'" + test[i].Ans1 + "','option2':'" + test[i].Ans2 + "','option3':'" + test[i].Ans3 + "','score':0" + "},";
                    IDList += test[i].Id+",";
                }
                else if (i == test.Count - 1)
                {
                    json += "{'id':'" + test[i].Id + "','word':'" + test[i].Word + "','meaning':'" + test[i].Meaning + "','option1':'" + test[i].Ans1 + "','option2':'" + test[i].Ans2 + "','option3':'" + test[i].Ans3 + "','score':0"+"}";
                    IDList += test[i].Id;
                }
            }
            json += "]}";
            ViewBag.IDList = IDList;
            return View("Test",null,json);
        }
    }
}