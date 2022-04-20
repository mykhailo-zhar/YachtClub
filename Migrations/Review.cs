using System;
using System.Collections.Generic;

namespace Project.Migrations
{
    public partial class Review
    {
        public Review()
        {
            ReviewCaptain = new HashSet<ReviewCaptain>();
            ReviewContract = new HashSet<ReviewContract>();
            ReviewYacht = new HashSet<ReviewYacht>();
        }

        public int Id { get; set; }
        public int Clientid { get; set; }
        public int Contractid { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }
        public int Rate { get; set; }

        public virtual Client Client { get; set; }
        public virtual Contract Contract { get; set; }
        public virtual ICollection<ReviewCaptain> ReviewCaptain { get; set; }
        public virtual ICollection<ReviewContract> ReviewContract { get; set; }
        public virtual ICollection<ReviewYacht> ReviewYacht { get; set; }
    }
}
