using System;
using System.Collections.Generic;

namespace Project.Migrations
{
    public partial class Position
    {
        public Position()
        {
            PositionEquivalentPosition = new HashSet<PositionEquivalent>();
            PositionEquivalentPositionequivalent = new HashSet<PositionEquivalent>();
            PositionYachttype = new HashSet<PositionYachttype>();
            StaffPosition = new HashSet<StaffPosition>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Salary { get; set; }

        public virtual ICollection<PositionEquivalent> PositionEquivalentPosition { get; set; }
        public virtual ICollection<PositionEquivalent> PositionEquivalentPositionequivalent { get; set; }
        public virtual ICollection<PositionYachttype> PositionYachttype { get; set; }
        public virtual ICollection<StaffPosition> StaffPosition { get; set; }
    }
}
