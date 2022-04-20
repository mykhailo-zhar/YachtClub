using System;
using System.Collections.Generic;

namespace Project.Migrations
{
    public partial class Materiallease
    {
        public int Id { get; set; }
        public int Material { get; set; }
        public int Seller { get; set; }
        public decimal Priceperunit { get; set; }
        public int Count { get; set; }
        public decimal Overallprice { get; set; }
        public DateTime Startdate { get; set; }
        public DateTime? Deliverydate { get; set; }

        public virtual Material MaterialNavigation { get; set; }
        public virtual Seller SellerNavigation { get; set; }
    }
}
