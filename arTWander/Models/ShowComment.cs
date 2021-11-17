namespace arTWander.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ShowComment")]
    public partial class ShowComment
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ShowComment()
        {
            ReportsList = new HashSet<Reports>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Comment { get; set; }

        public int Star { get; set; }

        public DateTime? CommentDate { get; set; }

        public int FK_ShowPage { get; set; }

        public int FK_ApplicationUser { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Reports> ReportsList { get; set; }

        public virtual ShowPage ShowPage { get; set; }
    }
}
