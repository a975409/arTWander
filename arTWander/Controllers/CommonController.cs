using arTWander.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
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

        //�򥻸�Ƴ]�w starts
        public ActionResult SetUp(string firstLogIn)
        {
            // ���omodel
            IndexViewModel model;
            if (firstLogIn == "true")
            {
                // ���p�즸�n�J
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

            // ���oviewModel
            int userId = User.Identity.GetUserId<int>();
            CommonInfoViewModel viewModel = new userFactory(DbContext).createViewModel(model, userId);
            
            // return
            return View(viewModel);

        }

        [HttpPost]
        public ActionResult SetUp(CommonInfoViewModel viewModel, HttpPostedFileBase avatarFile)
        {
            // ���oviewModel�s�J��Ʈw
            ApplicationDbContext db = new ApplicationDbContext();
            int userId = User.Identity.GetUserId<int>();
            ApplicationUser user = db.Users.Where(u => u.Id == userId).FirstOrDefault();

            new userFactory(DbContext).updateToDB(user, viewModel);
            db.SaveChanges();

            // ���o�ɮצs�J���w��Ƨ�
            new userFactory(DbContext).saveAvatarToFolder(user, avatarFile);

            // return
            return RedirectToAction("Index", "Home");
        }
        //�򥻸�Ƴ]�w ends

        //=========================================================================================

        //Common���� start

        public ActionResult Index(string city="")
        {
            // ���o�ϥΪ��Y���Ωm�W
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

            //�ϥΪ̥��n�J
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
                    ViewBag.avatarUrl = string.IsNullOrEmpty(user.Avatar) ? "/image/avatar/avatar_default.png" : $"/SaveFiles/Member/{user.Id}/{user.Avatar}";
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

        [HttpPost]
        public ActionResult GalleryList()
        {
            return View();
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

        //Common���� end

        //=========================================================================================

        //Aside start
        public ActionResult MyshowPage(int? cityId)
        {
            // �إ� ������list
            var cityList = DbContext.City.Select(x => x).ToList();

            // �إ� �߷R���i���M�檺viewmodel ��list
            int userId = User.Identity.GetUserId<int>();
            var user = UserManager.FindById(userId);

            List<CommonShowViewModel> myShowList = new userFactory(DbContext).createMyShowList(cityId, userId, user);

            // �N���list�[�JviewModel
            CommonMyShowViewNodel viewModel = new userFactory(DbContext).createMyShowViewNodel(cityList, myShowList);
            
            // �P�_��ܦa�ϨS���i�� || ���K�[����i���i�ڪ��i���� ��ܪ��T��
            if (myShowList.Count() < 1 && cityId != null)
            {
                ViewBag.errorMsg = "���a�ϩ|�����i���Q�K�[�ܡu�ڪ��i���v";
                ViewBag.guidMsg = "�Y�Q�W�����a�Ϫ��ݮi��{�A�ЦA�^��i���M�汴���Ӧa�Ϯi��";
            }
            else if(myShowList.Count() < 1 && cityId == null)
            {
                ViewBag.errorMsg = "�|�����i���Q�K�[�ܡu�ڪ��i���v";
                ViewBag.guidMsg = "�Y�Q�i��i����{�W���A�i�q�u�����i���v�K�[�i���i�J�u�ڪ��i���v";
            }
                

            return View(viewModel);

        }

        public ActionResult MyItineraryPage()
        {
            return View();
        }

        public ActionResult MySubscription()
        {
            return View();
        }
        //Aside end
        //�q�\�i�t��� �e�� start
        public ActionResult SubscriptionDetail()
        {
            return View();
        }
        public ActionResult ShowInformDetail()
        {
            return View();
        }
        
        //�q�\�i�t��� �e�� end

    }
}