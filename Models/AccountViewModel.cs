using Project.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Models
{
    public class AccountViewModel
    {
        public string Password { get; set; }
        public string AsWho { get; set; }
        public string Login { get; set; }
        public string Position { get; set; }
        public Person User { get; set; }
    }
}
