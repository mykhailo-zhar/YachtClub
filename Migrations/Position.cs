using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Migrations
{
    [Table("position")]
    public partial class Position
    {
        public Position()
        {
            PositionEquivalentPosition = new HashSet<PositionEquivalent>();
            PositionEquivalentPositionequivalent = new HashSet<PositionEquivalent>();
            PositionYachttype = new HashSet<PositionYachttype>();
            StaffPosition = new HashSet<StaffPosition>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("name", TypeName = "character varying")]
        public string Name { get; set; }
        [Column("salary", TypeName = "numeric")]
        public decimal Salary { get; set; }

        [InverseProperty(nameof(PositionEquivalent.Position))]
        public virtual ICollection<PositionEquivalent> PositionEquivalentPosition { get; set; }
        [InverseProperty(nameof(PositionEquivalent.Positionequivalent))]
        public virtual ICollection<PositionEquivalent> PositionEquivalentPositionequivalent { get; set; }
        [InverseProperty("Position")]
        public virtual ICollection<PositionYachttype> PositionYachttype { get; set; }
        [InverseProperty("Position")]
        public virtual ICollection<StaffPosition> StaffPosition { get; set; }
    }
}
