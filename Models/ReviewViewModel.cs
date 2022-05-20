using Project.Migrations;
using System.Collections.Generic;

namespace Project.Models
{
    public class ReviewViewModel
    {
        public Review Review { get; set; }
        public IEnumerable<ReviewCaptain> Captains { get; set; }
        public IEnumerable<ReviewYacht> Yachts{ get; set; }
    }
}
