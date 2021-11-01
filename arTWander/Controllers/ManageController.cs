using arTWander.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace arTWander.Controllers
{
  [Authorize]
  public class ManageController : Controller
  {
    public ManageController()
    {
    }

    public ManageController(ApplicationUserManager userManager)
    {
      UserManager = userManager;
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

    //
    // GET: /Account/Index
    public async Task<ActionResult> Index(ManageMessageId? message)
    {
      ViewBag.StatusMessage =
          message == ManageMessageId.ChangePasswordSuccess ? "已變更您的密碼。"
          : message == ManageMessageId.SetPasswordSuccess ? "已設定您的密碼。"
          : message == ManageMessageId.SetTwoFactorSuccess ? "已設定您的雙因素驗證。"
          : message == ManageMessageId.Error ? "發生錯誤。"
          : message == ManageMessageId.AddPhoneSuccess ? "已新增您的電話號碼。"
          : message == ManageMessageId.RemovePhoneSuccess ? "已移除您的電話號碼。"
          : "";

      var model = new IndexViewModel
      {
        HasPassword = HasPassword(),
        PhoneNumber = await UserManager.GetPhoneNumberAsync(User.Identity.GetUserId<int>()),
        TwoFactor = await UserManager.GetTwoFactorEnabledAsync(User.Identity.GetUserId<int>()),
        Logins = await UserManager.GetLoginsAsync(User.Identity.GetUserId<int>()),
        BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(User.Identity.GetUserId<int>().ToString())
      };

      //var r = new ManageLoginsViewModel();
      //r.CurrentLogins

      var RoleName = await UserManager.GetRolesAsync(User.Identity.GetUserId<int>());

      TempData["model"] = model;

      switch (RoleName[0])
      {
        case "Admin":
          //return RedirectToAction("Index", "Admin");
          string success = SweetAlert.timeoutCloseToLinkAlert(3000, Url.Action("Index", "Admin")) + SweetAlert.SuccessAlert("登入成功", "3秒後自動跳轉到首頁", "");
          return JavaScript(success);
        case "Company":
          success = SweetAlert.timeoutCloseToLinkAlert(3000, Url.Action("Index", "Home")) + SweetAlert.SuccessAlert("登入成功", "3秒後自動跳轉到首頁", "");
          return JavaScript(success);
        case "Member":
          success = SweetAlert.timeoutCloseToLinkAlert(3000, Url.Action("Index", "Home")) + SweetAlert.SuccessAlert("登入成功", "3秒後自動跳轉到首頁", "");
          return JavaScript(success);
        default:
          return View("Error");
      }
      //return View(model);
    }

    //
    // GET: /Account/RemoveLogin
    public ActionResult RemoveLogin()
    {
      var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId<int>());
      ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
      return View(linkedAccounts);
    }

    //
    // POST: /Manage/RemoveLogin
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
    {
      ManageMessageId? message;
      var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId<int>(), new UserLoginInfo(loginProvider, providerKey));
      if (result.Succeeded)
      {
        var user = await UserManager.FindByIdAsync(User.Identity.GetUserId<int>());
        if (user != null)
        {
          await SignInAsync(user, isPersistent: false);
        }
        message = ManageMessageId.RemoveLoginSuccess;
      }
      else
      {
        message = ManageMessageId.Error;
      }
      return RedirectToAction("ManageLogins", new { Message = message });
    }

    //
    // GET: /Account/AddPhoneNumber
    public ActionResult AddPhoneNumber()
    {
      return View();
    }

    //
    // POST: /Account/AddPhoneNumber
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
    {
      if (!ModelState.IsValid)
      {
        return View(model);
      }
      // Generate the token and send it
      var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId<int>(), model.Number);
      if (UserManager.SmsService != null)
      {
        var message = new IdentityMessage
        {
          Destination = model.Number,
          Body = "您的安全碼為: " + code
        };
        await UserManager.SmsService.SendAsync(message);
      }
      return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
    }

    //
    // POST: /Manage/RememberBrowser
    [HttpPost]
    public ActionResult RememberBrowser(IndexViewModel model)
    {
      var rememberBrowserIdentity = AuthenticationManager.CreateTwoFactorRememberBrowserIdentity(User.Identity.GetUserId<int>().ToString());
      AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = true }, rememberBrowserIdentity);
      model.BrowserRemembered = true;
      //return RedirectToAction("Index");
      return PartialView("_IsRememberBrowser", model);
    }

    //
    // POST: /Manage/ForgetBrowser
    [HttpPost]
    public ActionResult ForgetBrowser(IndexViewModel model)
    {
      AuthenticationManager.SignOut(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);
      model.BrowserRemembered = false;
      //return RedirectToAction("Index");
      return PartialView("_IsRememberBrowser", model);
    }

    //
    // POST: /Manage/EnableTFA
    [HttpPost]
    public async Task<ActionResult> EnableTFA(IndexViewModel model)
    {
      await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId<int>(), true);
      var user = await UserManager.FindByIdAsync(User.Identity.GetUserId<int>());
      if (user != null)
      {
        await SignInAsync(user, isPersistent: false);
      }
      model.TwoFactor = true;
      //return RedirectToAction("Index", "Manage");
      return PartialView("_IsTwoFactor", model);
    }

    //
    // POST: /Manage/DisableTFA
    [HttpPost]
    public async Task<ActionResult> DisableTFA(IndexViewModel model)
    {
      await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId<int>(), false);
      var user = await UserManager.FindByIdAsync(User.Identity.GetUserId<int>());
      if (user != null)
      {
        await SignInAsync(user, isPersistent: false);
      }
      model.TwoFactor = false;
      return PartialView("_IsTwoFactor", model);
      //return RedirectToAction("Index", "Manage");
    }

    //
    // GET: /Account/VerifyPhoneNumber
    public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
    {
      // This code allows you exercise the flow without actually sending codes
      // For production use please register a SMS provider in IdentityConfig and generate a code here.
      var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId<int>(), phoneNumber);
      ViewBag.Status = "For DEMO purposes only, the current code is " + code;

      // 透過 SMS 提供者傳送 SMS，以驗證電話號碼
      return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
    }

    //
    // POST: /Account/VerifyPhoneNumber
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
    {
      if (!ModelState.IsValid)
      {
        return View(model);
      }
      var result = await UserManager.ChangePhoneNumberAsync(User.Identity.GetUserId<int>(), model.PhoneNumber, model.Code);
      if (result.Succeeded)
      {
        var user = await UserManager.FindByIdAsync(User.Identity.GetUserId<int>());
        if (user != null)
        {
          await SignInAsync(user, isPersistent: false);
        }
        return RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
      }
      // 如果執行到這裡，發生某項失敗，則重新顯示表單
      ModelState.AddModelError("", "Failed to verify phone");
      return View(model);
    }

    //
    // GET: /Account/RemovePhoneNumber
    public async Task<ActionResult> RemovePhoneNumber()
    {
      var result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId<int>(), null);
      if (!result.Succeeded)
      {
        return RedirectToAction("Index", new { Message = ManageMessageId.Error });
      }
      var user = await UserManager.FindByIdAsync(User.Identity.GetUserId<int>());
      if (user != null)
      {
        await SignInAsync(user, isPersistent: false);
      }
      return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
    }

    //
    // GET: /Manage/ChangePassword
    public ActionResult ChangePassword()
    {
      return View();
    }

    //
    // POST: /Account/Manage
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
    {
      if (!ModelState.IsValid)
      {
        //return View(model);
        string validAlert = SweetAlert.initAlert() + SweetAlert.ErrorAlert("變更密碼失敗", "欄位驗證失敗!", "");
        return JavaScript(validAlert);
      }
      var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId<int>(), model.OldPassword, model.NewPassword);
      if (result.Succeeded)
      {
        var user = await UserManager.FindByIdAsync(User.Identity.GetUserId<int>());
        if (user != null)
        {
          await SignInAsync(user, isPersistent: false);
        }
        //return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
        string success = SweetAlert.timeoutCloseToLinkAlert(3000, Url.Action("Index", "Manage")) + SweetAlert.SuccessAlert("變更密碼成功", "3秒後自動跳轉到展演單位主頁", "");
        return JavaScript(success);
      }

      //AddErrors(result);
      //return View(model);
      string loginFailure = SweetAlert.initAlert() + SweetAlert.ErrorAlert("變更密碼失敗", result.Errors.FirstOrDefault(), "");
      return JavaScript(loginFailure);
    }

    //
    // GET: /Manage/SetPassword
    public ActionResult SetPassword()
    {
      return View();
    }

    //
    // POST: /Manage/SetPassword
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
    {
      if (ModelState.IsValid)
      {
        var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId<int>(), model.NewPassword);
        if (result.Succeeded)
        {
          var user = await UserManager.FindByIdAsync(User.Identity.GetUserId<int>());
          if (user != null)
          {
            await SignInAsync(user, isPersistent: false);
          }
          return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
        }
        AddErrors(result);
      }

      // 如果執行到這裡，發生某項失敗，則重新顯示表單
      return View(model);
    }

    //
    // GET: /Account/Manage
    public async Task<ActionResult> ManageLogins(ManageMessageId? message)
    {
      ViewBag.StatusMessage =
          message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
          : message == ManageMessageId.Error ? "An error has occurred."
          : "";
      var user = await UserManager.FindByIdAsync(User.Identity.GetUserId<int>());
      if (user == null)
      {
        return View("Error");
      }
      var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId<int>());
      var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
      ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
      return View(new ManageLoginsViewModel
      {
        CurrentLogins = userLogins,
        OtherLogins = otherLogins
      });
    }

    //
    // POST: /Manage/LinkLogin
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult LinkLogin(string provider)
    {
      // 要求重新導向至外部登入提供者，以連結目前使用者的登入
      return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId<int>().ToString());
    }

    //
    // GET: /Manage/LinkLoginCallback
    public async Task<ActionResult> LinkLoginCallback()
    {
      var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId<int>().ToString());
      if (loginInfo == null)
      {
        return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
      }
      var result = await UserManager.AddLoginAsync(User.Identity.GetUserId<int>(), loginInfo.Login);
      return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
    }

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

    private async Task SignInAsync(ApplicationUser user, bool isPersistent)
    {
      AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.TwoFactorCookie);
      AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, await user.GenerateUserIdentityAsync(UserManager));
    }

    private void AddErrors(IdentityResult result)
    {
      foreach (var error in result.Errors)
      {
        ModelState.AddModelError("", error);
      }
    }

    private bool HasPassword()
    {
      var user = UserManager.FindById(User.Identity.GetUserId<int>());
      if (user != null)
      {
        return user.PasswordHash != null;
      }
      return false;
    }

    private bool HasPhoneNumber()
    {
      var user = UserManager.FindById(User.Identity.GetUserId<int>());
      if (user != null)
      {
        return user.PhoneNumber != null;
      }
      return false;
    }

    public enum ManageMessageId
    {
      AddPhoneSuccess,
      ChangePasswordSuccess,
      SetTwoFactorSuccess,
      SetPasswordSuccess,
      RemoveLoginSuccess,
      RemovePhoneSuccess,
      Error
    }

    #endregion
  }
}