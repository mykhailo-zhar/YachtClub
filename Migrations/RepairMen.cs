using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Migrations
{
    [Table("repairmen")]
    public partial class Repairmen
    {
        [Column("id")]
        public int? Id { get; set; }
        [Column("staffid")]
        public int? Staffid { get; set; }
        [Column("positionid")]
        public int? Positionid { get; set; }
        [Column("startdate")]
        public DateTime? Startdate { get; set; }
        [Column("enddate")]
        public DateTime? Enddate { get; set; }
        [Column("description")]
        public string Description { get; set; }
    }
}
