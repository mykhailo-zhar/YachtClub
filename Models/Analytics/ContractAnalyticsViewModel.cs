using Project.Migrations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Models
{

    public class ContractInfo
    {
        public bool Flag { get; set; }

        public string LikeName { get; set; }
        public string LikeSurname { get; set; }
        public string LikePhone { get; set; }
        public string LikeEmail { get; set; }
        public string LikeYName { get; set; }
        public string LikeYType { get; set; }

        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public IEnumerable<string> Yachts { get; set; }
        public IEnumerable<string> YachtType { get; set; }
        public ContractAnalyticsViewModel Contracts { get; set; } = new ContractAnalyticsViewModel();

    }
    public class ContractAnalyticsViewModel
    {
        [Column("avg_money")]
        public decimal Avg { get; set; }
        [Column("sum_money")]
        public decimal Sum { get; set; }
        [Column("count")]
        public long Count { get; set; }
    }
}
