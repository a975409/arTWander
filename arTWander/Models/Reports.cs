namespace arTWander.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Reports
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Reason { get; set; }

        public DateTime? Created_At { get; set; }

        [StringLength(10)]
        public string ResponseStatus { get; set; }

        [StringLength(255)]
        public string ResponseComment { get; set; }

        public DateTime? Response_At { get; set; }

        public int? FK_ApplicationUser { get; set; }

        public int? FK_Company { get; set; }

        public int? FK_ShowPage { get; set; }

        public int? FK_ResponseShowComment { get; set; }

        public int? FK_ShowComment { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual Company Company { get; set; }

        public virtual ResponseShowComment ResponseShowComment { get; set; }

        public virtual ShowPage ShowPage { get; set; }

        public virtual ShowComment ShowComment { get; set; }
    }
}
