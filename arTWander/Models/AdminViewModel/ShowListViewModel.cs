using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace arTWander.Models.AdminViewModel
{
    public class ShowListViewModel
    {

        public int Id { get; set; }

        [DisplayName("展演主題")]
        public string Title { get; set; }

        [DisplayName("展演內容")]
        public string Description { get; set; }

        
        [DisplayName("開放日期")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [DisplayName("結束日期")]
        public DateTime EndDate { get; set; }

        [DisplayName("展出時間")]
        public DateTime StartTime { get; set; }

        [DisplayName("結束時間")]
        public DateTime EndTime { get; set; }

        [DisplayName("是否收費？")]
        public bool Cost { get; set; }

        [DisplayName("收費金額($TW)")]
        public int Price { get; set; }

        [DisplayName("未滿18歲可觀看？")]
        public bool AgeRange { get; set; }

        [DisplayName("地址")]
        public string Address { get; set; }

        [DisplayName("備註")]
        public string Remark { get; set; }

        [DisplayName("關鍵字")]
        public string[] Keywords { get; set; }

        [DisplayName("展出時段")]
        public int[] Todays { get; set; }

        [DisplayName("展演建立日期")]
        public DateTime Created_At { get; set; }

        public string[] images { get; set; }

        public ulong ViewCount { get; set; }

        public ShowComment[] showComments { get; set; }

        //縣市
        [DisplayName("縣市")]
        public string CityName { get; set; }

        //鄉鎮
        [DisplayName("鄉鎮")]
        public string DistrictName { get; set; }

        //展演單位
        [DisplayName("展演單位名稱")]
        public string CompanyName { get; set; }

        [DisplayName("展演單位簡介")]
        public string CompanyDescription { get; set; }


        [DisplayName("電話")]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(40)]
        [DisplayName("負責人")]
        public string UserName { get; set; }


        public string ShowImg { get; set; }
    }
}