using Project.Migrations;
using System.Collections.Generic;
namespace Project.Models
{
    public class MaterialViewModel
    {
        public IEnumerable<Material> Materials {get; set;}
        public IEnumerable<Availableresources> Availableresources { get; set; }
    }
}
