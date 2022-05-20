using Project.Migrations;
using System.Collections.Generic;
namespace Project.Models
{
    public class MaterialViewModel
    {
        public Material Material {get; set;}
        public long Count { get; set; }
        public string Format { get; set; }
    }
}
