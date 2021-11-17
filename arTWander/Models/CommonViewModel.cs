using Microsoft.AspNet.Identity;
using PagedList;
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

            [Required(ErrorMessage = "請填寫聯絡電話")]
            [Phone(ErrorMessage = "請填寫有效的聯絡電話")]
            [StringLength(20, ErrorMessage = "字數最多20個字")]
            [DisplayName("聯絡電話")]
            public string PhoneNumber { get; set; }

            public bool TwoFactor { get; set; }

            public bool BrowserRemembered { get; set; }

            [Required]
            [StringLength(40)]
            [DisplayName("使用者姓名")]
            public string UserName { get; set; }

            public string Birthday { get; set; }

            [Required]
            [StringLength(255)]
            [DisplayName("地址")]
            public string AccountAddress { get; set; }

            public string AvatarUrl { get; set; }

            public string AvatarName { get; set; }

            [Required(ErrorMessage = "請填寫電子信箱")]
            [StringLength(100, ErrorMessage = "字數最多100個字")]
            [EmailAddress(ErrorMessage = "請填寫有效的電子信箱")]
            [DisplayName("電子信箱")]
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
            public bool end { get; set; }
        }

        public class CommonMyShowViewNodel
        {
            public List<City> allCity { get; set; }
            public IPagedList<CommonShowViewModel> allShow { get; set; }
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