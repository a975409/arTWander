namespace arTWander.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ShowPage")]
    public partial class ShowPage
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ShowPage()
        {
            PageToTodays = new HashSet<PageToTodays>();
            PageViewCount = new HashSet<PageViewCount>();
            Reports = new HashSet<Reports>();
            ShowComment = new HashSet<ShowComment>();
            ApplicationUser = new HashSet<ApplicationUser>();
            Keywords = new HashSet<Keywords>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(30)]
        public string Title { get; set; }

        [Required]
        [StringLength(255)]
        public string Description { get; set; }

        [Required]
        [StringLength(30)]
        public string ImgFileName { get; set; }

        [Required]
        [StringLength(30)]
        public string ImgContent { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public bool Cost { get; set; }

        public int? Price { get; set; }

        public bool AgeRange { get; set; }

        [Required]
        [StringLength(255)]
        public string Address { get; set; }

        [Required]
        [StringLength(255)]
        public string Remark { get; set; }

        public DateTime? Created_At { get; set; }

        public int FK_Company { get; set; }

        public int FK_City { get; set; }

        public int FK_District { get; set; }

        public int FK_Post { get; set; }

        public virtual City City { get; set; }

        public virtual Company Company { get; set; }

        public virtual District District { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PageToTodays> PageToTodays { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PageViewCount> PageViewCount { get; set; }

        public virtual Posts Posts { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Reports> Reports { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ShowComment> ShowComment { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ApplicationUser> ApplicationUser { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Keywords> Keywords { get; set; }
    }
}
