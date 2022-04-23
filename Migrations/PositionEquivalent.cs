using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Migrations
{
    [Table("position_equivalent")]
    public partial class PositionEquivalent
    {
        [Key]
        [Column("positionid")]
        public int Positionid { get; set; }
        [Key]
        [Column("positionequivalentid")]
        public int Positionequivalentid { get; set; }

        [ForeignKey(nameof(Positionid))]
        [InverseProperty("PositionEquivalentPosition")]
        public virtual Position Position { get; set; }
        [ForeignKey(nameof(Positionequivalentid))]
        [InverseProperty("PositionEquivalentPositionequivalent")]
        public virtual Position Positionequivalent { get; set; }
    }
}
