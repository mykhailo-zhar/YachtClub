using System;
using System.Collections.Generic;

namespace Project.Migrations
{
    public partial class ReviewContract
    {
        public int Reviewid { get; set; }
        public int Contractid { get; set; }

        public virtual Contract Contract { get; set; }
        public virtual Review Review { get; set; }
    }
}
