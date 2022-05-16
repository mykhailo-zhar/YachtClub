using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Migrations
{
    [Table("yacht_crew")]
    public partial class YachtCrew
    {
        public YachtCrew()
        {
            Contract = new HashSet<Contract>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("yachtid")]
        public int Yachtid { get; set; }
        [Column("crewid")]
        public int Crewid { get; set; }
        [Column("startdate", TypeName = "timestamp(2) without time zone")]
        public DateTime Startdate { get; set; }
        [Column("enddate", TypeName = "timestamp(2) without time zone")]
        public DateTime? Enddate { get; set; }
        [Column("description")]
        public string Description { get; set; }

        [ForeignKey(nameof(Crewid))]
        [InverseProperty(nameof(StaffPosition.YachtCrew))]
        public virtual StaffPosition Crew { get; set; }
        [ForeignKey(nameof(Yachtid))]
        [InverseProperty("YachtCrew")]
        public virtual Yacht Yacht { get; set; }
        [InverseProperty("Captaininyacht")]
        public virtual ICollection<Contract> Contract { get; set; }
    }
}
