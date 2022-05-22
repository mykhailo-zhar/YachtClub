using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Project.Migrations;

namespace Project.Models
{
    public class ExtradationRequestAvalilableViewModel
    {
        public Extradationrequest Extradationrequest { get; set; }
        public long Count { get; set; }
        public string Format { get; set; }
        public bool LeadRight { get; set; }
    }
}
