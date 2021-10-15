namespace arTWander.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PageToTodays
    {
        public int Id { get; set; }

        public int Today { get; set; }

        public int FK_ShowPage { get; set; }

        public virtual ShowPage ShowPage { get; set; }
    }
}
