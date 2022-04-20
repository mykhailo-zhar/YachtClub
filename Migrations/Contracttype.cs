using System;
using System.Collections.Generic;

namespace Project.Migrations
{
    public partial class Contracttype
    {
        public Contracttype()
        {
            Contract = new HashSet<Contract>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Contract> Contract { get; set; }
    }
}
