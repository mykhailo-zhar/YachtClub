using System;
using System.Collections.Generic;

namespace Project.Migrations
{
    public partial class Staff
    {
        public Staff()
        {
            StaffPosition = new HashSet<StaffPosition>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime Birthdate { get; set; }
        public string Sex { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime Hiringdate { get; set; }

        public virtual ICollection<StaffPosition> StaffPosition { get; set; }
    }
}
