using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Migrations
{
    [Table("staff")]
    public partial class Staff
    {
        public Staff()
        {
            StaffPosition = new HashSet<StaffPosition>();
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
        [Column("hiringdate", TypeName = "date")]
        public DateTime Hiringdate { get; set; }

        [InverseProperty("Staff")]
        public virtual ICollection<StaffPosition> StaffPosition { get; set; }
    }
}
