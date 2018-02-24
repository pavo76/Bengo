using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bengo.Models
{
    public class Kanji
    {
        public int ID { get; set; }
        public string KanjiUnit { get; set; }
        public string Meaning { get; set; }
        public string OnReading { get; set; }
        public string KunReading { get; set; }
        public string SRC { get; set; }

    }
}