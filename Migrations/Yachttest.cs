using System;
using System.Collections.Generic;

namespace Project.Migrations
{
    public partial class Yachttest
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Results { get; set; }
        public int Yachtid { get; set; }
        public int Staffid { get; set; }

        public virtual StaffPosition Staff { get; set; }
        public virtual Yacht Yacht { get; set; }
    }
}
