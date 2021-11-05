using arTWander.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Microsoft.Owin.Security;
using System.IO;

namespace arTWander.Controllers
{
    [Authorize(Roles = "Company")]
    public class CompanyController : Controller
    {
        //如果要在其他Controller引用DbContext來對資料表做CRUD，請參考第17~33行新增DbContext
        //詳細用法請參考底下 Index 這個 Action
        public CompanyController()
        {
        }

        public CompanyController(ApplicationUserManager userManager, ApplicationDbContext dbContext)
        {
            UserManager = userManager;
            DbContext = dbContext;
        }

        private ApplicationDbContext _dbContext;
        public ApplicationDbContext DbContext
        {
            get
            {
                return _dbContext ?? HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            }
            private set
            {
                _dbContext = value;
            }
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

        // GET: Company
        public async Task<ActionResult> Index()
        {
            //取得登入的用戶Id
            int userId = User.Identity.GetUserId<int>();

            //透過DbContext讀取Company資料表資料
            var company = new CompanyFactory(DbContext).GetCompany(userId);

            //如果沒有輸入展演單位資訊，就會跳到展演單位資訊編輯畫面
            if (company == null)
                return RedirectToAction("Edit");

            string address = await new CompanyFactory(DbContext).GetFullAddress(company.FK_City, company.FK_District, company.Address);

            string dirPath = $"/SaveFiles/Company/{company.Id}/Info";

            var viewModel = new CompanyViewModel
            {
                Id = company.Id,
                Address = address,
                CompanyDescription = company.CompanyDescription,
                CompanyName = company.CompanyName,
                Email = company.Email,
                BusinessHours = company.BusinessHours,
                Fax = company.Fax,
                HomePage = company.HomePage,
                Phone = company.Phone,
                PhotoSticker = company.PhotoStickerImage,
                PromotionalImage = company.PromotionalImage,
                HasPassword = HasPassword(),
                PhoneNumber = await UserManager.GetPhoneNumberAsync(User.Identity.GetUserId<int>()),
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(User.Identity.GetUserId<int>()),
                Logins = await UserManager.GetLoginsAsync(User.Identity.GetUserId<int>()),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(User.Identity.GetUserId<int>().ToString())
            };
            return View(viewModel);
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

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        public ActionResult Edit()
        {
            int userId = User.Identity.GetUserId<int>();
            var company = new CompanyFactory(DbContext).GetCompany(userId);
            var model = new CompanyEditViewModel();
            
            if (company != null)
            {
                string dirPath = $"/SaveFiles/Company/{company.Id}/Info";
                model.Id = company.Id;
                model.Address = company.Address;
                model.BusinessHours = company.BusinessHours;
                model.CompanyDescription = company.CompanyDescription;
                model.CompanyName = company.CompanyName;
                model.Email = company.Email;
                model.Fax = company.Fax;
                model.FK_City = company.FK_City;
                model.FK_District = company.FK_District;
                model.HomePage = company.HomePage;
                model.Phone = company.Phone;
                model.PhotoSticker = company.PhotoStickerImage;
                model.PromotionalImage = company.PromotionalImage;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(CompanyEditViewModel model, HttpPostedFileBase Promotional, HttpPostedFileBase PhotoSticker)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            CompanyFactory factory = new CompanyFactory(DbContext);

            string temp = model.CompanyDescription.ToLower();

            if (temp.Contains("<script>") || temp.Contains("</script>"))
            {
                ModelState.AddModelError("Description", "該欄位疑似有出現不安全的程式碼");
                return View(model);
            }

            int userId = User.Identity.GetUserId<int>();
            var company = factory.GetCompany(userId);

            if (company == null)
            {
                company = new Company
                {
                    FK_ApplicationUser = userId,
                    Address = model.Address,
                    BusinessHours = model.BusinessHours,
                    CompanyDescription = model.CompanyDescription,
                    CompanyName = model.CompanyName,
                    Email = model.Email,
                    Fax = model.Fax,
                    FK_City = model.FK_City,
                    FK_District = model.FK_District,
                    HomePage = model.HomePage,
                    Phone = model.Phone
                };
                DbContext.Company.Add(company);
            }
            else
            {
                company.FK_ApplicationUser = userId;
                company.Address = model.Address;
                company.BusinessHours = model.BusinessHours;
                company.CompanyDescription = model.CompanyDescription;
                company.CompanyName = model.CompanyName;
                company.Email = model.Email;
                company.Fax = model.Fax;
                company.FK_City = model.FK_City;
                company.FK_District = model.FK_District;
                company.HomePage = model.HomePage;
                company.Phone = model.Phone;
            }
            await DbContext.SaveChangesAsync();

            string saveDir = Path.Combine(Server.MapPath("~/SaveFiles/Company"), company.Id.ToString(), "Info");
            Directory.CreateDirectory(saveDir);

            //設定宣傳圖
            if (Promotional != null)
            {
                company.PromotionalImage = "PromotionalImage" + Path.GetExtension(Promotional.FileName);

                if (factory.SaveCompanyPageImage(Promotional, saveDir, company.PromotionalImage))
                {
                    await DbContext.SaveChangesAsync();
                }
            }

            //設定大頭照
            if (PhotoSticker != null) {
                company.PhotoStickerImage = "PhotoStickerImg" + Path.GetExtension(PhotoSticker.FileName);

                if (factory.SaveCompanyPageImage(PhotoSticker, saveDir, company.PhotoStickerImage))
                {
                    await DbContext.SaveChangesAsync();
                }
            }

            return RedirectToAction("Index");
        }
    }
}