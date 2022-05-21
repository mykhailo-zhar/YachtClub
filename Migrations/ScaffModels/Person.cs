using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Migrations.ScaffModels
{
    [Table("person")]
    public partial class Person
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("name", TypeName = "character varying")]
        public string Name { get; set; }
        [Required]
        [Column("surname", TypeName = "character varying")]
        public string Surname { get; set; }
        [Column("birthdate", TypeName = "timestamp(2) without time zone")]
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
        [Column("staffonly")]
        public bool Staffonly { get; set; }
        [Column("registrydate", TypeName = "timestamp(2) without time zone")]
        public DateTime Registrydate { get; set; }
    }
}
