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
            Hiredstaff = new HashSet<Hiredstaff>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("yachtid")]
        public int Yachtid { get; set; }
        [Column("crewid")]
        public int Crewid { get; set; }
        [Column("startdate")]
        public DateTime Startdate { get; set; }
        [Column("enddate")]
        public DateTime? Enddate { get; set; }
        [Column("description")]
        public string Description { get; set; }

        [ForeignKey(nameof(Crewid))]
        [InverseProperty(nameof(StaffPosition.YachtCrew))]
        public virtual StaffPosition Crew { get; set; }
        [ForeignKey(nameof(Yachtid))]
        [InverseProperty("YachtCrew")]
        public virtual Yacht Yacht { get; set; }
        [InverseProperty("Yachtwithcrew")]
        public virtual ICollection<Contract> Contract { get; set; }
        [InverseProperty("YachtCrew")]
        public virtual ICollection<Hiredstaff> Hiredstaff { get; set; }
    }
}
