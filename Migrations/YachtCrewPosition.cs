using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Migrations
{
    [Table("yacht_crew_position")]
    public partial class YachtCrewPosition
    {
        [Column("id")]
        public int? Id { get; set; }
        [Column("yachtid")]
        public int? Yachtid { get; set; }
        [Column("crewid")]
        public int? Crewid { get; set; }
        [Column("positionid")]
        public int? Positionid { get; set; }
        [Column("positionname", TypeName = "character varying")]
        public string Positionname { get; set; }
    }
}
