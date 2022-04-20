using System;
using System.Collections.Generic;

namespace Project.Migrations
{
    public partial class Yachtlease
    {
        public int Id { get; set; }
        public DateTime Startdate { get; set; }
        public DateTime? Enddate { get; set; }
        public DateTime Duration { get; set; }
        public decimal Overallprice { get; set; }
        public int Yachtid { get; set; }
        public int Yachtleasetypeid { get; set; }

        public virtual Yacht Yacht { get; set; }
        public virtual Yachtleasetype Yachtleasetype { get; set; }
    }
}
