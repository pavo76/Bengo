using System;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bengo.Models
{
    public class UserData
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public string VocabularyList { get; set; }
        public string KanjiList { get; set; }
        public string GrammarList { get; set; }
        public string KanaList { get; set; }
        public bool KanaDone { get; set; }

    }
}