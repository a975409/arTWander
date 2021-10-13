namespace arTWander.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PostLike")]
    public partial class PostLike
    {
        public int Id { get; set; }

        public bool Statue { get; set; }

        public int FK_ApplicationUser { get; set; }

        public int FK_Posts { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual Posts Posts { get; set; }
    }
}
