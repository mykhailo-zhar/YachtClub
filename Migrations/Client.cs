using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Migrations
{
    [Table("client")]
    public partial class Client
    {
        public Client()
        {
            Contract = new HashSet<Contract>();
            Review = new HashSet<Review>();
            Yacht = new HashSet<Yacht>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("name", TypeName = "character varying")]
        public string Name { get; set; }
        [Required]
        [Column("surname", TypeName = "character varying")]
        public string Surname { get; set; }
        [Required]
        [Column("birthdate", TypeName = "date")]
        public DateTime Birthdate { get; set; }
        [Required]
        [Column("sex", TypeName = "character varying")]
        public string Sex { get; set; }
        [Required]
        [Column("email", TypeName = "character varying")]
        public string Email { get; set; }
        [Required]
        [Column("phone", TypeName = "character varying")]
        public string Phone { get; set; }
        [Column("registrydate", TypeName = "timestamp")]
        public DateTime Registrydate { get; set; }

        [InverseProperty("Client")]
        public virtual ICollection<Contract> Contract { get; set; }
        [InverseProperty("Client")]
        public virtual ICollection<Review> Review { get; set; }
        [InverseProperty("Yachtowner")]
        public virtual ICollection<Yacht> Yacht { get; set; }
    }
}
