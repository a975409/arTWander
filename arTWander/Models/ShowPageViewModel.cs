using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace arTWander.Models
{
    public class ShowMinViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int Comment { get; set; }

        public string fileName { get; set; }

        public string cityName { get; set; }

        public int cityId { get; set; }

        public bool end { get; set; }
    }

    public class ShowPageViewModel
    {
        public int Id { get; set; }

        [DisplayName("展演主題：")]
        public string Title { get; set; }

        [DisplayName("展演內容：")]
        public string Description { get; set; }

        [DisplayName("開放日期")]
        public DateTime StartDate { get; set; }

        [DisplayName("結束日期")]
        public DateTime EndDate { get; set; }

        [DisplayName("展出時間")]
        public DateTime StartTime { get; set; }

        [DisplayName("結束時間")]
        public DateTime EndTime { get; set; }

        [DisplayName("是否收費：")]
        public bool Cost { get; set; }

        [DisplayName("收費金額($TW)：")]
        public int Price { get; set; }

        [DisplayName("未滿18歲可觀看：")]
        public bool AgeRange { get; set; }

        [DisplayName("地址")]
        public string Address { get; set; }

        [DisplayName("備註")]
        public string Remark { get; set; }

        [DisplayName("關鍵字")]
        public string[] Keywords { get; set; }

        [DisplayName("展出時段")]
        public int[] Todays { get; set; }

        public string[] images { get; set; }

        public ulong ViewCount { get; set; }

        public ShowComment[] showComments { get; set; }
    }

    public class ShowCommentViewModel { 
        
        public int showCommentId { get; set; }
        
        public string userName { get; set; }

        public string userComment { get; set; }

        public int userStar { get; set; }

        public string PicImg { get; set; }

        public DateTime? CommentDate { get; set; }

        public string CompanyName { get; set; }

        public string CompanyComment { get; set; }

        public DateTime? ResponseDate { get; set; }

    }

    public class ShowPageEditViewModel
    {
        public int Id { get; set; }

        [DisplayName("展演主題：")]
        [Required(ErrorMessage = "請填寫展演主題")]
        [StringLength(30,ErrorMessage = "展演主題不得超過30個字")]
        public string Title { get; set; }

        [AllowHtml]
        [Required(ErrorMessage = "請填寫展演內容")]
        [DisplayName("展演內容：")]
        public string Description { get; set; }

        [Required(ErrorMessage = "請填寫開放日期")]
        [DisplayName("開放日期")]
        [DataType(DataType.Date,ErrorMessage = "請填寫有效日期")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "請填寫結束日期")]
        [DisplayName("結束日期")]
        [DataType(DataType.Date, ErrorMessage = "請填寫有效日期")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "請填寫開放時間")]
        [DisplayName("開放時間")]
        [DataType(DataType.Time, ErrorMessage = "請填寫有效時間")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "請填寫結束時間")]
        [DisplayName("結束時間")]
        [DataType(DataType.Time, ErrorMessage = "請填寫有效時間")]
        public DateTime EndTime { get; set; }

        [DisplayName("是否收費？")]
        public bool Cost { get; set; }

        [DisplayName("收費金額($TW)")]
        public int Price { get; set; }

        [DisplayName("未滿18可觀看？")]
        public bool AgeRange { get; set; }

        [Required(ErrorMessage = "請填寫地址")]
        [StringLength(255, ErrorMessage = "地址不得超過255個字")]
        [DisplayName("地址")]
        public string Address { get; set; }

        [StringLength(255, ErrorMessage = "備註不得超過255個字")]
        [DisplayName("備註")]
        public string Remark { get; set; }

        [DisplayName("縣市")]
        public int FK_City { get; set; }

        [DisplayName("鄉鎮市區")]
        public int FK_District { get; set; }

        [DisplayName("關鍵字")]
        public string searchKeyword { get; set; }

        [Required(ErrorMessage = "請選擇展出時段")]
        [DisplayName("展出時段")]
        public int[] Todays { get; set; }
    }
}