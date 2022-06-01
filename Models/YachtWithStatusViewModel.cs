using Project.Migrations;
using System.Collections.Generic;

namespace Project.Models
{
    public class YachtWithStatusViewModel
    {
        public Yacht Yacht { get; set; }
        public string Status { get; set; }
        public IEnumerable< YachtCrew> Crew { get; set; }
        public IEnumerable<YachtTestViewModel> Tests { get; set; }
        public IEnumerable<RepairViewModel> Repairs { get; set; }
}
}
