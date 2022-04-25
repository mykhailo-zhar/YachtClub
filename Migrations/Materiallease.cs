using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Migrations
{
    [Table("materiallease")]
    public partial class Materiallease
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("material")]
        public int Material { get; set; }
        [Column("seller")]
        public int Seller { get; set; }
        [Column("priceperunit", TypeName = "numeric")]
        public decimal Priceperunit { get; set; }
        [Column("count")]
        public int Count { get; set; }
        [Column("overallprice", TypeName = "numeric")]
        public decimal Overallprice { get; set; }
        [Column("startdate")]
        public DateTime Startdate { get; set; }
        [Column("deliverydate")]
        public DateTime? Deliverydate { get; set; }

        [ForeignKey(nameof(Material))]
        [InverseProperty("Materiallease")]
        public virtual Material MaterialNavigation { get; set; }
        [ForeignKey(nameof(Seller))]
        [InverseProperty("Materiallease")]
        public virtual Seller SellerNavigation { get; set; }
    }
}
