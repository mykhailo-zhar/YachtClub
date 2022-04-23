using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Migrations
{
    [Table("review_yacht")]
    public partial class ReviewYacht
    {
        [Key]
        [Column("reviewid")]
        public int Reviewid { get; set; }
        [Key]
        [Column("yachtid")]
        public int Yachtid { get; set; }

        [ForeignKey(nameof(Reviewid))]
        [InverseProperty("ReviewYacht")]
        public virtual Review Review { get; set; }
        [ForeignKey(nameof(Yachtid))]
        [InverseProperty("ReviewYacht")]
        public virtual Yacht Yacht { get; set; }
    }
}
