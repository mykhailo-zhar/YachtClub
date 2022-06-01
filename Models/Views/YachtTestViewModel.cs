using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Models
{
    public class YachtTestViewModel
    {
        [Column("_test")]
        public string Text { get; set; }
        [Column("_who")]
        public string RepairManInfo { get; set; }
        [Column("_date")]
        public DateTime Date { get; set; }
    }
}
