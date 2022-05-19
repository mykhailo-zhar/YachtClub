using Project.Migrations;
using System.Collections.Generic;
namespace Project.Models
{
    public class MinCrewViewModel
    {
        public Yachttype Yachttype { get; set; }
        public IEnumerable<PositionYachttype> MinCrew { get; set; }
    }
}
