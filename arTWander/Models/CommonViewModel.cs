using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace arTWander.Models
{
    public class CommonViewModel
    {
        public class CommonInfoViewModel
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

        public class CommonShowViewModel
        {
            public string showCity { get; set; }
            public string showTitle { get; set; }
            public string showDiscription { get; set; }
            public string showCompany { get; set; }
            public string showImg { get; set; }
            public int showId { get; set; }
        }

        public class CommonMyShowViewNodel
        {
            public List<City> allCity { get; set; }
            public List<CommonShowViewModel> allShow { get; set; }
        }

        public class CommonMyItineraryPage
        {
            public int showId { get; set; }

            public City city { get; set; }

            public string showCompany { get; set; }

            public string showTitle { get; set; }

            public string showAddress { get; set; }

        }
    }
}