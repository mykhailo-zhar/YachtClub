using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Project.Migrations;

namespace Project.Models
{
    public class ExtradationRequestAvalilable
    {
        public Extradationrequest Extradationrequest { get; set; }
        public long Count { get; set; }
        public string Format { get; set; }
    }
}
