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
            PositionYachttype = new HashSet<PositionYachttype>();
            StaffPosition = new HashSet<StaffPosition>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("name", TypeName = "character varying")]
        public string Name { get; set; }
        [Column("crewposition")]
        public bool Crewposition { get; set; }
        [Column("salary", TypeName = "numeric")]
        public decimal Salary { get; set; }

        [InverseProperty("Position")]
        public virtual ICollection<PositionYachttype> PositionYachttype { get; set; }
        [InverseProperty("Position")]
        public virtual ICollection<StaffPosition> StaffPosition { get; set; }
    }
}
