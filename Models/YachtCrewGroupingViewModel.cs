using Project.Migrations;
using System.Collections.Generic;
using System.Linq;

namespace Project.Models
{
    public class YachtCrewStatusViewModel
    {
        public string Status { get; set; }
        public YachtCrew Crew { get; set; }
    }
    public class YachtCrewGroupingViewModel
    {
        public IGrouping<Yacht, YachtCrewStatusViewModel> Group { get; set; }
        public bool AllowedOnlyCaptain { get; set; }
    }
}
