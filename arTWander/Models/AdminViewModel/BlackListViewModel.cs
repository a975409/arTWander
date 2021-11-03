﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace arTWander.Models.AdminViewModel
{
    public class BlackListViewModel
    {
        //一般使用者

        [DisplayName("電話")]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(40)]
        [DisplayName("用戶")]
        public string UserName { get; set; }

        [DisplayName("生日")]
        public string Birthday { get; set; }

        [Required]
        [StringLength(255)]
        [DisplayName("地址")]
        public string AccountAddress { get; set; }

        public string AvatarUrl { get; set; }

        public string AvatarName { get; set; }
        [DisplayName("信箱")]
        public string Email { get; set; }

        //展演單位
        [DisplayName("展演單位名稱")]
        public string CompanyName { get; set; }

        [DisplayName("展演單位簡介")]
        public string CompanyDescription { get; set; }

        [StringLength(255)]
        [DisplayName("地址")]
        public string CompanyAddress { get; set; }

        [DisplayName("大頭照")]
        public string PhotoSticker { get; set; }

        [DisplayName("傳真：")]
        public string Fax { get; set; }

        [DisplayName("展演數量")]
        public int ShowCount { get; set; }

        [DisplayName("縣市")]
        public string CityName { get; set; }

        [DisplayName("鄉鎮")]
        public string DistrictName { get; set; }

        //登入資訊
        public int LoginLogId { get; set; }
        [DisplayName("註冊日")]
        public DateTime? RegisterTime { get; set; }
        [DisplayName("最後登入時間")]
        public DateTime? LastloginTime { get; set; }
        [DisplayName("最近登出時間")]
        public DateTime? LoginOutTime { get; set; }
        [DisplayName("登入次數")]
        public int? LogingCount { get; set; }
        [DisplayName("在線狀態")]
        public bool Statue { get; set; }


        //黑名單
        [DisplayName("Id")]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        [DisplayName("黑名單原因")]
        public string Reason { get; set; }

        [DisplayName("黑名單日期")]
        public DateTime? Created_At { get; set; }

        [DisplayName("用戶")]
        public int FK_ApplicationUser { get; set; }


        //blacklist
        [DisplayName("狀態")]
        public string IsBlackList { get; set; }

        //jason 讀取
        public string userId { get; set; }
    }
}