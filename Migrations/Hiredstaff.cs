using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Migrations
{
    [Table("hiredstaff")]
    public partial class Hiredstaff
    {
        [Key]
        [Column("staffid")]
        public int Staffid { get; set; }
        [Key]
        [Column("clientid")]
        public int Clientid { get; set; }

        [ForeignKey(nameof(Clientid))]
        [InverseProperty(nameof(Person.Hiredstaff))]
        public virtual Person Client { get; set; }
        [ForeignKey(nameof(Staffid))]
        public virtual StaffPosition Staff { get; set; }
    }
}
