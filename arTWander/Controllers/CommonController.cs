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



        //基本資料設定 starts
        public ActionResult SetUp()
        {
            // 取得model
            IndexViewModel model = (IndexViewModel)TempData["model"];
            TempData.Keep("model");

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

            new userFactory(DbContext).updateToDB(user, viewModel);
            db.SaveChanges();

            // 取得檔案存入指定資料夾
            new userFactory(DbContext).saveAvatarToFolder(user, avatarFile);

            // return
            return RedirectToAction("Index", "Home");
        }
        //基本資料設定 ends

        //=========================================================================================

        //Common首頁 start

        public ActionResult Index(string city="")
        {
            // 取得使用者頭像及姓名
            int userId = User.Identity.GetUserId<int>();
            ApplicationUser user = DbContext.Users.Where(u => u.Id == userId).FirstOrDefault();

            SearchShowPagesViewModel search = new SearchShowPagesViewModel
            {
                Cost = CostStatus.none,
                OrderSortField = OrderSortField.DateSort
            };

            if (!string.IsNullOrEmpty(city))
            {
                search.FK_City = DbContext.City.Where(m => m.CityName == city).Select(m => m.Id).FirstOrDefault();
            }

            //使用者未登入
            if (user == null)
            {
                ViewBag.userName = "";
                ViewBag.avatarUrl = "/image/avatar/avatar_default.png";
                return View(new userFactory(DbContext).getShowPages(model: search));
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
            return View(new userFactory(DbContext).getShowPages(model: search));
        }

        [HttpPost]
        public ActionResult getShowList(SearchShowPagesViewModel searchModel, int page = 1)
        {
            TempData["SearchModel"] = searchModel;
            TempData.Keep("SearchModel");

            var model = new userFactory(DbContext).getShowPages(page, searchModel);
            return PartialView("~/Views/Shared/CommonPartial/Card/_PartialShowList.cshtml", model);
        }

        public ActionResult GalleryList(int? cityId)
        {
            List<CommonCompanyViewModel> viewModels = new List<CommonCompanyViewModel>();
            if (cityId != null)
            {
                viewModels = (new CommonPageFactory(DbContext).queryCustomerByCity(cityId)).ToList();
            }
            else
            {
                viewModels = (new CommonPageFactory(DbContext).queryAllCustomer()).ToList();
                
            }
            // 建立 城市的list
            var cityList = DbContext.City.Select(x => x).ToList();
            ViewBag.city = cityList;

            return View(viewModels);
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
        public ActionResult MyshowPage(int? cityId)
        {
            // 建立 城市的list
            var cityList = DbContext.City.Select(x => x).ToList();

            // 建立 喜愛的展覽清單的viewmodel 的list
            int userId = User.Identity.GetUserId<int>();
            var user = UserManager.FindById(userId);

            List<CommonShowViewModel> myShowList = new userFactory(DbContext).createMyShowList(cityId, userId, user);

            // 將兩個list加入viewModel
            CommonMyShowViewNodel viewModel = new userFactory(DbContext).createMyShowViewNodel(cityList, myShowList);

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

        public ActionResult MyItineraryPage()
        {
            return View();
        }

        public ActionResult MySubscription()
        {
            int userId = User.Identity.GetUserId<int>();
            var user = UserManager.FindById(userId);
            var companyList = user.CompanySubs;
            if (companyList.Count == 0)
            {
                ViewBag.errorMsg = "尚未有展覽單位被添加至「訂閱單位」";
            }
            return View(companyList.ToList());
        }
        //Aside 導引畫面end


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
                    CompanyDescription = companyInfo.CompanyDescription,
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
                    ShowDiscription = showInfo.Select(s => s.Description).ToArray(),
                    ShowImg = DbContext?.ShowPageFile.Where(m => m.ShowPage.FK_Company == companyInfo.Id).Select(m => ("/SaveFiles/Company/" + companyInfo.Id + "/show/" + m.ShowPage.Id + "/" + m.fileName)).ToArray()
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