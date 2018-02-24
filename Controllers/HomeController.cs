using Bengo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bengo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Test()
        {
            List<Test> test = new List<Test>();
            test.Add(new Test(1, "おはよう", "Good Morning", "Good Day", "Good Night", "Good Evening"));
            test.Add(new Test(2, "こんにちは", "Good Day", "Good Morning", "Good Night", "Good Evening"));
            test.Add(new Test(3, "こんばんは", "Good Evening", "Good Morning", "Good Night", "Good Day"));
            test.Add(new Test(4, "おやすみ", "Good Night", "Good Morning", "Good Night", "Good Evening"));


            String json = "{'id':'1', 'word':'おはよう', 'answer':'Good Morning', 'option1':'Good Day','option2':'Good Night','option3':'Good Evening'}";
            return View("Test",null,json);
        }
    }
}