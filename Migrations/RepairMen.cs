using System;
using System.Collections.Generic;

namespace Project.Migrations
{
    public partial class RepairMen
    {
        public int Repairid { get; set; }
        public int Staffid { get; set; }

        public virtual Repair Repair { get; set; }
        public virtual StaffPosition Staff { get; set; }
    }
}
