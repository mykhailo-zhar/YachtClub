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
        public int Count { get; set; } = 1;
        [Column("material")]
        public int Material { get; set; }
        [Column("staffid")]
        public int Staffid { get; set; }
        [Column("repairid")]
        public int Repairid { get; set; }
        [Column("startdate")]
        public DateTime Startdate { get; set; }
        [Column("enddate")]
        public DateTime? Enddate { get; set; }
        [Column("duration")]
        public DateTime Duration { get; set; } = DateTime.Now;
        [Column("description")]
        public string Description{ get; set; }
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
