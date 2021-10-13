namespace arTWander.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ResponseComment")]
    public partial class ResponseComment
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Comment { get; set; }

        public int FK_ResponsePost { get; set; }

        public int FK_ApplicationUser { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual PostComment PostComment { get; set; }
    }
}
