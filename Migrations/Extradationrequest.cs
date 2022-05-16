using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Migrations
{
    [Table("extradationrequest")]
    public partial class Extradationrequest
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("count")]
        public int Count { get; set; }
        [Column("material")]
        public int Material { get; set; }
        [Column("staffid")]
        public int Staffid { get; set; }
        [Column("repairid")]
        public int Repairid { get; set; }
        [Column("startdate", TypeName = "timestamp(2) without time zone")]
        public DateTime Startdate { get; set; }
        [Column("enddate", TypeName = "timestamp(2) without time zone")]
        public DateTime? Enddate { get; set; }
        [Column("duration", TypeName = "timestamp(2) without time zone")]
        public DateTime Duration { get; set; }
        [Required]
        [Column("description")]
        public string Description { get; set; }
        [Required]
        [Column("status", TypeName = "character varying")]
        public string Status { get; set; }

        [ForeignKey(nameof(Material))]
        [InverseProperty("Extradationrequest")]
        public virtual Material MaterialNavigation { get; set; }
        [ForeignKey(nameof(Repairid))]
        [InverseProperty("Extradationrequest")]
        public virtual Repair Repair { get; set; }
        [ForeignKey(nameof(Staffid))]
        [InverseProperty(nameof(StaffPosition.Extradationrequest))]
        public virtual StaffPosition Staff { get; set; }
    }
}
