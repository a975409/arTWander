using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace arTWander.Models.AdminViewModel
{
    public class SetupViewModel
    {

        public bool HasPassword { get; set; }

        public IList<UserLoginInfo> Logins { get; set; }

        public string PhoneNumber { get; set; }

        public bool TwoFactor { get; set; }

        public bool BrowserRemembered { get; set; }

        [Required]
        [StringLength(40)]
        public string UserName { get; set; }

        public string Birthday { get; set; }

        [Required]
        [StringLength(255)]
        [DisplayName("地址")]
        public string AccountAddress { get; set; }

        public string AvatarUrl { get; set; }

        public string AvatarName { get; set; }

        public string Email { get; set; }

        public bool IsSuscribe { get; set; }


    }
}