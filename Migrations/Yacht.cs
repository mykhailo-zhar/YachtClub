using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Migrations
{
    [Table("yacht")]
    public partial class Yacht
    {
        public Yacht()
        {
            Repair = new HashSet<Repair>();
            ReviewYacht = new HashSet<ReviewYacht>();
            Winner = new HashSet<Winner>();
            YachtCrew = new HashSet<YachtCrew>();
            Yachtlease = new HashSet<Yachtlease>();
            Yachttest = new HashSet<Yachttest>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("name", TypeName = "character varying")]
        public string Name { get; set; }
        [Required]
        [Column("status", TypeName = "character varying")]
        public string Status { get; set; }
        [Required]
        [Column("rentable")]
        public bool? Rentable { get; set; }
        [Column("typeid")]
        public int Typeid { get; set; }
        [Column("yachtownerid")]
        public int Yachtownerid { get; set; }

        [ForeignKey(nameof(Typeid))]
        [InverseProperty(nameof(Yachttype.Yacht))]
        public virtual Yachttype Type { get; set; }
        [ForeignKey(nameof(Yachtownerid))]
        [InverseProperty(nameof(Client.Yacht))]
        public virtual Client Yachtowner { get; set; }
        [InverseProperty("Yacht")]
        public virtual ICollection<Repair> Repair { get; set; }
        [InverseProperty("Yacht")]
        public virtual ICollection<ReviewYacht> ReviewYacht { get; set; }
        [InverseProperty("Yacht")]
        public virtual ICollection<Winner> Winner { get; set; }
        [InverseProperty("Yacht")]
        public virtual ICollection<YachtCrew> YachtCrew { get; set; }
        [InverseProperty("Yacht")]
        public virtual ICollection<Yachtlease> Yachtlease { get; set; }
        [InverseProperty("Yacht")]
        public virtual ICollection<Yachttest> Yachttest { get; set; }
    }
}
