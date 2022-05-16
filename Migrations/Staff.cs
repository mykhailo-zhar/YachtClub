using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Migrations
{
    [Table("staff")]
    public partial class Staff
    {
        [Column("id")]
        public int? Id { get; set; }
        [Column("name", TypeName = "character varying")]
        public string Name { get; set; }
        [Column("surname", TypeName = "character varying")]
        public string Surname { get; set; }
        [Column("birthdate", TypeName = "timestamp(2) without time zone")]
        public DateTime? Birthdate { get; set; }
        [Column("sex", TypeName = "character varying")]
        public string Sex { get; set; }
        [Column("email", TypeName = "character varying")]
        public string Email { get; set; }
        [Column("phone", TypeName = "character varying")]
        public string Phone { get; set; }
        [Column("staffonly")]
        public bool? Staffonly { get; set; }
        [Column("registrydate", TypeName = "timestamp(2) without time zone")]
        public DateTime? Registrydate { get; set; }
    }
}
