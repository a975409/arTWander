namespace arTWander.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ResponseCommentLike")]
    public partial class ResponseCommentLike
    {
        [Key]
        [Column(Order = 0)]
        public bool Statue { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int FK_ResponsePost { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int FK_ApplicationUser { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual PostComment PostComment { get; set; }
    }
}
