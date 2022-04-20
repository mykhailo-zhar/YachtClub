using System;
using System.Collections.Generic;

namespace Project.Migrations
{
    public partial class Winner
    {
        public int Eventid { get; set; }
        public int Yachtid { get; set; }
        public int? Place { get; set; }

        public virtual Event Event { get; set; }
        public virtual Yacht Yacht { get; set; }
    }
}
