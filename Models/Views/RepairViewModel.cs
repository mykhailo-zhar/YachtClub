using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Models
{
    public class RepairViewModel
    {
        [Column("_text")]
        public string Text { get; set; } 
        [Column("_status")]
        public string Status { get; set; }
        [Column("_who")]
        public string RepairManInfo { get; set; }

        [Column("_start")]
        public DateTime Start { get; set; }
        [Column("_end")]
        public DateTime End { get; set; }
        [Column("_fend")]
        public DateTime? Fend { get; set; }
    }
}
