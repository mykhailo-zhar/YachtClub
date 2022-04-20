using System;
using System.Collections.Generic;

namespace Project.Migrations
{
    public partial class ReviewCaptain
    {
        public int Reviewid { get; set; }
        public int Captainid { get; set; }

        public virtual StaffPosition Captain { get; set; }
        public virtual Review Review { get; set; }
    }
}
