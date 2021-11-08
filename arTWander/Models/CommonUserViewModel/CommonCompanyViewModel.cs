using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace arTWander.Models
{
    public class CommonCompanyViewModel
    {

        //展演單位資訊
        [DisplayName("Id")]
        public int Id { get; set; }

        [DisplayName("展演單位名稱")]
        public string CompanyName { get; set; }

        [DisplayName("展演單位簡介")]
        public string CompanyDescription { get; set; }

        [DisplayName("地址")]
        public string Address { get; set; }

        [DisplayName("縣市")]
        public string CompanyCity { get; set; }

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

        //展演資訊

        [DisplayName("展演Id")]
        public int[] ShowId { get; set; }

        [DisplayName("展演地區")]
        public string[] ShowCity { get; set; }

        [DisplayName("Title")]
        public string[] ShowTitle { get; set; }

        [DisplayName("展演描述")]
        public string[] ShowDiscription { get; set; }

        [DisplayName("展演公司")]
        public string ShowCompany { get; set; }

        [DisplayName("展演圖")]
        public string[] ShowImg { get; set; }


        [DisplayName("圖檔路徑")]
        public string Path { get; set; }

        public List<City> AllCity { get; set; }

        public List<CommonCompanyViewModel> AllCompany { get; set; }
    }
}