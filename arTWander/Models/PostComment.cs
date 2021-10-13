namespace arTWander.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PostComment")]
    public partial class PostComment
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PostComment()
        {
            ResponseComment = new HashSet<ResponseComment>();
            ResponseCommentLike = new HashSet<ResponseCommentLike>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Comment { get; set; }

        public int FK_ApplicationUser { get; set; }

        public int FK_Posts { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual Posts Posts { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ResponseComment> ResponseComment { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ResponseCommentLike> ResponseCommentLike { get; set; }
    }
}
