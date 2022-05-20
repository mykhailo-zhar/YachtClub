using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Migrations
{
    [Table("availableresources")]
    public partial class Availableresources
    {
        [Column("material")]
        public int Material { get; set; }
        [Column("count")]
        public long Count { get; set; }
        [Column("format", TypeName = "character varying")]
        public string Format { get; set; }
    }
}
