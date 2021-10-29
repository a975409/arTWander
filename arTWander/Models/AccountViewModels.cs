using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace arTWander.Models
{
    //public class ExternalLoginConfirmationViewModel
    //{
    //    [Required]
    //    [Display(Name = "電子郵件")]
    //    public string Email { get; set; }
    //}

    //public class ExternalLoginListViewModel
    //{
    //    public string ReturnUrl { get; set; }
    //}


    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "驗證碼")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "記住此瀏覽器?")]
        public bool RememberBrowser { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    //public class AccountViewModel
    //{
    //    public class Login
    //    {
    //        [Required]
    //        [Display(Name = "Email")]
    //        [EmailAddress(ErrorMessage = "請輸入正確的Email")]
    //        public string Email { get; set; }

    //        [Required]
    //        [DataType(DataType.Password)]
    //        [Display(Name = "密碼")]
    //        public string Password { get; set; }

    //        [Display(Name = "記住帳號")]
    //        public bool RememberMe { get; set; }
    //    }

    //    public class Register
    //    {
    //        [Required]
    //        [EmailAddress]
    //        [Display(Name = "電子信箱")]
    //        public string Email { get; set; }

    //        [Required]
    //        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    //        [DataType(DataType.Password)]
    //        [Display(Name = "密碼")]
    //        public string Password { get; set; }

    //        [DataType(DataType.Password)]
    //        [Display(Name = "確認密碼")]
    //        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    //        public string ConfirmPassword { get; set; }

    //        [Display(Name = "用戶權限")]
    //        public string AccountRoles { get; set; }
    //    }
    //}

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage ="請輸入正確的Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密碼")]
        public string Password { get; set; }

        [Display(Name = "記住帳號")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "電子信箱")]
        public string Email { get; set; }

        //[Required]
        //[StringLength(20, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        //[Display(Name = "使用者名稱")]
        //public string UserName { get; set; }

        [Required]
        [RegularExpression("^((?=.{8,}$)(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).*|(?=.{8,}$)(?=.*\\d)(?=.*[a-zA-Z])(?=.*[!\u0022#$%&'()*+,./:;<=>?@'[\\]\\^_`{|}~-]).*)", ErrorMessage = "設定的密碼不符合規範")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "密碼")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "確認密碼")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "用戶權限")]
        public string AccountRoles { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [RegularExpression("^((?=.{8,}$)(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).*|(?=.{8,}$)(?=.*\\d)(?=.*[a-zA-Z])(?=.*[!\u0022#$%&'()*+,./:;<=>?@'[\\]\\^_`{|}~-]).*)",ErrorMessage ="設定的密碼不符合規範")]
        [Required(ErrorMessage ="請填寫密碼")]
        [StringLength(100, ErrorMessage = "{0}至少設定 {2} 以上", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "密碼")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "確認密碼")]
        [Compare("Password", ErrorMessage = "輸入的密碼不一致")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage ="請輸入Email")]
        [EmailAddress(ErrorMessage = "請輸入有效的Email")]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}