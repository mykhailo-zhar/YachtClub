using Project.Migrations;
using System.Collections.Generic;

namespace Project.Models
{
    public class MaterialLeaseWithMetricsViewModel
    {
        public Materiallease Materiallease { get; set; }
        public long Count { get; set; }
        public string Format { get; set; }
    }
}
