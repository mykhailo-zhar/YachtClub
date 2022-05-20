using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Migrations
{
    [Table("review")]
    public partial class Review
    {
        public Review()
        {
            ReviewCaptain = new HashSet<ReviewCaptain>();
            ReviewYacht = new HashSet<ReviewYacht>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("clientid")]
        public int Clientid { get; set; }
        [Column("date", TypeName = "timestamp(2) without time zone")]
        public DateTime? Date { get; set; }
        [Column("text")]
        public string Text { get; set; }
        [Required]
        [Column("public")]
        public bool Public { get; set; }
        [Column("userrate")]
        public int Userrate { get; set; }
        [Column("rate")]
        public int Rate { get; set; }

        [ForeignKey(nameof(Clientid))]
        [InverseProperty(nameof(Person.Review))]
        public virtual Person Client { get; set; }
        [InverseProperty("Review")]
        public virtual ICollection<ReviewCaptain> ReviewCaptain { get; set; }
        [InverseProperty("Review")]
        public virtual ICollection<ReviewYacht> ReviewYacht { get; set; }
    }
}
