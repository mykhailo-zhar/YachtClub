using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Models
{
    public class MaterialInfo
    {
        public bool Flag  { get; set; }

        public string LikeName { get; set; }
        public string LikeTypeName { get; set; }

        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public List<MaterialAnalyticsViewModel> Materials { get; set; } = new List<MaterialAnalyticsViewModel>();

    }
    public class MaterialAnalyticsViewModel
    {
        [Column("mname")]
        public string Name { get; set; }
        [Column("mtype")]
        public string TypeName { get; set; }
        [Column("avg_money")]
        public decimal Avg { get; set; }
        [Column("sum_money")]
        public decimal Sum { get; set; }
        [Column("remains")]
        public long ReqCount { get; set; }
        [Column("all_mat")]
        public long AllCount { get; set; }
        [Column("mlid")]
        public long Contracts { get; set; } 
        [Column("erid")]
        public long Requests { get; set; }
        [Column("metric")]
        public string Metric { get; set; }
    }
}
