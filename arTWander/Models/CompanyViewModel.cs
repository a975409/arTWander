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
        [DisplayName("展演單位名稱")]
        public string CompanyName { get; set; }

        [DisplayName("展演單位簡介")]
        public string CompanyDescription { get; set; }

        [DisplayName("地址")]
        public string Address { get; set; }

        [DisplayName("電子信箱：")]
        public string Email { get; set; }

        [DisplayName("聯絡電話：")]
        public string Phone { get; set; }

        [DisplayName("傳真：")]
        public string Fax { get; set; }

        [DisplayName("官方網站")]
        public string HomePage { get; set; }

        [DisplayName("營業時間")]
        public string BusinessHours { get; set; }

        [DisplayName("大頭照")]
        public string PhotoSticker { get; set; }

        [DisplayName("封面照")]
        public string PromotionalImage { get; set; }

        //Manage的Index原有的功能
        public bool HasPassword { get; set; }
        public IList<UserLoginInfo> Logins { get; set; }
        public string PhoneNumber { get; set; }
        public bool TwoFactor { get; set; }
        public bool BrowserRemembered { get; set; }

    }

    public class CompanyEditViewModel
    {
        [Required(ErrorMessage = "請填寫展演單位名稱")]
        [StringLength(10, ErrorMessage = "字數最多10個字")]
        [DisplayName("展演單位名稱")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "請填寫展演單位簡介")]
        [StringLength(1000, ErrorMessage = "填寫展演單位簡介，限字數1000字以內")]
        [DisplayName("展演單位簡介")]
        public string CompanyDescription { get; set; }

        [Url(ErrorMessage = "請填寫有效的網址")]
        [StringLength(100, ErrorMessage = "字數最多100個字")]
        [DisplayName("官方網站")]
        public string HomePage { get; set; }

        [Required(ErrorMessage = "請填寫營業時間")]
        [StringLength(100, ErrorMessage = "字數最多100個字")]
        [DisplayName("營業時間")]
        public string BusinessHours { get; set; }

        [DisplayName("縣市")]
        public int FK_City { get; set; }

        [DisplayName("鄉鎮市區")]
        public int FK_District { get; set; }

        [Required(ErrorMessage = "請填寫地址")]
        [StringLength(100, ErrorMessage = "字數最多100個字")]
        [DisplayName("地址")]
        public string Address { get; set; }

        [Required(ErrorMessage = "請填寫電子信箱")]
        [StringLength(100, ErrorMessage = "字數最多100個字")]
        [EmailAddress(ErrorMessage = "請填寫有效的電子信箱")]
        [DisplayName("電子信箱")]
        public string Email { get; set; }

        [Required(ErrorMessage = "請填寫聯絡電話")]
        [Phone(ErrorMessage = "請填寫有效的聯絡電話")]
        [StringLength(20, ErrorMessage = "字數最多20個字")]
        [DisplayName("聯絡電話")]
        public string Phone { get; set; }

        [StringLength(20, ErrorMessage = "字數最多20個字")]
        [DisplayName("傳真")]
        public string Fax { get; set; }
    }
}