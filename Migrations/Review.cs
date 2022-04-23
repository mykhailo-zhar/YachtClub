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
        [Column("contractid")]
        public int Contractid { get; set; }
        [Column("date", TypeName = "date")]
        public DateTime Date { get; set; }
        [Required]
        [Column("text")]
        public string Text { get; set; }
        [Column("rate")]
        public int Rate { get; set; }

        [ForeignKey(nameof(Clientid))]
        [InverseProperty("Review")]
        public virtual Client Client { get; set; }
        [ForeignKey(nameof(Contractid))]
        [InverseProperty("Review")]
        public virtual Contract Contract { get; set; }
        [InverseProperty("Review")]
        public virtual ICollection<ReviewCaptain> ReviewCaptain { get; set; }
        [InverseProperty("Review")]
        public virtual ICollection<ReviewYacht> ReviewYacht { get; set; }
    }
}
