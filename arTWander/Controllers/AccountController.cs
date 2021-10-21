using System.Globalization;
using arTWander.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace arTWander.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //登入＆註冊畫面
        [AllowAnonymous]
        public ActionResult AccountIndex()
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage", null);
            }

            //讀取cookie裡面的帳號
            HttpCookie cookie = Request.Cookies["Email"];
            if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
            {
                ViewBag.Email = cookie.Value;
                ViewBag.RememberMe = true;
            }
            else
            {
                ViewBag.Email = (string)TempData["Email"];
                //TempData.Keep("Email");
            }

            return View();
        }


        // GET: /Account/Login
        //[AllowAnonymous]
        //public ActionResult Login(string returnUrl)
        //{
        //    LoginViewModel model = new LoginViewModel();

        //    //讀取cookie裡面的帳號
        //    HttpCookie cookie = Request.Cookies["Email"];
        //    if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
        //    {
        //        model.Email = cookie.Value;
        //        model.RememberMe = true;
        //    }

        //    ViewBag.ReturnUrl = returnUrl;
        //    return View(model);
        //}

        private ApplicationSignInManager _signInManager;

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set { _signInManager = value; }
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            //寫入帳號至cookie
            HttpCookie cookie = Request.Cookies["Email"];

            if (model.RememberMe)
            {
                if (cookie == null)
                {
                    cookie = new HttpCookie("Email");
                    cookie.Value = model.Email;
                    cookie.Expires = DateTime.Now.AddMonths(1);
                    Response.Cookies.Add(cookie);
                }
                else
                {
                    cookie.Value = model.Email;
                    Response.Cookies.Set(cookie);
                }
            }

            //if (!ModelState.IsValid)
            //{
            //    TempData["Email"] = model.Email;
            //    TempData["LoginPage"] = true;
            //    TempData["Status"] = "登入失敗";
            //    TempData["DialogMsg"] = "欄位驗證失敗，請檢查所有欄位是否已填寫!<br><br>";
            //    //return RedirectToAction("AccountIndex");
            //    return View(model);
            //}

            var usermanger = UserManager.FindByEmail(model.Email);
            // 這不會計算為帳戶鎖定的登入失敗
            // 若要啟用密碼失敗來觸發帳戶鎖定，請變更為 shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            
            switch (result)
            {
                case SignInStatus.Success:
                    //檢查該帳號是否有做mail驗證
                    if (!UserManager.IsEmailConfirmed(usermanger.Id))
                    {
                        string error = SweetAlert.initAlert() + SweetAlert.ErrorAlert("登入失敗", "此帳號的信箱尚未驗證，請驗證後再登入!", "");
                        return JavaScript(error);
                    }
                    else
                    {
                        //登入成功
                        //return RedirectToLocal(returnUrl);
                        //return RedirectToAction("Index", "Home");
                        string success = SweetAlert.timeoutCloseToLinkAlert(3000, Url.Action("Index", "Home")) + SweetAlert.SuccessAlert("登入成功", "3秒後自動跳轉到首頁", "");
                        return JavaScript(success);
                    }
                case SignInStatus.LockedOut:
                    //return View("Lockout");
                    string LockedOut = SweetAlert.initAlert() + SweetAlert.ErrorAlert("登入失敗", "該用戶已被鎖定，請稍後再試", "");
                    return JavaScript(LockedOut);
                case SignInStatus.RequiresVerification:
                    //return RedirectToAction("SendCode", new { ReturnUrl = returnUrl });
                    string Verification = SweetAlert.timeoutCloseToLinkAlert(3000, Url.Action("SendCode", "Account", new { ReturnUrl = returnUrl })) + SweetAlert.SuccessAlert("請稍後", "3秒後自動跳轉到驗證畫面", "");
                    return JavaScript(Verification);
                case SignInStatus.Failure:
                default:
                    //傳遞Model=> new RouteValueDictionary(model)
                    //return RedirectToAction("AccountIndex", new RouteValueDictionary(model));
                    string loginFailure = SweetAlert.initAlert() + SweetAlert.ErrorAlert("登入失敗", "請確認您輸入的帳號密碼是否正確!", "");
                    return JavaScript(loginFailure);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl)
        {
            // 需要使用者已透過使用者名稱/密碼或外部登入進行登入
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }

            var user = await UserManager.FindByIdAsync(await SignInManager.GetVerifiedUserIdAsync());
            if (user != null)
            {
                ViewBag.Status = "For DEMO purposes the current " + provider + " code is: " + await UserManager.GenerateTwoFactorTokenAsync(user.Id, provider);
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // 下列程式碼保護兩個因素碼不受暴力密碼破解攻擊。 
            // 如果使用者輸入不正確的代碼來表示一段指定的時間，則使用者帳戶 
            // 會有一段指定的時間遭到鎖定。 
            // 您可以在 IdentityConfig 中設定帳戶鎖定設定
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: false, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    //return RedirectToLocal(model.ReturnUrl);
                    string successAlert = SweetAlert.timeoutCloseToLinkAlert(3000, Url.Action("Index", "Home")) + SweetAlert.SuccessAlert("驗證成功", "3秒後自動跳轉到首頁", "");

                    return JavaScript(successAlert);
                case SignInStatus.LockedOut:
                    //return View("Lockout");
                    string LockedOut = SweetAlert.initAlert() + SweetAlert.ErrorAlert("驗證失敗", "該用戶已被鎖定，請稍後再試", "");
                    return JavaScript(LockedOut);
                case SignInStatus.Failure:
                default:
                    //ModelState.AddModelError("", "代碼無效。");
                    //return View(model);
                    string validAlert = SweetAlert.initAlert() + SweetAlert.ErrorAlert("驗證失敗", "驗證碼無效", "");
                    return JavaScript(validAlert);
            }
        }

        //
        // GET: /Account/Register
        //[AllowAnonymous]
        //public ActionResult Register()
        //{
        //    return View("~/Views/Account/AccountIndex.cshtml");
        //}

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            //if (!ModelState.IsValid)
            //{
            //    //如果執行到這裡，發生某項失敗，則重新顯示表單
            //    //return View(model);
            //    TempData["LoginPage"] = false;
            //    TempData["Status"] = "註冊失敗";
            //    TempData["DialogMsg"] = "<p>欄位驗證失敗，請檢查所有欄位是否已填寫!</p><br><br>";
            //    return RedirectToAction("AccountIndex");
            //}

            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await UserManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // 如需如何進行帳戶確認及密碼重設的詳細資訊，請前往 https://go.microsoft.com/fwlink/?LinkID=320771
                // 傳送包含此連結的電子郵件

                var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

                //寄mail到新註冊使用者的帳戶
                await UserManager.SendEmailAsync(user.Id, "您的arTWander帳號已註冊完畢, 請盡速確認您的帳戶", "請按一下此連結確認您的帳戶 <a href=\"" + callbackUrl + "\">這裏</a> \n若您並未註冊本網站請忽略此信, 以確保您自身權益");

                UserManager.AddToRoleAsync(user.Id, model.AccountRoles);

                //await UserManager.AddToRoleAsync(user.Id,"Admin");//系統管理員
                //await UserManager.AddToRoleAsync(user.Id,"Company");//展演單位
                //await UserManager.AddToRoleAsync(user.Id,"Member");//一般會員
                //await UserManager.AddToRoleAsync(user.Id,"Blacklist");//黑名單

                //ViewBag.Link = callbackUrl;
                //return View("DisplayEmail");
                //TempData["Status"] = "註冊完成";
                //TempData["DialogMsg"] = "<p>請至信箱收驗證信</p><p>或點擊 <a href=" + callbackUrl + ">此連結</a></p><br><br>";
                //return RedirectToAction("AccountIndex");

                string success = SweetAlert.timeoutCloseToLinkAlert(0, Url.Action("Index", "Home")) + SweetAlert.SuccessAlert("註冊成功", "請至信箱收驗證信", "或點擊 <a href=" + callbackUrl + ">此連結</a>");
                return JavaScript(success);
            }
            AddErrors(result);
            //TempData["LoginPage"] = false;
            //TempData["Status"] = "註冊失敗";
            //TempData["DialogMsg"] = "<p>註冊失敗！！</p><br><br>";
            //return RedirectToAction("AccountIndex");
            string failure = SweetAlert.initAlert() + SweetAlert.ErrorAlert("註冊失敗", "欄位驗證失敗!", "");
            return JavaScript(failure);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(int userId, string code)
        {
            if (userId > 0 || code == null)
            {
                var result = await UserManager.ConfirmEmailAsync(userId, code);
                return View(result.Succeeded ? "ConfirmEmail" : "Error");
            }
            return View("Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    string alert = SweetAlert.initAlert() + SweetAlert.ErrorAlert("Email寄送失敗", "該使用者不存在或未確認", "");
                    return JavaScript(alert);
                }

                // 如需如何進行帳戶確認及密碼重設的詳細資訊，請前往 https://go.microsoft.com/fwlink/?LinkID=320771
                // 傳送包含此連結的電子郵件

                var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "您的arTWander密碼重設確認信", "請按 <a href=\"" + callbackUrl + "\">這裏</a> 重設您的密碼 \n若您並未重置您的密碼請忽略此信");

                string a_href = "或請按 <a href=\"" + callbackUrl + "\">這裏</a> 重設密碼";
                string successAlert = SweetAlert.timeoutCloseToLinkAlert(0, Url.Action("AccountIndex", "Account")) + SweetAlert.SuccessAlert("Email寄送成功", "請至Email收密碼重設信", a_href);

                return JavaScript(successAlert);
            }
            string errorMsg = ModelState["Email"].Errors[0].ErrorMessage;
            string validAlert = SweetAlert.initAlert() + SweetAlert.ErrorAlert("Email寄送失敗", errorMsg, "");
            return JavaScript(validAlert);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            TempData.Keep();
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(int userId, string code)
        {
            var user = UserManager.FindById(userId);
            ViewBag.Email = user.Email;
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    string notFind = SweetAlert.initAlert() + SweetAlert.ErrorAlert("密碼重設失敗", "該使用者不存在或未確認", "");
                    return JavaScript(notFind);
                }
                var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
                if (result.Succeeded)
                {
                    string successAlert = SweetAlert.timeoutCloseToLinkAlert(3000, Url.Action("AccountIndex", "Account")) + SweetAlert.SuccessAlert("密碼重設成功", "3秒後自動跳轉到登入畫面", "");
                    return JavaScript(successAlert);
                }
                else
                {
                    AddErrors(result);
                    string errorAlert = SweetAlert.initAlert() + SweetAlert.ErrorAlert("密碼重設失敗", "發生未知錯誤，請稍後再試", "");
                    return JavaScript(errorAlert);
                }
            }
            string validAlert = SweetAlert.initAlert() + SweetAlert.ErrorAlert("密碼重設失敗", "欄位驗證失敗，請重新輸入", "");
            return JavaScript(validAlert);
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public ActionResult ExternalLogin(string provider, string returnUrl)
        //{
        //    // 要求重新導向至外部登入提供者
        //    return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        //}

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId > 0)
            {
                var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
                var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();

                return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl });
            }
            return View("Error");
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl });
        }

        //
        // GET: /Account/ExternalLoginCallback
        //[AllowAnonymous]
        //public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        //{
        //    var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
        //    if (loginInfo == null)
        //    {
        //        return RedirectToAction("Login");
        //    }

        //    // 若使用者已經有登入資料，請使用此外部登入提供者登入使用者
        //    var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
        //    switch (result)
        //    {
        //        case SignInStatus.Success:
        //            return RedirectToLocal(returnUrl);
        //        case SignInStatus.LockedOut:
        //            return View("Lockout");
        //        case SignInStatus.RequiresVerification:
        //            return RedirectToAction("SendCode", new { ReturnUrl = returnUrl });
        //        case SignInStatus.Failure:
        //        default:
        //            // 若使用者沒有帳戶，請提示使用者建立帳戶
        //            ViewBag.ReturnUrl = returnUrl;
        //            ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
        //            return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
        //    }
        //}

        //
        // POST: /Account/ExternalLoginConfirmation
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        return RedirectToAction("Index", "Manage");
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        // 從外部登入提供者處取得使用者資訊
        //        var info = await AuthenticationManager.GetExternalLoginInfoAsync();
        //        if (info == null)
        //        {
        //            return View("ExternalLoginFailure");
        //        }
        //        var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
        //        var result = await UserManager.CreateAsync(user);
        //        if (result.Succeeded)
        //        {
        //            result = await UserManager.AddLoginAsync(user.Id, info.Login);
        //            if (result.Succeeded)
        //            {
        //                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //                return RedirectToLocal(returnUrl);
        //            }
        //        }
        //        AddErrors(result);
        //    }

        //    ViewBag.ReturnUrl = returnUrl;
        //    return View(model);
        //}

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        //[AllowAnonymous]
        //public ActionResult ExternalLoginFailure()
        //{
        //    return View();
        //}

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}