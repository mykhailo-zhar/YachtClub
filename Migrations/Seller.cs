using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Migrations
{
    [Table("seller")]
    public partial class Seller
    {
        public Seller()
        {
            Materiallease = new HashSet<Materiallease>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("name", TypeName = "character varying")]
        public string Name { get; set; }
        [Column("description")]
        public string Description { get; set; }

        [InverseProperty("SellerNavigation")]
        public virtual ICollection<Materiallease> Materiallease { get; set; }
    }
}
