using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Migrations
{
    [Table("yachtleasetype")]
    public partial class Yachtleasetype
    {
        public Yachtleasetype()
        {
            Yachtlease = new HashSet<Yachtlease>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("name", TypeName = "character varying")]
        public string Name { get; set; }
        [Column("price", TypeName = "numeric")]
        public decimal Price { get; set; }
        [Column("staffonly")]
        public bool Staffonly { get; set; }
        [Column("description")]
        public string Description { get; set; }

        [NotMapped]
        public int? Count { get; set; }

        [InverseProperty("Yachtleasetype")]
        public virtual ICollection<Yachtlease> Yachtlease { get; set; }
    }
}
