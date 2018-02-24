using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bengo.Models
{
    public class KanaPractice
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int KanaID { get; set; }
        public DateTime LastPracticed { get; set; }
        public int RepeatInterval { get; set; }


        public virtual Kana Kana { get; set; }
        public virtual UserData UserData { get; set; }
    }
}