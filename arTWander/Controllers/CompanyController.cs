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
            //IndexViewModel model = (IndexViewModel)TempData["model"];
            //TempData.Keep("model");
            //取得登入的用戶Id
            int userId = User.Identity.GetUserId<int>();
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId<int>());

            //透過DbContext讀取Company資料表資料
            var company = DbContext.Company.Where(m => m.FK_ApplicationUser == userId).FirstOrDefault();


            //如果沒有輸入展演單位資訊，就會跳到展演單位資訊編輯畫面
            if (company == null)
                return RedirectToAction("Edit");

            string CityName = City_District.getCities(DbContext).Where(m => m.Id == company.FK_City).Select(m => m.CityName).FirstOrDefault();

            string DistrictName = "";
            var districts = await City_District.getDistricts(company.FK_City, DbContext);

            if (districts != null)
                DistrictName = districts.Where(m => m.Id == company.FK_District).Select(m => m.DistrictName).FirstOrDefault();

            string dirPath = $"/SaveFiles/Company/{company.Id}/Info";

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
                PhotoSticker = string.IsNullOrEmpty(company.PhotoStickerImage) ? "" : $"{dirPath}/{company.PhotoStickerImage}",
                PromotionalImage = string.IsNullOrEmpty(company.PromotionalImage) ? "" : $"{dirPath}/{company.PromotionalImage}",
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
                string dirPath = $"/SaveFiles/Company/{company.Id}/Info";
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

                ViewBag.PhotoSticker = $"{dirPath}/{company.PhotoStickerImage}";
                ViewBag.PromotionalImage = $"{dirPath}/{company.PromotionalImage}";
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

            int userId = User.Identity.GetUserId<int>();
            var company = DbContext.Company.Where(m => m.FK_ApplicationUser == userId).FirstOrDefault();

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

            if (Promotional != null && Promotional.ContentLength > 0)
            {
                byte[] ImageData = new byte[Promotional.ContentLength];
                Promotional.InputStream.Read(ImageData, 0, Promotional.ContentLength);

                using (MemoryStream stream = new MemoryStream(ImageData))
                {
                    //判斷上傳的檔案是否為圖片檔
                    if (IsImage(stream))
                    {
                        //移除原先儲存在Server上的封面圖
                        var PromotionalImages = Directory.GetFiles(saveDir).Where(m => Path.GetFileNameWithoutExtension(m) == "PromotionalImage");

                        foreach(var item in PromotionalImages)
                        {
                            FileInfo info = new FileInfo(item);

                            if (info.Exists)
                            {
                                try
                                {
                                    info.Delete();
                                }
                                catch { }
                            }
                        }

                        //取得副檔名
                        string extension = Path.GetExtension(Promotional.FileName);

                        //設定檔名
                        company.PromotionalImage = "PromotionalImage" + extension;

                        //完整另存路徑
                        string savePath = Path.Combine(saveDir, company.PromotionalImage);

                        //server端下載檔案
                        Promotional.SaveAs(savePath);
                        await DbContext.SaveChangesAsync();
                    }
                }
            }

            if (PhotoSticker != null && PhotoSticker.ContentLength > 0)
            {
                byte[] ImageData = new byte[PhotoSticker.ContentLength];
                PhotoSticker.InputStream.Read(ImageData, 0, PhotoSticker.ContentLength);

                using (MemoryStream stream = new MemoryStream(ImageData))
                {
                    if (IsImage(stream))
                    {
                        //移除原先儲存在Server上的大頭照
                        var PhotoStickerImages = Directory.GetFiles(saveDir).Where(m => Path.GetFileNameWithoutExtension(m) == "PhotoStickerImg");

                        foreach (var item in PhotoStickerImages)
                        {
                            FileInfo info = new FileInfo(item);

                            if (info.Exists)
                            {
                                try
                                {
                                    info.Delete();
                                }
                                catch { }
                            }
                        }

                        //取得副檔名
                        string extension = Path.GetExtension(PhotoSticker.FileName);

                        //設定檔名
                        company.PhotoStickerImage = "PhotoStickerImg" + extension;

                        //完整另存路徑
                        string savePath = Path.Combine(saveDir, company.PhotoStickerImage);

                        //server端下載檔案
                        PhotoSticker.SaveAs(savePath);
                        await DbContext.SaveChangesAsync();
                    }
                }
            }

            return RedirectToAction("Index");
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