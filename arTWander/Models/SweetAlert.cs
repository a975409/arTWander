using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace arTWander.Models
{
    /// <summary>
    /// 參考網站 https://sweetalert2.github.io/#allowOutsideClick
    /// </summary>
    public static class SweetAlert
    {
        /// <summary>
        /// 自定義sweetalert的樣式＆動作方法
        /// </summary>
        /// <param name="timeout">alert停留時間</param>
        /// <param name="link">關閉alert後跳轉到的網址</param>
        /// <returns></returns>
        public static string timeoutCloseToLinkAlert(int timeout,string link)
        {
            string msg = "const Toast = Swal.mixin({timer:"+ timeout + ",timerProgressBar: true,didClose: (toast) => {location.href = ('"+ link + "');}});";

            return msg;
        }

        /// <summary>
        /// sweetalert的預設樣式
        /// </summary>
        /// <returns></returns>
        public static string initAlert() {

            string msg = "const Toast = Swal.mixin();";

            return msg;
        }
        /// <summary>
        /// 錯誤訊息，單個Cancel按鈕
        /// </summary>
        /// <param name="title">標題</param>
        /// <param name="text">內文</param>
        /// <param name="footer">底部內文</param>
        /// <returns></returns>
        public static string ErrorAlert(string title, string text, string footer)
        {
            string msg = "Toast.fire({icon: 'error',title: '" + title + "',text: '" + text + "',footer: '" + footer + "',showConfirmButton:false,showCancelButton:true});";

            return msg;
        }

        /// <summary>
        /// 成功訊息，沒有按鈕
        /// </summary>
        /// <param name="title">標題</param>
        /// <param name="text">內文</param>
        /// <param name="footer">底部內文</param>
        /// <returns></returns>
        public static string SuccessAlert(string title, string text, string footer)
        {
            string msg = "Toast.fire({icon:'success',title:'" + title + "',text:'" + text + "',showConfirmButton: false,footer:'" + footer + "'});";

            return msg;
        }
    }
}