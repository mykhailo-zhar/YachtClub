using Project.Migrations;
using System.Collections.Generic;

namespace Project.Models
{
    public class MaterialLeaseWithMetrics
    {
        public Materiallease Materiallease { get; set; }
        public string Metrics { get; set; }
    }
}
