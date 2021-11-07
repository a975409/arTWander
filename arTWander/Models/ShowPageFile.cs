using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace arTWander.Models
{
    [Table("ShowPageFiles")]
    public class ShowPageFile
    {
        public int Id { get; set; }

        [Required]
        public string fileName { get; set; }

        public int FK_ShowPage { get; set; }

        public virtual ShowPage ShowPage { get; set; }
    }
}