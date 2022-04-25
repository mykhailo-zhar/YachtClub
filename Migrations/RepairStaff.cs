using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Migrations
{
    [Table("repairmen")]
    public class RepairStaff
    {

        [Column("id")]
        public int? Id { get; set; }
        [Column("staffid")]
        public int? Staffid { get; set; }
        [Column("positionid")]
        public int? Positionid { get; set; }
        [Column("startdate", TypeName = "date")]
        public DateTime? Startdate { get; set; }
        [Column("enddate", TypeName = "date")]
        public DateTime? Enddate { get; set; }
        [Column("description")]
        public string? Description { get; set; }

        [ForeignKey(nameof(Staffid))]
        public virtual Person Staff { get; set; }
        [ForeignKey(nameof(Positionid))]
        public virtual Position Position{ get; set; }

    }
}
