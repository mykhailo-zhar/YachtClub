using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Migrations
{
    [Table("yachttype")]
    public partial class Yachttype
    {
        public Yachttype()
        {
            PositionYachttype = new HashSet<PositionYachttype>();
            Yacht = new HashSet<Yacht>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("name", TypeName = "character varying")]
        public string Name { get; set; }
        [Required]
        [Column("frame", TypeName = "character varying")]
        public string Frame { get; set; }
        [Required]
        [Column("goal", TypeName = "character varying")]
        public string Goal { get; set; }
        [Required]
        [Column("class", TypeName = "character varying")]
        public string Class { get; set; }
        [Column("crewcapacity")]
        public int Crewcapacity { get; set; }
        [Column("capacity")]
        public int Capacity { get; set; }
        [Column("sails")]
        public int Sails { get; set; }
        [Column("description")]
        public string Description { get; set; }

        [InverseProperty("Yachttype")]
        public virtual ICollection<PositionYachttype> PositionYachttype { get; set; }
        [InverseProperty("Type")]
        public virtual ICollection<Yacht> Yacht { get; set; }
    }
}
