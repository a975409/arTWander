using arTWander.Models;
using arTWander.Models.CommonFactory;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static arTWander.Controllers.ManageController;
using static arTWander.Models.CommonViewModel;

namespace arTWander.Controllers
{
    public class CommonController : Controller
    {

        //引入EF dbcontext
        public CommonController()
        {
        }

        public CommonController(ApplicationUserManager userManager, ApplicationDbContext dbContext)
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

        #region Helper
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

        #endregion

        private const int pageSize = 12;

        //基本資料設定 starts
        public ActionResult SetUp(string firstLogIn)
        {
            // 取得model
            IndexViewModel model;
            if (firstLogIn == "true")
            {
                // 假如初次登入
                model = (IndexViewModel)TempData["model"];
                TempData.Keep("model");
                ViewBag.firstLogIn = "true";
            }
            else
            {
                model = new IndexViewModel
                {
                    HasPassword = HasPassword(),
                    PhoneNumber = UserManager.GetPhoneNumber(User.Identity.GetUserId<int>()),
                    TwoFactor = UserManager.GetTwoFactorEnabled(User.Identity.GetUserId<int>()),
                    Logins = UserManager.GetLogins(User.Identity.GetUserId<int>()),
                    BrowserRemembered = AuthenticationManager.TwoFactorBrowserRemembered(User.Identity.GetUserId<int>().ToString())
                };
            }

            // 取得viewModel
            int userId = User.Identity.GetUserId<int>();
            CommonInfoViewModel viewModel = new userFactory(DbContext).createViewModel(model, userId);

            // return
            return View(viewModel);

        }

        [HttpPost]
        public ActionResult SetUp(CommonInfoViewModel viewModel, HttpPostedFileBase avatarFile)
        {
            // 取得viewModel存入資料庫
            ApplicationDbContext db = new ApplicationDbContext();
            int userId = User.Identity.GetUserId<int>();
            ApplicationUser user = db.Users.Where(u => u.Id == userId).FirstOrDefault();

            // 找到舊檔案
            string delAvatar = "";

            if (user.Avatar != viewModel.AvatarName)
                delAvatar = user.Avatar;

            // 更新資料
            new userFactory(DbContext).updateToDB(user, viewModel);
            db.SaveChanges();

            // 取得檔案存入指定資料夾
            new userFactory(DbContext).saveAvatarToFolder(user, avatarFile);
            // 刪除舊檔案
            new userFactory(DbContext).DeleteAvatarFromFolder(user, delAvatar);

            // return
            return RedirectToAction("Index", "Common");
        }
        //基本資料設定 ends

        //=========================================================================================

        //Common首頁 start

        public ActionResult Index(string city = "")
        {
            // 取得使用者頭像及姓名
            int userId = User.Identity.GetUserId<int>();
            getUserByIdInfo(userId);

            SearchShowPagesViewModel search = new SearchShowPagesViewModel
            {
                Cost = CostStatus.none,
                OrderSortField = OrderSortField.DateSort
            };

            if (!string.IsNullOrEmpty(city))
                search.FK_City = DbContext.City.Where(m => m.CityName == city).Select(m => m.Id).FirstOrDefault();

            TempData["SearchModel"] = search;
            TempData.Keep("SearchModel");

            return View(new userFactory(DbContext).getShowPages(model: search));
        }

        //取得使用者名稱＆大頭照
        private void getUserByIdInfo(int userId)
        {
            ApplicationUser user = DbContext.Users.Where(u => u.Id == userId).FirstOrDefault();

            //使用者未登入
            if (user == null)
            {
                ViewBag.userName = "";
                ViewBag.avatarUrl = "/image/avatar/avatar_default.png";
                return;
            }

            string role = UserManager.GetRoles(userId)[0];

            switch (role)
            {
                case "Member":
                    ViewBag.userName = user.UserName;
                    ViewBag.avatarUrl = string.IsNullOrEmpty(user.Avatar) ? "/image/avatar/avatar_default.png" : $"/SaveFiles/Member/{user.Id}/Avatar/{user.Avatar}";
                    break;
                case "Company":
                    var company = DbContext.Company.Where(m => m.FK_ApplicationUser == userId).FirstOrDefault();

                    if (company == null)
                    {
                        ViewBag.userName = user.UserName;
                        ViewBag.PhotoStickerImage = "/image/avatar/avatar_default.png";
                    }
                    else
                    {
                        ViewBag.userName = company.CompanyName;
                        ViewBag.PhotoStickerImage = string.IsNullOrEmpty(company.PhotoStickerImage) ? "/image/exhibiton/Null.png" : $"/SaveFiles/Company/{company.Id}/Info/{company.PhotoStickerImage}";
                    }
                    break;
                default:
                    ViewBag.userName = "";
                    ViewBag.avatarUrl = "/image/avatar/avatar_default.png";
                    break;

            }
        }
        
        /// <summary>
        /// 進階（條件）搜尋展覽 & 關鍵字搜尋展覽
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="page"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult getShowList(SearchShowPagesViewModel searchModel, int page = 1, string keyword = "")
        {
            SearchShowPagesViewModel search;
            if (string.IsNullOrEmpty(keyword))
            {
                search = searchModel;
                TempData["SearchModel"] = search;
                TempData.Keep("SearchModel");
            }
            else
            {
                search = new SearchShowPagesViewModel
                {
                    Cost = CostStatus.none,
                    OrderSortField = OrderSortField.AllData
                };

                TempData["keyword"] = keyword;
                TempData.Keep("keyword");
            }

            var model = new userFactory(DbContext).getShowPages(page, search, keyword);
            return PartialView("~/Views/Shared/CommonPartial/Card/_PartialShowList.cshtml", model);
        }

      
        public ActionResult GalleryList(int? cityId, int page = 1)
        {
            // 取得使用者頭像及姓名
            int userId = User.Identity.GetUserId<int>();
            getUserByIdInfo(userId);

            var model = new CommonPageFactory(DbContext).getGalleryPages(page, cityId);
            // 建立 城市的list
            var cityList = DbContext.City.Select(x => x).ToList();
            ViewBag.city = cityList;
            return View(model);
        }

        public ActionResult getGalleryList(int? cityId, int page = 1)
        {
            var model = new CommonPageFactory(DbContext).getGalleryPages(page, cityId);
            // 建立 城市的list
            var cityList = DbContext.City.Select(x => x).ToList();
            ViewBag.city = cityList;
            return PartialView("~/Views/Shared/CommonPartial/Card/_PartialCompanyList.cshtml", model);
        }


        public ActionResult addToMyGallery(string galleryId)
        {
            int userId = User.Identity.GetUserId<int>();
            var user = UserManager.FindById(userId);
            var galleryID = JsonConvert.DeserializeObject<Company>(galleryId);
            var myGallery = DbContext.Company.Where(p => p.Id == galleryID.Id).FirstOrDefault();
            try
            {
                user.CompanySubs.Add(myGallery);
                DbContext.SaveChanges();
            }
            catch
            {
                return new HttpStatusCodeResult(500, "false");
            }
            return new HttpStatusCodeResult(201, "success");
        }


        public ActionResult addToMyShow(int showId)
        {
            int userId = User.Identity.GetUserId<int>();
            var user = UserManager.FindById(userId);

            var theShow = DbContext.ShowPage.Where(p => p.Id == showId).FirstOrDefault();
            try
            {
                user.ShowPage.Add(theShow);
            }
            catch
            {
                return new HttpStatusCodeResult(500, "false");
            }

            try
            {
                DbContext.SaveChanges();
            }
            catch
            {
                return new HttpStatusCodeResult(500, "false");
            }

            return new HttpStatusCodeResult(201, "success");

        }

        

        public ActionResult deleFromMyShow(int showId)
        {
            var userId = User.Identity.GetUserId<int>();
            var user = UserManager.FindById(userId);

            var theDeletedShow = user.ShowPage.Where(p => p.Id == showId).FirstOrDefault();
            user.ShowPage.Remove(theDeletedShow);
            DbContext.SaveChanges();

            return new HttpStatusCodeResult(201, "success");
        }

        //Common首頁 end

        //=========================================================================================

        //Aside 導引畫面start
        public ActionResult MyshowPage(int? cityId, int page = 1)
        {
            // 建立 城市的list
            var cityList = DbContext.City.Select(x => x).ToList();

            // 建立 喜愛的展覽清單的viewmodel 的list
            int userId = User.Identity.GetUserId<int>();
            var user = UserManager.FindById(userId);
            
            // 取得使用者頭像及姓名
            getUserByIdInfo(userId);

            List<CommonShowViewModel> myShowList = new userFactory(DbContext).createMyShowList(cityId, userId, user);
            var ipagedMyShowList = OtherMethod.getCurrentPagedList(myShowList, page, pageSize);

            // 將兩個list加入viewModel
            CommonMyShowViewNodel viewModel = new userFactory(DbContext).createMyShowViewNodel(cityList, ipagedMyShowList);

            // 判斷選擇地區沒有展覽 || 未添加任何展覽進我的展覽時 顯示的訊息
            if (myShowList.Count() < 1 && cityId != null)
            {
                ViewBag.errorMsg = "此地區尚未有展覽被添加至「我的展覽」";
                ViewBag.guidMsg = "若想規劃此地區的看展行程，請再回到展覽清單探索該地區展覽";
            }
            else if (myShowList.Count() < 1 && cityId == null)
            {
                ViewBag.errorMsg = "尚未有展覽被添加至「我的展覽」";
                ViewBag.guidMsg = "若想進行展覽行程規劃，可從「全部展覽」添加展覽進入「我的展覽」";
            }


            return View(viewModel);
        }

        public ActionResult getMyshowPage(int? cityId, int page = 1)
        {
            // 建立 城市的list
            var cityList = DbContext.City.Select(x => x).ToList();

            // 建立 喜愛的展覽清單的viewmodel 的list
            int userId = User.Identity.GetUserId<int>();
            var user = UserManager.FindById(userId);

            List<CommonShowViewModel> myShowList = new userFactory(DbContext).createMyShowList(cityId, userId, user);
            var ipagedMyShowList = OtherMethod.getCurrentPagedList(myShowList, page, pageSize);

            // 將兩個list加入viewModel
            CommonMyShowViewNodel viewModel = new userFactory(DbContext).createMyShowViewNodel(cityList, ipagedMyShowList);

            // 判斷選擇地區沒有展覽 || 未添加任何展覽進我的展覽時 顯示的訊息
            if (myShowList.Count() < 1 && cityId != null)
            {
                ViewBag.errorMsg = "此地區尚未有展覽被添加至「我的展覽」";
                ViewBag.guidMsg = "若想規劃此地區的看展行程，請再回到展覽清單探索該地區展覽";
            }
            else if (myShowList.Count() < 1 && cityId == null)
            {
                ViewBag.errorMsg = "尚未有展覽被添加至「我的展覽」";
                ViewBag.guidMsg = "若想進行展覽行程規劃，可從「全部展覽」添加展覽進入「我的展覽」";
            }

            //return View(viewModel);
            return PartialView("~/Views/Shared/CommonPartial/Card/_PartialMyShowList.cshtml", viewModel.allShow);
        }

        public ActionResult MyItineraryPage()
        {
            int userId = User.Identity.GetUserId<int>();
            var user = UserManager.FindById(userId);
            
            // 取得使用者頭像及姓名
            getUserByIdInfo(userId);

            var model = new userFactory(_dbContext).getMyShowPage(user, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue);

            return View(model);
        }

        public ActionResult getMyShow(int cityId, DateTime StartDate, string StartTime, string EndTime)
        {
            int userId = User.Identity.GetUserId<int>();
            var user = UserManager.FindById(userId);

            int startHours = int.Parse(StartTime.Split(':')[0]);
            int startMinutes = int.Parse(StartTime.Split(':')[1]);
            DateTime startTime = new DateTime(1, 1, 1, startHours, startMinutes, 0);

            int endHours = int.Parse(EndTime.Split(':')[0]);
            int endMinutes = int.Parse(EndTime.Split(':')[1]);
            DateTime endTime = new DateTime(1, 1, 1, endHours, endMinutes, 0);

            var model = new userFactory(_dbContext).getMyShowPage(user, StartDate, startTime, endTime);

            if (cityId > 0)
                model = model.Where(m => m.city.Id == cityId);

            return PartialView("~/Views/Shared/CommonPartial/Card/_PartialMyItineraryShowList.cshtml", model);
        }


        public async Task<ActionResult> SendRountToEmail(string url)
        {
            int userId = User.Identity.GetUserId<int>();
            var user = UserManager.FindById(userId);

            await UserManager.SendEmailAsync(user.Id, "您的觀展路線已規劃完畢，請點擊內部連結，即可將路線導入至Google Map", "請按一下此連結將路線導入至Google Map：<a href=\"" + url + "\">這裏</a>");

            string success = SweetAlert.initAlert() + SweetAlert.SuccessAlert("寄送成功", "路線已寄送至您的信箱", "");
            return JavaScript(success);
        }

        public ActionResult MySubscription(int page = 1)
        {
            int userId = User.Identity.GetUserId<int>();
            var user = UserManager.FindById(userId);

            // 取得使用者頭像及姓名
            getUserByIdInfo(userId);

            var companyList = user.CompanySubs;
            var model = new CommonPageFactory(DbContext).getMyGalleryPages(page, companyList);
            if (companyList.Count == 0)
            {
                ViewBag.errorMsg = "尚未有展覽單位被添加至「訂閱單位」";
            }
            return View(model);
        }

        public ActionResult getMySubscription(int page = 1)
        {
            int userId = User.Identity.GetUserId<int>();
            var user = UserManager.FindById(userId);

            var companyList = user.CompanySubs;
            var model = new CommonPageFactory(DbContext).getMyGalleryPages(page, companyList);
            if (companyList.Count == 0)
            {
                ViewBag.errorMsg = "尚未有展覽單位被添加至「訂閱單位」";
            }
            return PartialView("~/Views/Shared/CommonPartial/Card/_PartialMyCompany.cshtml", model);
        }

        //訂閱展演單位 畫面 start
        public ActionResult SubscriptionDetail(string companyID)
        {
            List<CommonCompanyViewModel> viewModels = new List<CommonCompanyViewModel>();

            //取json對應userId data
            var searchId = JsonConvert.DeserializeObject<CommonCompanyViewModel>(companyID);
            var companyInfo = DbContext.Company.Find(searchId.Id);
            var showInfo = DbContext.ShowPage.Where(m => m.FK_Company == searchId.Id);

            if (companyInfo == null)
            {
                return HttpNotFound("該展演單位不存在或已被移除");
            }
            else
            {
                CommonCompanyViewModel model = new CommonCompanyViewModel()
                {
                    Id = companyInfo.Id,
                    CompanyName = companyInfo.CompanyName,
                    CompanyDescription = MvcHtmlString.Create(companyInfo.CompanyDescription).ToString(),
                    CompanyCity = companyInfo.City.CityName,
                    Address = companyInfo.Address,
                    Email = companyInfo.Email,
                    Phone = companyInfo.Phone,
                    HomePage = companyInfo.HomePage,
                    BusinessHours = companyInfo.BusinessHours,
                    PhotoSticker = "/SaveFiles/Company/" + companyInfo.Id + "/Info/" + companyInfo.PhotoStickerImage,
                    PromotionalImage = "/SaveFiles/Company/" + companyInfo.Id + "/Info/" + companyInfo.PromotionalImage,
                    ShowId = showInfo?.Select(s => s.Id).ToArray(),
                    ShowCity = showInfo?.Select(s => s.City.CityName).ToArray(),
                    ShowTitle = showInfo?.Select(s => s.Title).ToArray(),
                    ShowImg = DbContext?.ShowPageFile.Where(m => m.ShowPage.FK_Company == companyInfo.Id).Select(m => ("/SaveFiles/Company/" + companyInfo.Id + "/show/" + m.ShowPage.Id + "/" + m.fileName)).ToArray(),
                     ShowDiscription= showInfo?.Select(s => s.Description).ToArray()
                };

                viewModels.Add(model);
                return View(viewModels);
            }
        }
        public ActionResult DelFromMyGallery(string companyID)
        {
            int userId = User.Identity.GetUserId<int>();
            var user = UserManager.FindById(userId);
            var galleryID = JsonConvert.DeserializeObject<Company>(companyID);
            var myGallery = DbContext.Company.Where(p => p.Id == galleryID.Id).FirstOrDefault();
            try
            {
                user.CompanySubs.Remove(myGallery);
                DbContext.SaveChanges();
            }
            catch
            {
                return new HttpStatusCodeResult(500, "false");
            }

            return new HttpStatusCodeResult(201, "success");
        }

        public ActionResult ShowInformDetail()
        {
            return View();

            //訂閱展演單位 畫面 end
        }


        public ActionResult ShowInfo(int showId)
        {
            var show = DbContext.ShowPage.Find(showId);

            if (show == null)
            {
                return HttpNotFound("該展覽不存在或已被移除");
            }

            ShowPageViewModel model = new ShowPageViewModel
            {
                Address = $"{show.City.CityName}{show.District.DistrictName}{show.Address}",
                AgeRange = show.AgeRange,
                Cost = show.Cost,
                Description = show.Description,
                EndDate = show.EndDate,
                EndTime = show.EndTime,
                Id = show.Id,
                Price = show.Price,
                Remark = show.Remark,
                StartDate = show.StartDate,
                StartTime = show.StartTime,
                Title = show.Title,
                Todays = show.PageToTodaysList?.Select(m => m.Today).ToArray(),
                Keywords = show.KeywordsList?.Select(m => m.Name).ToArray(),
                images = show.ShowPageFiles?.Select(m => $"/SaveFiles/Company/{show.Company.Id}/show/{show.Id}/{m.fileName}").ToArray(),
                ViewCount = (ulong)show.PageViewCounts?.Select(m => m.Count).Sum()
            };

            return View(model);
        }

    }
}