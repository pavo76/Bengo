using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bengo.Models
{
    public class Kanji_Practice
    {
        public int ID { get; set; }
        public string UserName{ get; set; }
        public int KanjiID { get; set; }
        public DateTime LastPracticed { get; set; }
        public int RepeatInterval { get; set; }


        public virtual Kanji Kanji { get; set; }
    }
}