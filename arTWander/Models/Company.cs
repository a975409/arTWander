namespace arTWander.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Company")]
    public partial class Company
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Company()
        {
            PageViewCounts = new HashSet<PageViewCount>();
            ReportsList = new HashSet<Reports>();
            ResponseShowComments = new HashSet<ResponseShowComment>();
            ShowPages = new HashSet<ShowPage>();
            ApplicationUserSubs = new HashSet<ApplicationUser>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        public string CompanyName { get; set; }

        [StringLength(255)]
        public string CompanyDescription { get; set; }

        [Required]
        [StringLength(255)]
        public string Address { get; set; }

        public int FK_City { get; set; }

        public int FK_District { get; set; }

        public int FK_ApplicationUser { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual City City { get; set; }

        public virtual District District { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PageViewCount> PageViewCounts { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Reports> ReportsList { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ResponseShowComment> ResponseShowComments { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ShowPage> ShowPages { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ApplicationUser> ApplicationUserSubs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CompanyLink> CompanyLinks { get; set; }
    }
}
