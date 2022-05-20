using System;
using System.Collections.Generic;
using System.Linq;
using Project.Migrations;

namespace Project.Models
{
    public class EventViewModel
    {
        public Event Event { get; set; }
        public IEnumerable<Winner> Winners { get; set; }
    }
}
