using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Migrations
{
    [Table("repair_men")]
    public partial class RepairMen
    {
        [Key]
        [Column("repairid")]
        public int Repairid { get; set; }
        [Key]
        [Column("staffid")]
        public int Staffid { get; set; }

        [ForeignKey(nameof(Repairid))]
        [InverseProperty("RepairMen")]
        public virtual Repair Repair { get; set; }
        [ForeignKey(nameof(Staffid))]
        [InverseProperty(nameof(StaffPosition.RepairMen))]
        public virtual StaffPosition Staff { get; set; }
    }
}
