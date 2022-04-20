using System;
using System.Collections.Generic;

namespace Project.Migrations
{
    public partial class Repair
    {
        public Repair()
        {
            Extradationrequest = new HashSet<Extradationrequest>();
            RepairMen = new HashSet<RepairMen>();
        }

        public int Id { get; set; }
        public DateTime Startdate { get; set; }
        public DateTime? Enddate { get; set; }
        public DateTime Duration { get; set; }
        public string Status { get; set; }
        public int Personnel { get; set; }
        public int Yachtid { get; set; }

        public virtual Yacht Yacht { get; set; }
        public virtual ICollection<Extradationrequest> Extradationrequest { get; set; }
        public virtual ICollection<RepairMen> RepairMen { get; set; }
    }
}
