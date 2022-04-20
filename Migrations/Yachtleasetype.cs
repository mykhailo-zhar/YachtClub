using System;
using System.Collections.Generic;

namespace Project.Migrations
{
    public partial class Yachtleasetype
    {
        public Yachtleasetype()
        {
            Yachtlease = new HashSet<Yachtlease>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Yachtlease> Yachtlease { get; set; }
    }
}
