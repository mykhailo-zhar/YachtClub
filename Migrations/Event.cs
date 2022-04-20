using System;
using System.Collections.Generic;

namespace Project.Migrations
{
    public partial class Event
    {
        public Event()
        {
            Winner = new HashSet<Winner>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Startdate { get; set; }
        public DateTime? Enddate { get; set; }
        public DateTime Duration { get; set; }
        public string Status { get; set; }

        public virtual ICollection<Winner> Winner { get; set; }
    }
}
