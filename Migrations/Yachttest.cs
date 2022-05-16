using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Migrations
{
    [Table("yachttest")]
    public partial class Yachttest
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("date", TypeName = "timestamp(2) without time zone")]
        public DateTime Date { get; set; }
        [Required]
        [Column("results")]
        public string Results { get; set; }
        [Column("yachtid")]
        public int Yachtid { get; set; }
        [Column("staffid")]
        public int Staffid { get; set; }

        [ForeignKey(nameof(Staffid))]
        [InverseProperty(nameof(StaffPosition.Yachttest))]
        public virtual StaffPosition Staff { get; set; }
        [ForeignKey(nameof(Yachtid))]
        [InverseProperty("Yachttest")]
        public virtual Yacht Yacht { get; set; }
    }
}
