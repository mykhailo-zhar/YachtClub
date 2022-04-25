using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Migrations
{
    [Table("staff_position")]
    public partial class StaffPosition
    {
        public StaffPosition()
        {
            Extradationrequest = new HashSet<Extradationrequest>();
            RepairMen1 = new HashSet<RepairMen1>();
            ReviewCaptain = new HashSet<ReviewCaptain>();
            YachtCrew = new HashSet<YachtCrew>();
            Yachttest = new HashSet<Yachttest>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("staffid")]
        public int Staffid { get; set; }
        [Column("positionid")]
        public int Positionid { get; set; }
        [Column("startdate")]
        public DateTime Startdate { get; set; }
        [Column("enddate")]
        public DateTime? Enddate { get; set; }
        [Column("description")]
        public string Description { get; set; }

        [ForeignKey(nameof(Positionid))]
        [InverseProperty("StaffPosition")]
        public virtual Position Position { get; set; }
        [ForeignKey(nameof(Staffid))]
        [InverseProperty(nameof(Person.StaffPosition))]
        public virtual Person Staff { get; set; }
        [InverseProperty("Staff")]
        public virtual ICollection<Extradationrequest> Extradationrequest { get; set; }
        [InverseProperty("Staff")]
        public virtual ICollection<RepairMen1> RepairMen1 { get; set; }
        [InverseProperty("Captain")]
        public virtual ICollection<ReviewCaptain> ReviewCaptain { get; set; }
        [InverseProperty("Crew")]
        public virtual ICollection<YachtCrew> YachtCrew { get; set; }
        [InverseProperty("Staff")]
        public virtual ICollection<Yachttest> Yachttest { get; set; }
    }
}
