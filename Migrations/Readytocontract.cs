using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Migrations
{
    [Table("readytocontract")]
    public partial class Readytocontract
    {
        [Column("id")]
        public int? Id { get; set; }
        [Column("name", TypeName = "character varying")]
        public string Name { get; set; }
        [Column("rentable")]
        public bool? Rentable { get; set; }
        [Column("registrydate", TypeName = "timestamp(2) without time zone")]
        public DateTime? Registrydate { get; set; }
        [Column("description")]
        public string Description { get; set; }
        [Column("typeid")]
        public int? Typeid { get; set; }
        [Column("yachtownerid")]
        public int? Yachtownerid { get; set; }
        [Column("status")]
        public string Status { get; set; }
    }
}
