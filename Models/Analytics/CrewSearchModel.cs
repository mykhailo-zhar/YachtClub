using Project.Migrations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Models
{
    public class CrewInfo
    {
        public bool Flag { get; set; }

        public string LikeName { get; set; }
        public string LikeSurname { get; set; }
        public string LikePhone { get; set; }
        public string LikeEmail { get; set; }
        public string LikePosition { get; set; }
        public string LikeYName { get; set; }
        public string LikeYType { get; set; }
        public bool Active { get; set; }

        public IEnumerable<string> Yachts { get; set; }
        public IEnumerable<string> YachtType { get; set; }
        public IEnumerable<string> Positions { get; set; }
        public List<CrewSearchModel> Crew { get; set; } = new List<CrewSearchModel>();

    }
    public class CrewSearchModel
    {
        [Column("p_name")]
        public string Name { get; set; }
        [Column("p_surname")]
        public string Surname { get; set; }
        [Column("p_email")]
        public string Email { get; set; }
        [Column("p_phone")]
        public string Phone { get; set; }
        [Column("p_position")]
        public string Position { get; set; }
        [Column("yachtname")]
        public string Yachtname { get; set; }
        [Column("yachttype")]
        public string Yachttype { get; set; }
        [Column("p_startdate")]
        public DateTime Startdate { get; set; }
        [Column("p_enddate")]
        public DateTime? Enddate { get; set; }
    }
}
