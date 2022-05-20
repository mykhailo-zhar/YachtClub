using Project.Migrations;
using System.Collections.Generic;

namespace Project.Models
{
    public class PortfolioViewModel
    {
        public Person Person { get; set; }
        public IEnumerable<StaffPosition> Portfolio { get; set; }
        public IEnumerable<YachtCrew> YachtPortfolio { get; set; }
    }
}
