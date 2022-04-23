using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Migrations
{
    [Table("contracttype")]
    public partial class Contracttype
    {
        public Contracttype()
        {
            Contract = new HashSet<Contract>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("name", TypeName = "character varying")]
        public string Name { get; set; }
        [Column("price", TypeName = "numeric")]
        public decimal Price { get; set; }
        [Column("description")]
        public string Description { get; set; }

        [InverseProperty("Contracttype")]
        public virtual ICollection<Contract> Contract { get; set; }
    }
}
