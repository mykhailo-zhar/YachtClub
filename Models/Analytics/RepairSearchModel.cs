using Project.Migrations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Models
{
    public class RepairInfo
    {
        public bool Flag { get; set; }

        public string LikeName { get; set; }
        public string LikeSurname { get; set; }
        public string LikePhone { get; set; }
        public string LikeEmail { get; set; }
        public string LikeYName { get; set; }
        public string LikeYType { get; set; }
        public bool Active { get; set; }

        public IEnumerable<string> Yachts { get; set; }
        public IEnumerable<string> YachtType { get; set; }

        public List<RepairSearchModel> Repairs { get; set; } = new List<RepairSearchModel>();
    }
    public class RepairSearchModel
    {
        [Column("p_name")]
        public string Name { get; set; }
        [Column("p_surname")]
        public string Surname { get; set; }
        [Column("p_email")]
        public string Email { get; set; }
        [Column("p_phone")]
        public string Phone { get; set; }
        [Column("yachtname")]
        public string Yachtname { get; set; }
        [Column("yachttype")]
        public string Yachttype { get; set; }
        [Column("p_startdate")]
        public DateTime Startdate { get; set; }
        [Column("p_enddate")]
        public DateTime? Enddate { get; set; } 
        [Column("p_status")]
        public string Status { get; set; }
    }

}
