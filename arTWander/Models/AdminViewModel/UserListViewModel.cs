using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace arTWander.Models.AdminViewModel
{
    public class UserListViewModel
    {
        public ApplicationUser users { get; set; }

        [DisplayName("電話")]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(40)]
        [DisplayName("ID")]
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
        [DisplayName("登入狀態")]
        public bool Statue { get; set; }
        [DisplayName("用戶ID")]
        public int? FK_ApplicationUser { get; set; }

        //show資訊
        [DisplayName("展演名稱")]
        public string Title { get; set; }
        [DisplayName("留言")]
        //show comment
        public string Comment { get; set; }
        [DisplayName("星星評等")]
        public int Star { get; set; }

        //blacklist
        [DisplayName("狀態")]
        public string IsBlackList { get; set; }
    }
}