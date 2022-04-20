using System;
using System.Collections.Generic;

namespace Project.Migrations
{
    public partial class ReviewYacht
    {
        public int Reviewid { get; set; }
        public int Yachtid { get; set; }

        public virtual Review Review { get; set; }
        public virtual Yacht Yacht { get; set; }
    }
}
