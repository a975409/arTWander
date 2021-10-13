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

        public int? FK_Company { get; set; }

        public int? FK_ShowPage { get; set; }

        public int? FK_Posts { get; set; }

        public virtual Company Company { get; set; }

        public virtual Posts Posts { get; set; }

        public virtual ShowPage ShowPage { get; set; }
    }
}
