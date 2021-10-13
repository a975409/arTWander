namespace arTWander.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BlackList")]
    public partial class BlackList
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Reason { get; set; }

        public DateTime? Created_At { get; set; }

        public int FK_ApplicationUser { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
