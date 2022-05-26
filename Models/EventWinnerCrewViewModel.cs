using System;
using System.Collections.Generic;
using System.Linq;
using Project.Migrations;


namespace Project.Models
{
    public class YachtCrewWinners{
        public YachtCrew Crew { get; set; }
        public Winner Winner{ get; set; }
    }
    public class EventWinnerCrewViewModel
    {
        public Event Event { get; set; }
        public IEnumerable<YachtCrewWinners> Winners { get; set; }
    }
}
