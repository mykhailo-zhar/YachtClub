using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Migrations.ScaffModels
{
    [Table("account")]
    public partial class Account
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("login")]
        public string Login { get; set; }
        [Required]
        [Column("password", TypeName = "character varying")]
        public string Password { get; set; }
        [Column("userid")]
        public int Userid { get; set; }
    }
}
