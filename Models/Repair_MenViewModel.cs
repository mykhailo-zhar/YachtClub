using Project.Migrations;
using System.Collections.Generic;
using System.Linq;
namespace Project.Models
{
    public class Repair_MenViewModel
    {
        public Repair Repair { get; set; }
        public IEnumerable<RepairMen> RepairMen { get; set; }
        public IEnumerable<Extradationrequest> Extradationrequests { get; set; }
    }
}
