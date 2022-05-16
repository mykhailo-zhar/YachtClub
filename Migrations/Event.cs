using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Migrations
{
    [Table("event")]
    public partial class Event
    {
        public Event()
        {
            Winner = new HashSet<Winner>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("name", TypeName = "character varying")]
        public string Name { get; set; }
        [Column("startdate", TypeName = "timestamp(2) without time zone")]
        public DateTime Startdate { get; set; }
        [Column("enddate", TypeName = "timestamp(2) without time zone")]
        public DateTime? Enddate { get; set; }
        [Column("duration", TypeName = "timestamp(2) without time zone")]
        public DateTime Duration { get; set; }
        [Column("description")]
        public string Description { get; set; }
        [Column("userrate")]
        public int? Userrate { get; set; }
        [Required]
        [Column("canhavewinners")]
        public bool? Canhavewinners { get; set; }

        [InverseProperty("Event")]
        public virtual ICollection<Winner> Winner { get; set; }
    }
}
