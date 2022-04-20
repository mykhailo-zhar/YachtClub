using System;
using System.Collections.Generic;

namespace Project.Migrations
{
    public partial class Yachttype
    {
        public Yachttype()
        {
            PositionYachttype = new HashSet<PositionYachttype>();
            Yacht = new HashSet<Yacht>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Frame { get; set; }
        public string Goal { get; set; }
        public string Class { get; set; }
        public int Crewcapacity { get; set; }
        public int Capacity { get; set; }
        public int Sails { get; set; }

        public virtual ICollection<PositionYachttype> PositionYachttype { get; set; }
        public virtual ICollection<Yacht> Yacht { get; set; }
    }
}
