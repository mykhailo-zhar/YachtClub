using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Migrations
{
    [Table("materialtype")]
    public partial class Materialtype
    {
        public Materialtype()
        {
            Material = new HashSet<Material>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("name", TypeName = "character varying")]
        public string Name { get; set; }
        [Column("metric", TypeName = "character varying")]
        public string Metric { get; set; }
        [Column("description")]
        public string Description { get; set; }

        [InverseProperty("Type")]
        public virtual ICollection<Material> Material { get; set; }
    }
}
