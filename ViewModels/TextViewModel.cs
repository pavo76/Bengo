using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bengo.ViewModels
{
    public class TextViewModel
    {
        public HtmlString Text { get; set; }
        public string Title { get; set; }
        public double VocabularyPercentage { get; set; }
        public double KanjiPercentage { get; set; }
        public double GrammarPercentage { get; set; }
        public double KanaPercentage { get; set; }
        public bool ForLearningKana { get; set; }
        public int GoalID { get; set; }
    }
}