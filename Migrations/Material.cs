using System;
using System.Collections.Generic;

namespace Project.Migrations
{
    public partial class Material
    {
        public Material()
        {
            Extradationrequest = new HashSet<Extradationrequest>();
            Materiallease = new HashSet<Materiallease>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Metric { get; set; }
        public int Typeid { get; set; }

        public virtual Materialtype Type { get; set; }
        public virtual ICollection<Extradationrequest> Extradationrequest { get; set; }
        public virtual ICollection<Materiallease> Materiallease { get; set; }
    }
}
