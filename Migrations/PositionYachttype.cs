using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Migrations
{
    [Table("position_yachttype")]
    public partial class PositionYachttype
    {
        [Key]
        [Column("positionid")]
        public int Positionid { get; set; }
        [Key]
        [Column("yachttypeid")]
        public int Yachttypeid { get; set; }

        [ForeignKey(nameof(Positionid))]
        [InverseProperty("PositionYachttype")]
        public virtual Position Position { get; set; }
        [ForeignKey(nameof(Yachttypeid))]
        [InverseProperty("PositionYachttype")]
        public virtual Yachttype Yachttype { get; set; }
    }
}
