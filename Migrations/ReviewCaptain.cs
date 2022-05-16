using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Migrations
{
    [Table("review_captain")]
    public partial class ReviewCaptain
    {
        [Key]
        [Column("reviewid")]
        public int Reviewid { get; set; }
        [Key]
        [Column("captainid")]
        public int Captainid { get; set; }

        [ForeignKey(nameof(Captainid))]
        [InverseProperty(nameof(Person.ReviewCaptain))]
        public virtual Person Captain { get; set; }
        [ForeignKey(nameof(Reviewid))]
        [InverseProperty("ReviewCaptain")]
        public virtual Review Review { get; set; }
    }
}
