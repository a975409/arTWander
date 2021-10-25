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
    [Authorize]
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
            //IndexViewModel model = (IndexViewModel)TempData["model"];
            //TempData.Keep("model");
            //取得登入的用戶Id
            int userId = User.Identity.GetUserId<int>();
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId<int>());

            //透過DbContext讀取Company資料表資料
            var company = DbContext.Company.Where(m => m.FK_ApplicationUser == userId).FirstOrDefault() ?? new Company();

            string CityName = City_District.getCities(DbContext).Where(m => m.Id == company.FK_City).Select(m => m.CityName).FirstOrDefault();

            string DistrictName = "";
            var districts = await City_District.getDistricts(company.FK_City, DbContext);

            if (districts != null)
                DistrictName = districts.Where(m => m.Id == company.FK_District).Select(m => m.DistrictName).FirstOrDefault();


            var viewModel = new CompanyViewModel
            {
                Address = $"{CityName}{DistrictName}{company.Address}",
                CompanyDescription = company.CompanyDescription,
                CompanyName = company.CompanyName,
                Email = company.Email,
                BusinessHours = company.BusinessHours,
                Fax = company.Fax,
                HomePage = company.HomePage,
                Phone = company.Phone,
                PhotoSticker = company.PhotoSticker,
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
            var company = DbContext.Company.Where(m => m.FK_ApplicationUser == userId).FirstOrDefault();
            var model = new CompanyEditViewModel();

            if (company != null)
            {
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

                ViewBag.PhotoSticker = company.PhotoSticker;
                ViewBag.PromotionalImage = company.PromotionalImage;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(CompanyEditViewModel model, HttpPostedFileBase Promotional, HttpPostedFileBase PhotoSticker)
        {
            if (!ModelState.IsValid)
                return View(model);

            int userId = User.Identity.GetUserId<int>();
            var company = DbContext.Company.Where(m => m.FK_ApplicationUser == userId).FirstOrDefault();

            string saveDir = Path.Combine(Server.MapPath("~/SaveFiles/Company"), userId.ToString(), "Info");
            Directory.CreateDirectory(saveDir);
            
            if (Promotional != null && Promotional.ContentLength > 0)
            {
                byte[] ImageData = new byte[Promotional.ContentLength];
                Promotional.InputStream.Read(ImageData, 0, Promotional.ContentLength);
                MemoryStream stream = new MemoryStream(ImageData);

                //判斷上傳的檔案是否為圖片檔
                if (IsImage(stream))
                {
                    //取得副檔名
                    string extension = Path.GetExtension(Promotional.FileName);
                    
                    //設定檔名
                    string fileName = DateTime.Now.ToString("yyyyMMdd_HHmmss") + extension;

                    //完整另存路徑
                    string savePath = Path.Combine(saveDir, fileName);

                    //server端下載檔案
                    Promotional.SaveAs(savePath);
                }
            }

            if (PhotoSticker != null && PhotoSticker.ContentLength > 0)
            {
                byte[] ImageData = new byte[PhotoSticker.ContentLength];
                PhotoSticker.InputStream.Read(ImageData, 0, PhotoSticker.ContentLength);
                MemoryStream stream = new MemoryStream(ImageData);

                if (IsImage(stream))
                {
                    //取得副檔名
                    string extension = Path.GetExtension(PhotoSticker.FileName);

                    //設定檔名
                    string fileName = DateTime.Now.ToString("yyyyMMdd_HHmmss") + extension;

                    //完整另存路徑
                    string savePath = Path.Combine(saveDir, fileName);

                    //server端下載檔案
                    Promotional.SaveAs(savePath);
                }
            }

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
                    Phone = model.Phone,
                    PromotionalImage = Promotional?.FileName,
                    PhotoSticker = PhotoSticker?.FileName
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
                company.PromotionalImage = Promotional?.FileName;
                company.PhotoSticker = PhotoSticker?.FileName;
            }
            await DbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public ActionResult GetCities()
        {
            ViewCity[] cities = City_District.getCities(DbContext);

            return Json(cities, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetDistricts(int cityId)
        {
            var districts = await City_District.getDistricts(cityId, DbContext);

            return Json(districts, JsonRequestBehavior.AllowGet);
        }

        private bool IsImage(Stream stream)
        {
            try
            {
                System.Drawing.Image img = System.Drawing.Image.FromStream(stream);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}