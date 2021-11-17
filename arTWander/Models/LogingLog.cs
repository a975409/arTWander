namespace arTWander.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LogingLog")]
    public partial class LogingLog
    {
        [Key]
        public int LoginLogId { get; set; }

        public DateTime? RegisterTime { get; set; }

        public DateTime? LastloginTime { get; set; }

        public DateTime? LoginOutTime { get; set; }

        public int? LogingCount { get; set; }

        public bool Statue { get; set; }

        public int? FK_ApplicationUser { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
