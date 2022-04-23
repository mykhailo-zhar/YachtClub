using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Migrations
{
    [Table("yachtlease")]
    public partial class Yachtlease
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("startdate", TypeName = "date")]
        public DateTime Startdate { get; set; }
        [Column("enddate", TypeName = "date")]
        public DateTime? Enddate { get; set; }
        [Column("duration", TypeName = "date")]
        public DateTime Duration { get; set; }
        [Column("overallprice", TypeName = "numeric")]
        public decimal Overallprice { get; set; }
        [Column("yachtid")]
        public int Yachtid { get; set; }
        [Column("yachtleasetypeid")]
        public int Yachtleasetypeid { get; set; }

        [ForeignKey(nameof(Yachtid))]
        [InverseProperty("Yachtlease")]
        public virtual Yacht Yacht { get; set; }
        [ForeignKey(nameof(Yachtleasetypeid))]
        [InverseProperty("Yachtlease")]
        public virtual Yachtleasetype Yachtleasetype { get; set; }
    }
}
