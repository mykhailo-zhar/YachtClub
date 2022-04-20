using System;
using System.Collections.Generic;

namespace Project.Migrations
{
    public partial class Extradationrequest
    {
        public int Id { get; set; }
        public int Count { get; set; }
        public int Material { get; set; }
        public int Staffid { get; set; }
        public int Repairid { get; set; }
        public DateTime Startdate { get; set; }
        public DateTime? Enddate { get; set; }
        public DateTime Duration { get; set; }
        public string Status { get; set; }

        public virtual Material MaterialNavigation { get; set; }
        public virtual Repair Repair { get; set; }
        public virtual StaffPosition Staff { get; set; }
    }
}
