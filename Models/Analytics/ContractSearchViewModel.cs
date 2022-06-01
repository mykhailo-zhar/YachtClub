using Project.Migrations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Models
{

    public class ContractSearchInfo
    {
        public bool Flag { get; set; }
        public bool IsPaid { get; set; }

        public string LikeCName { get; set; }
        public string LikeCSurname { get; set; }
        public string LikeCPhone { get; set; }
        public string LikeCEmail { get; set; }
        public string LikeCaName { get; set; }
        public string LikeCaSurname { get; set; }
        public string LikeCaPhone { get; set; }
        public string LikeCaEmail { get; set; }
        public string LikeYName { get; set; }
        public string LikeYType { get; set; }
        public string LikeCType { get; set; }

        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public IEnumerable<string> Yachts { get; set; }
        public IEnumerable<string> YachtType { get; set; }
        public IEnumerable<string> Contracttype { get; set; }
        public IEnumerable<ContractSearchViewModel> Contracts { get; set; } = new List<ContractSearchViewModel>();

    }
    public class ContractSearchViewModel
    {
        [Column("_id")]
        public string Id { get; set; }
        [Column("_start")]
        public DateTime Start { get; set; }
        [Column("_end")]
        public DateTime End { get; set; }
        [Column("_fend")]
        public DateTime? FactEnd { get; set; }
        [Column("_clinfo")]
        public string ClientInfo { get; set; }
        [Column("_yinfo")]
        public string YachtInfo { get; set; }
        [Column("_capinfo")]
        public string CapInfo { get; set; }
        [Column("_specials")]
        public string Specials { get; set; }
        [Column("_averall")]
        public decimal Averall { get; set; }
        [Column("_ctname")]
        public string CName { get; set; }
        [Column("_ctprice")]
        public decimal CPrice { get; set; }

    }
}
