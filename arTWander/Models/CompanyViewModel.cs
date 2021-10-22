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
            [Required(ErrorMessage = "請填寫展演單位名稱")]
            [StringLength(10)]
            [DisplayName("展演單位名稱")]
            public string CompanyName { get; set; }

            [StringLength(300)]
            [DisplayName("展演單位簡介")]
            public string CompanyDescription { get; set; }

            [DisplayName("縣市")]
            public int FK_City { get; set; }

            [DisplayName("鄉鎮市區")]
            public int FK_District { get; set; }

            [Required]
            [StringLength(5)]
            [DisplayName("區碼")]
            public string LocalCode { get; set; }

            [Required(ErrorMessage = "請填寫地址")]
            [StringLength(100)]
            [DisplayName("地址")]
            public string Address { get; set; }

            [StringLength(100)]
            [EmailAddress(ErrorMessage = "請填寫有效的電子信箱")]
            [DisplayName("電子信箱")]
            public string Email { get; set; }

            [Phone(ErrorMessage = "請填寫有效的聯絡電話")]
            [StringLength(20)]
            [DisplayName("聯絡電話")]
            public string Phone { get; set; }

            [StringLength(20)]
            [DisplayName("傳真")]
            public string Fax { get; set; }

            [Url(ErrorMessage = "請填寫有效的網址")]
            [StringLength(100)]
            [DisplayName("官方網站")]
            public string HomePage { get; set; }

            [Required(ErrorMessage = "請填寫營業時間")]
            [StringLength(100)]
            [DisplayName("營業時間")]
            public string BusinessHours { get; set; }

        }
    }
}