using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bengo.Models
{
    public class GrammarQuestion
    {
        public enum QuestionType
        {
            Sort,
            Choice
        }

        public int ID { get; set; }
        public int GrammarID { get; set; }
        public QuestionType Type { get; set; }
        public string Question { get; set; }
        public string Answer1 { get; set; }
        public string Answer2 { get; set; }
        public string Answer3 { get; set; }
        public string Answer4 { get; set; }
    }
}