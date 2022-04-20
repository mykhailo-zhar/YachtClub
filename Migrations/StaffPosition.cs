using System;
using System.Collections.Generic;

namespace Project.Migrations
{
    public partial class StaffPosition
    {
        public StaffPosition()
        {
            Extradationrequest = new HashSet<Extradationrequest>();
            RepairMen = new HashSet<RepairMen>();
            ReviewCaptain = new HashSet<ReviewCaptain>();
            YachtCrew = new HashSet<YachtCrew>();
            Yachttest = new HashSet<Yachttest>();
        }

        public int Id { get; set; }
        public int Staffid { get; set; }
        public int Positionid { get; set; }
        public DateTime Startdate { get; set; }
        public DateTime? Enddate { get; set; }
        public string Description { get; set; }

        public virtual Position Position { get; set; }
        public virtual Staff Staff { get; set; }
        public virtual ICollection<Extradationrequest> Extradationrequest { get; set; }
        public virtual ICollection<RepairMen> RepairMen { get; set; }
        public virtual ICollection<ReviewCaptain> ReviewCaptain { get; set; }
        public virtual ICollection<YachtCrew> YachtCrew { get; set; }
        public virtual ICollection<Yachttest> Yachttest { get; set; }
    }
}
