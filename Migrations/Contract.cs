using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Migrations
{
    [Table("contract")]
    public partial class Contract
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("clientid")]
        public int Clientid { get; set; }
        [Column("contracttypeid")]
        public int Contracttypeid { get; set; }
        [Column("captaininyachtid")]
        public int Captaininyachtid { get; set; }
        [Column("startdate", TypeName = "timestamp(2) without time zone")]
        public DateTime Startdate { get; set; }
        [Column("enddate", TypeName = "timestamp(2) without time zone")]
        public DateTime? Enddate { get; set; }
        [Column("duration", TypeName = "timestamp(2) without time zone")]
        public DateTime Duration { get; set; }
        [Column("specials")]
        public string Specials { get; set; }
        [Column("paid")]
        public bool Paid { get; set; }
        [Column("averallprice", TypeName = "numeric")]
        public decimal? Averallprice { get; set; }

        [ForeignKey(nameof(Captaininyachtid))]
        [InverseProperty(nameof(YachtCrew.Contract))]
        public virtual YachtCrew Captaininyacht { get; set; }
        [ForeignKey(nameof(Clientid))]
        [InverseProperty(nameof(Person.Contract))]
        public virtual Person Client { get; set; }
        [ForeignKey(nameof(Contracttypeid))]
        [InverseProperty("Contract")]
        public virtual Contracttype Contracttype { get; set; }
    }
}
