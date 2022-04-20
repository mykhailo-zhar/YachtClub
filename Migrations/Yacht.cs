using System;
using System.Collections.Generic;

namespace Project.Migrations
{
    public partial class Yacht
    {
        public Yacht()
        {
            Repair = new HashSet<Repair>();
            ReviewYacht = new HashSet<ReviewYacht>();
            Winner = new HashSet<Winner>();
            YachtCrew = new HashSet<YachtCrew>();
            Yachtlease = new HashSet<Yachtlease>();
            Yachttest = new HashSet<Yachttest>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public bool? Rentable { get; set; }
        public int Typeid { get; set; }
        public int Yachtownerid { get; set; }

        public virtual Yachttype Type { get; set; }
        public virtual Client Yachtowner { get; set; }
        public virtual ICollection<Repair> Repair { get; set; }
        public virtual ICollection<ReviewYacht> ReviewYacht { get; set; }
        public virtual ICollection<Winner> Winner { get; set; }
        public virtual ICollection<YachtCrew> YachtCrew { get; set; }
        public virtual ICollection<Yachtlease> Yachtlease { get; set; }
        public virtual ICollection<Yachttest> Yachttest { get; set; }
    }
}
