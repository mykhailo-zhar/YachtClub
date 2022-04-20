using System;
using System.Collections.Generic;

namespace Project.Migrations
{
    public partial class Materialtype
    {
        public Materialtype()
        {
            Material = new HashSet<Material>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Material> Material { get; set; }
    }
}
