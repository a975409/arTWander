using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace arTWander.Models
{
    public class CompanyViewModel
    {
        public class CompanyInfoViewModel
        {
            [Required]
            [EmailAddress]
            [DisplayName("電子信箱(帳號)")]
            public string Email { get; set; }

            public bool HasPassword { get; set; }
            public IList<UserLoginInfo> Logins { get; set; }
            public string PhoneNumber { get; set; }
            public bool TwoFactor { get; set; }
            public bool BrowserRemembered { get; set; }

            [Required]
            [StringLength(10)]
            [DisplayName("展演單位名稱")]
            public string CompanyName { get; set; }

            [StringLength(255)]
            [DisplayName("說明")]
            public string CompanyDescription { get; set; }

            [DisplayName("縣市")]
            public string CityName { get; set; }

            [DisplayName("鄉鎮市區")]
            public string DistrictName { get; set; }

            [Required]
            [StringLength(255)]
            [DisplayName("地址")]
            public string Address { get; set; }

            public CompanyLinkViewModel[] companyLinks { get; set; }

        }

        public class CompanyLinkViewModel
        {
            [Required]
            [StringLength(10)]
            [DisplayName("連結名稱")]
            public string Title { get; set; }

            [Required]
            [StringLength(255)]
            [DisplayName("網址")]
            public string link { get; set; }
        }
    }
}