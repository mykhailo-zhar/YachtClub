using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Migrations
{
    [Table("winner")]
    public partial class Winner
    {
        [Key]
        [Column("eventid")]
        public int Eventid { get; set; }
        [Key]
        [Column("yachtid")]
        public int Yachtid { get; set; }
        [Column("place")]
        public int? Place { get; set; }

        [ForeignKey(nameof(Eventid))]
        [InverseProperty("Winner")]
        public virtual Event Event { get; set; }
        [ForeignKey(nameof(Yachtid))]
        [InverseProperty("Winner")]
        public virtual Yacht Yacht { get; set; }
    }
}
