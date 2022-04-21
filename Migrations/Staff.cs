using System.ComponentModel.DataAnnotations;
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

        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime Birthdate { get; set; }
        [Required]
        public string Sex { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [Phone]
        public string Phone { get; set; }

        [DataType(DataType.Date)]
        public DateTime Hiringdate { get; set; }

        public virtual ICollection<StaffPosition> StaffPosition { get; set; }
    }
}
