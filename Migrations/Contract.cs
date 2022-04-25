using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Migrations
{
    [Table("contract")]
    public partial class Contract
    {
        public Contract()
        {
            Review = new HashSet<Review>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("clientid")]
        public int Clientid { get; set; }
        [Column("contracttypeid")]
        public int Contracttypeid { get; set; }
        [Column("yachtwithcrewid")]
        public int Yachtwithcrewid { get; set; }
        [Column("startdate")]
        public DateTime Startdate { get; set; }
        [Column("enddate")]
        public DateTime? Enddate { get; set; }
        [Column("duration")]
        public DateTime Duration { get; set; }
        [Required]
        [Column("specials")]
        public string Specials { get; set; }
        [Required]
        [Column("status", TypeName = "character varying")]
        public string Status { get; set; }
        [Column("averallprice", TypeName = "numeric")]
        public decimal Averallprice { get; set; }

        [ForeignKey(nameof(Clientid))]
        [InverseProperty(nameof(Person.Contract))]
        public virtual Person Client { get; set; }
        [ForeignKey(nameof(Contracttypeid))]
        [InverseProperty("Contract")]
        public virtual Contracttype Contracttype { get; set; }
        [ForeignKey(nameof(Yachtwithcrewid))]
        [InverseProperty(nameof(YachtCrew.Contract))]
        public virtual YachtCrew Yachtwithcrew { get; set; }
        [InverseProperty("Contract")]
        public virtual ICollection<Review> Review { get; set; }
    }
}
