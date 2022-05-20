using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Migrations
{
    [Table("busyyacht")]
    public partial class Busyyacht
    {
        [Column("id")]
        public int? Id { get; set; }
        [Column("r")]
        public bool? R { get; set; }
        [Column("c")]
        public bool? C { get; set; }
        [Column("e")]
        public bool? E { get; set; }
        [Column("val")]
        public bool? Val { get; set; }
        [Column("filled")]
        public bool? Filled { get; set; }

        public virtual Yacht Yacht { get; set; }
    }
}
