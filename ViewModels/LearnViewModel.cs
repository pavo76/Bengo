using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bengo.Models
{
    public class LearnViewModel
    {
        public int Id { get; set; }
        public string Item { get; set; }
        public string Meaning { get; set; }
        public string Ans1 { get; set; }
        public string Ans2 { get; set; }
        public string Ans3 { get; set; }

        public LearnViewModel(int id, string word, string meaning, string ans1, string ans2, string ans3)
        {
            this.Id=id;
            this.Item=word;
            this.Meaning= meaning;
            this.Ans1= ans1;
            this.Ans2= ans2;
            this.Ans3= ans3;
        }

    }
}