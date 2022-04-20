using System;
using System.Collections.Generic;

namespace Project.Migrations
{
    public partial class YachtCrew
    {
        public YachtCrew()
        {
            Contract = new HashSet<Contract>();
        }

        public int Id { get; set; }
        public int Yachtid { get; set; }
        public int Crewid { get; set; }
        public DateTime Startdate { get; set; }
        public DateTime? Enddate { get; set; }
        public string Description { get; set; }

        public virtual StaffPosition Crew { get; set; }
        public virtual Yacht Yacht { get; set; }
        public virtual ICollection<Contract> Contract { get; set; }
    }
}
