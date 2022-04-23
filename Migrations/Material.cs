using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Migrations
{
    [Table("material")]
    public partial class Material
    {
        public Material()
        {
            Extradationrequest = new HashSet<Extradationrequest>();
            Materiallease = new HashSet<Materiallease>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("name", TypeName = "character varying")]
        public string Name { get; set; }
        [Column("metric", TypeName = "character varying")]
        public string Metric { get; set; }
        [Column("typeid")]
        public int Typeid { get; set; }

        [ForeignKey(nameof(Typeid))]
        [InverseProperty(nameof(Materialtype.Material))]
        public virtual Materialtype Type { get; set; }
        [InverseProperty("MaterialNavigation")]
        public virtual ICollection<Extradationrequest> Extradationrequest { get; set; }
        [InverseProperty("MaterialNavigation")]
        public virtual ICollection<Materiallease> Materiallease { get; set; }
    }
}
