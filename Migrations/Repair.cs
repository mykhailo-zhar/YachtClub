using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Migrations
{
    [Table("repair")]
    public partial class Repair
    {
        public Repair()
        {
            Extradationrequest = new HashSet<Extradationrequest>();
            RepairMen = new HashSet<RepairMen>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("startdate")]
        public DateTime Startdate { get; set; }
        [Column("enddate")]
        public DateTime? Enddate { get; set; }
        [Column("duration")]
        public DateTime Duration { get; set; }
        [Required]
        [Column("status", TypeName = "character varying")]
        public string Status { get; set; }
        [Column("personnel")]
        public int Personnel { get; set; }
        [Required]
        [Column("description")]
        public string Description { get; set; }
        [Column("yachtid")]
        public int Yachtid { get; set; }

        [ForeignKey(nameof(Yachtid))]
        [InverseProperty("Repair")]
        public virtual Yacht Yacht { get; set; }
        [InverseProperty("Repair")]
        public virtual ICollection<Extradationrequest> Extradationrequest { get; set; }
        [InverseProperty("Repair")]
        public virtual ICollection<RepairMen> RepairMen { get; set; }
    }
}
