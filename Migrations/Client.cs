using System;
using System.Collections.Generic;

namespace Project.Migrations
{
    public partial class Client
    {
        public Client()
        {
            Contract = new HashSet<Contract>();
            Review = new HashSet<Review>();
            Yacht = new HashSet<Yacht>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime Birthdate { get; set; }
        public string Sex { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public virtual ICollection<Contract> Contract { get; set; }
        public virtual ICollection<Review> Review { get; set; }
        public virtual ICollection<Yacht> Yacht { get; set; }
    }
}
