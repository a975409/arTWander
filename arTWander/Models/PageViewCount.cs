namespace arTWander.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PageViewCount")]
    public partial class PageViewCount
    {
        public int Id { get; set; }

        public int? Count { get; set; }

        public int? FK_ApplicationUser { get; set; }

        public int? FK_ShowPage { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual ShowPage ShowPage { get; set; }
    }
}
