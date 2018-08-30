using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bengo.Models
{
    public class Vocabulary
    {
        public int ID { get; set; }
        public string VocabularyUnit { get; set; }
        public string Kana { get; set; }
        public string Romaji { get; set; }
        public string Meaning { get; set; }
        public string POS { get; set; }
        public int CategoryID { get; set; }
        public string KanjiList { get; set; }
        public string KanaList { get; set; }

        public virtual Category Category { get; set; }
    }
}