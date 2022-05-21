using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Migrations
{
    [Table("account")]
    public class Account
    {

        [Column("id")]
        public int Id { get; set; }
        [Column("login")]
        [Required]
        public string Login { get; set; }
        [Column("password", TypeName = "character varying")]
        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }

        [Column("role")]
        public string? Role { get; set; }

        [Column("userid")]
        public int Userid { get; set; }

        [ForeignKey(nameof(Userid))]
        [InverseProperty(nameof(Person.Account))]
        public virtual Person User { get; set; }

        
        

        [NotMapped]
        public string Position { get; set; } 
        [NotMapped]
        public string AsWho { get; set; }
    }
}
