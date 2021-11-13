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

        //�ޤJEF dbcontext
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

            // ������ɮ�
            string delAvatar = "";

            if (user.Avatar != viewModel.AvatarName)
                delAvatar = user.Avatar;

            // ��s���
            new userFactory(DbContext).updateToDB(user, viewModel);
            db.SaveChanges();

            // ���o�ɮצs�J���w��Ƨ�
            new userFactory(DbContext).saveAvatarToFolder(user, avatarFile);
            // �R�����ɮ�
            new userFactory(DbContext).DeleteAvatarFromFolder(user, delAvatar);

            // return
            return RedirectToAction("Index", "Common");
        }
        //�򥻸�Ƴ]�w ends

        //=========================================================================================

        //Common���� start

        public ActionResult Index(string city = "")
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
                search.FK_City = DbContext.City.Where(m => m.CityName == city).Select(m => m.Id).FirstOrDefault();

            TempData["SearchModel"] = search;
            TempData.Keep("SearchModel");

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

      
        public ActionResult GalleryList(int? cityId, int page = 1)
        {
            var model = new CommonPageFactory(DbContext).getGalleryPages(page, cityId);
            // �إ� ������list
            var cityList = DbContext.City.Select(x => x).ToList();
            ViewBag.city = cityList;
            return View(model);
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

        //Common���� end

        //=========================================================================================

        //Aside �ɤ޵e��start
        public ActionResult MyshowPage(int? cityId, int page = 1)
        {

            // �إ� ������list
            var cityList = DbContext.City.Select(x => x).ToList();

            // �إ� �߷R���i���M�檺viewmodel ��list
            int userId = User.Identity.GetUserId<int>();
            var user = UserManager.FindById(userId);

            List<CommonShowViewModel> myShowList = new userFactory(DbContext).createMyShowList(cityId, userId, user);
            var ipagedMyShowList = OtherMethod.getCurrentPagedList(myShowList, page, pageSize);

            // �N���list�[�JviewModel
            CommonMyShowViewNodel viewModel = new userFactory(DbContext).createMyShowViewNodel(cityList, ipagedMyShowList);

            // �P�_��ܦa�ϨS���i�� || ���K�[����i���i�ڪ��i���� ��ܪ��T��
            if (myShowList.Count() < 1 && cityId != null)
            {
                ViewBag.errorMsg = "���a�ϩ|�����i���Q�K�[�ܡu�ڪ��i���v";
                ViewBag.guidMsg = "�Y�Q�W�����a�Ϫ��ݮi��{�A�ЦA�^��i���M�汴���Ӧa�Ϯi��";
            }
            else if (myShowList.Count() < 1 && cityId == null)
            {
                ViewBag.errorMsg = "�|�����i���Q�K�[�ܡu�ڪ��i���v";
                ViewBag.guidMsg = "�Y�Q�i��i����{�W���A�i�q�u�����i���v�K�[�i���i�J�u�ڪ��i���v";
            }


            return View(viewModel);
        }

        public ActionResult MyItineraryPage()
        {
            int userId = User.Identity.GetUserId<int>();
            var user = UserManager.FindById(userId);

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

            await UserManager.SendEmailAsync(user.Id, "�z���[�i���u�w�W�������A���I�������s���A�Y�i�N���u�ɤJ��Google Map", "�Ы��@�U���s���N���u�ɤJ��Google Map�G<a href=\"" + url + "\">�o��</a>");

            string success = SweetAlert.initAlert() + SweetAlert.SuccessAlert("�H�e���\", "���u�w�H�e�ܱz���H�c", "");
            return JavaScript(success);
        }

        public ActionResult MySubscription(int page = 1)
        {
            int userId = User.Identity.GetUserId<int>();
            var user = UserManager.FindById(userId);
            var companyList = user.CompanySubs;
            var model = new CommonPageFactory(DbContext).getMyGalleryPages(page, companyList);
            if (companyList.Count == 0)
            {
                ViewBag.errorMsg = "�|�����i�����Q�K�[�ܡu�q�\���v";
            }
            return View(model);
        }
        //public ActionResult GalleryList(int? cityId, int page = 1)
        //{
        //    var model = new CommonPageFactory(DbContext).getGalleryPages(page, cityId);
        //    // �إ� ������list
        //    var cityList = DbContext.City.Select(x => x).ToList();
        //    ViewBag.city = cityList;
        //    return View(model);
        //}
        //Aside �ɤ޵e��end


        //�q�\�i�t��� �e�� start
        public ActionResult SubscriptionDetail(string companyID)
        {
            List<CommonCompanyViewModel> viewModels = new List<CommonCompanyViewModel>();

            //��json����userId data
            var searchId = JsonConvert.DeserializeObject<CommonCompanyViewModel>(companyID);
            var companyInfo = DbContext.Company.Find(searchId.Id);
            var showInfo = DbContext.ShowPage.Where(m => m.FK_Company == searchId.Id);

            if (companyInfo == null)
            {
                return HttpNotFound("�Ӯi�t��줣�s�b�Τw�Q����");
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

            //�q�\�i�t��� �e�� end
        }


        public ActionResult ShowInfo(int showId)
        {
            var show = DbContext.ShowPage.Find(showId);

            if (show == null)
            {
                return HttpNotFound("�Ӯi�����s�b�Τw�Q����");
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

        // footer starts
        [HttpPost]
        public async Task sendMsgToAdminAsync(string feedBack, string userEmail)
        {
            IdentityMessage msg;
            IdentityMessage msgConfirm;
            if (Request.IsAuthenticated)
            {
                // ���o�ϥΪ�
                int userId = User.Identity.GetUserId<int>();
                var user = UserManager.FindById(userId);

                // �H�H���޲z��
                msg = new IdentityMessage
                {
                    Destination = "admin@artwander.art",
                    Subject = "�ϥΪ̷N���^�X",
                    Body = $"�ϥΪ̦W�� : {user.UserName}</br>�ϥΪ�id : {user.Id}</br>�ϥΪ̫H�c : {user.Email}</br></br>���� : {feedBack}"
                };

                // �H�H���ϥΪ�
                msgConfirm = new IdentityMessage
                {
                    Destination = user.Email,
                    Subject = "arTWander ���庩�B",
                    Body = $"<h3>arTWander�w����z���^�X</h3><br><p>{user.UserName} �z�n : <br>�P�±z���ӫH�A�z���_�Q�N���P��ĳ�A�O�ڭ̤��_�ﵽ����A�ȫ~�誺�̨ΰʤO�A�q�Ф��[���СC�p������ðݡA�w��ӹq���ߡA�����N�ܸ۬��z�A��</p><br><br>���l�󬰨t�εo�e�l��A�ФŪ����^��<br>�q�ܡG0800-111-222 | �H�c�Gtest001@gmail.com"
                };
                
            }
            else
            {
                msg = new IdentityMessage
                {
                    Destination = "admin@artwander.art",
                    Subject = $"�ϥΪ̷N���^�X",
                    Body = $"�ϥΪ̦W�� : null</br>�ϥΪ�id : null</br>�ϥΪ̫H�c : {userEmail}</br></br>���� : {feedBack}"
                };

                // �H�H���ϥΪ�
                msgConfirm = new IdentityMessage
                {
                    Destination = userEmail,
                    Subject = "arTWander ���庩�B",
                    Body = $"<h3>arTWander�w����z���^�X</h3><br><p>{userEmail}�z�n :<br> �P�±z���ӫH�A�z���_�Q�N���P��ĳ�A�O�ڭ̤��_�ﵽ����A�ȫ~�誺�̨ΰʤO�A�q�Ф��[���СC�p������ðݡA�w��ӹq���ߡA�����N�ܸ۬��z�A��</p><br><br>���l�󬰨t�εo�e�l��A�ФŪ����^��<br>�q�ܡG0800-111-222 | �H�c�Gtest001@gmail.com"
                };
            }

            await new EmailService().SendAsync(msg);
            await new EmailService().SendAsync(msgConfirm);
        }
        // footer ends
    }
}