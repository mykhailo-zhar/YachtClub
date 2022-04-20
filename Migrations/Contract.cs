using System;
using System.Collections.Generic;

namespace Project.Migrations
{
    public partial class Contract
    {
        public Contract()
        {
            Review = new HashSet<Review>();
            ReviewContract = new HashSet<ReviewContract>();
        }

        public int Id { get; set; }
        public int Clientid { get; set; }
        public int Contracttypeid { get; set; }
        public int Yachtwithcrewid { get; set; }
        public DateTime Startdate { get; set; }
        public DateTime? Enddate { get; set; }
        public DateTime Duration { get; set; }
        public string Specials { get; set; }
        public string Status { get; set; }
        public decimal Averallprice { get; set; }

        public virtual Client Client { get; set; }
        public virtual Contracttype Contracttype { get; set; }
        public virtual YachtCrew Yachtwithcrew { get; set; }
        public virtual ICollection<Review> Review { get; set; }
        public virtual ICollection<ReviewContract> ReviewContract { get; set; }
    }
}
