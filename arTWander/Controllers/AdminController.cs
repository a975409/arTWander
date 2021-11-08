using arTWander.Models;
using arTWander.Models.AdminFactory;
using arTWander.Models.AdminViewModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace arTWander.Controllers
{
    public class AdminController : Controller
    {
        
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

        // GET: Admin
        public ActionResult Index()
        {
            // 取得model
            IndexViewModel model = (IndexViewModel)TempData["model"];
            TempData.Keep("model");

            // 取得user
            int Userid = User.Identity.GetUserId<int>();
            var user = new AdminFactory().getUserById(Userid);

            // 取得user資料模型
            SetupViewModel viewModel = new SetupViewModel
            {
                HasPassword = model.HasPassword,
                Logins = model.Logins,
                TwoFactor = model.TwoFactor,
                BrowserRemembered = model.BrowserRemembered,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                AccountAddress = user.AccountAddress,
                AvatarUrl = "/SaveFiles/Admin/Avatar/",
                AvatarName = user.Avatar,
                Email = user.Email
            };
            return View(viewModel);
        }



        //Aside導引start
        public ActionResult MemberManage()
        {

            return View();
        }

        public ActionResult ShowManage()
        {
            return View();
        }

        //展演列表
        public ActionResult AllShowList()
        {
            List<ShowListViewModel> model = null;
            //判斷是否有搜尋字串
            var searchWord = Request.Form["txtKeyword"];
            if (string.IsNullOrEmpty(searchWord))
            {
                model = (List<ShowListViewModel>)new AdminFactory().GetShowListAll();
            }
            else
            {
                model = (List<ShowListViewModel>)new AdminFactory().GetShowListBySearch(searchWord);
            }
            return View(model);
        }

        public ActionResult InformMange()
        {
            return View();
        }
        public ActionResult dataStatisticsManage()
        {
            return View();
        }


        //Aside導引end

        //User Detail Page start
        public ActionResult UserList()
        {
            var db = new ApplicationDbContext();
            var userSearch = from users in db.Users select users.UserName;
            List<UserListViewModel> model = null;
            //判斷是否有搜尋字串
            var searchWord = Request.Form["txtKeyword"];
            if (string.IsNullOrEmpty(searchWord))
            {
                model = (List<UserListViewModel>)new AdminFactory().GetUserListAll();
            }
            else
            {
                model = (List<UserListViewModel>)new AdminFactory().GetUserListBySearch(searchWord);
            }
            return View(model);
        }

 
        //用戶詳細資訊

        //public ActionResult UserInform()
        //{
        //    return View();
        //}

        public ActionResult UserInform(string userId)
        {
            List<UserListViewModel> model = null;
            //取json對應userId data
            var userSearch = JsonConvert.DeserializeObject<UserListViewModel>(userId);
            //取得該id詳細資訊
            model = (List<UserListViewModel>)new AdminFactory().GetUserInformById(userSearch.userId);
            return View(model);
        }

        //User 加入黑名單
        [HttpPost]
        public ActionResult UserAddBlack(string blackAdd)
        {
            var db = new ApplicationDbContext();
            //取json對應blackAdd data
            var BlackSearch = JsonConvert.DeserializeObject<BlackList>(blackAdd);
            var BlackSuccess = JsonConvert.DeserializeObject<ApplicationUser>(blackAdd);
            //取json對應userId data
            var black = db.BlackList.Where(m=>m.FK_ApplicationUser == BlackSearch.FK_ApplicationUser).FirstOrDefault();
            //新增黑名單
            if (black == null)
            {
                db.BlackList.Add(new BlackList()
                    {
                        FK_ApplicationUser= BlackSearch.FK_ApplicationUser,
                        Created_At= DateTime.Now,
                        Reason= BlackSearch.Reason
                });
                db.SaveChanges();
            }
            
            string success = SweetAlert.SuccessAlert("黑名單", $"{BlackSuccess.UserName} 已加入黑名單", "");
            //重新導向
            return JavaScript(success);
        }

        //展演單位清單
        public ActionResult ShowCustomList()
        {
            var db = new ApplicationDbContext();
            List < CustomerListViewModel > model = null;
            //判斷是否有搜尋字串
            var searchWord = Request.Form["txtKeyword"];
            //= Request.Form["txtKeyword"];
            if (string.IsNullOrEmpty(searchWord))
            {
                model = (List<CustomerListViewModel>)new AdminFactory().GetCustomerListAll();
            }
            else
            {
                model = (List<CustomerListViewModel>)new AdminFactory().GetCustomerBySearch(searchWord);
            }
            return View(model);

        }

        //展演單位詳細資訊
        public ActionResult CustomerInform(string userId)
        {
            List<CustomerListViewModel> model = null;
            //取json對應userId data
            var userSearch = JsonConvert.DeserializeObject<CustomerListViewModel>(userId);
            //取得該id詳細資訊
            model = (List<CustomerListViewModel>)new AdminFactory().GetCustomerInformById(userSearch.userId);
            return View(model);
        }

        //User 加入黑名單
        [HttpPost]
        public ActionResult CustomerAddBlack(string blackAdd)
        {
            var db = new ApplicationDbContext();
            //取json對應blackAdd data
            var BlackSearch = JsonConvert.DeserializeObject<BlackList>(blackAdd);
            var BlackSuccess = JsonConvert.DeserializeObject<ApplicationUser>(blackAdd);

            //取json對應userId data
            var black = db.BlackList.Where(m => m.FK_ApplicationUser == BlackSearch.FK_ApplicationUser).FirstOrDefault();
            //新增黑名單
            if (black == null)
            {
                db.BlackList.Add(new BlackList()
                {
                    FK_ApplicationUser = BlackSearch.FK_ApplicationUser,
                    Created_At = DateTime.Now,
                    Reason = BlackSearch.Reason
                });
                db.SaveChanges();
            }

            //sweetalert無用
            string success = SweetAlert.SuccessAlert("黑名單", $"{BlackSuccess.UserName} 已加入黑名單", "");
            //重新導向
            return JavaScript(success);
        }


        //黑名單
        public ActionResult BlackList()
        {
            return View();
        }
        //public ActionResult BlackInform()
        //{
        //    return View();
        //}

        //黑名單用戶
        public ActionResult BlackUser()
        {
            var db = new ApplicationDbContext();
            var userSearch = from users in db.Users select users.UserName;
            List<BlackListViewModel> model = null;
            //判斷是否有搜尋字串
            var searchWord = Request.Form["txtKeyword"];
            if (string.IsNullOrEmpty(searchWord))
            {
                model = (List<BlackListViewModel>)new AdminFactory().GetBlackUserAll();
            }
            else
            {
                model = (List<BlackListViewModel>)new AdminFactory().GetBlackUserBySearch(searchWord);
            }
            return View(model);
        }

        public ActionResult BlackUserInform(string userId)
        {
            List<BlackListViewModel> model = null;
            //取json對應userId data
            var userSearch = JsonConvert.DeserializeObject<BlackListViewModel>(userId);
            //取得該id詳細資訊
            model = (List<BlackListViewModel>)new AdminFactory().GetBlackUserInformById(userSearch.userId);
            return View(model);
        }

       

        public ActionResult BlackCustomer()
        {
            var db = new ApplicationDbContext();
            //var userSearch = from users in db.Users where role.RoleId.ToString() == "2" select users.UserName;
            List<BlackListViewModel> model = null;
            //判斷是否有搜尋字串
            var searchWord = Request.Form["txtKeyword"];
            //= Request.Form["txtKeyword"];
            if (string.IsNullOrEmpty(searchWord))
            {
                model = (List<BlackListViewModel>)new AdminFactory().GetBlackCustomerAll();
            }
            else
            {
                model = (List<BlackListViewModel>)new AdminFactory().GetCustomerBySearch(searchWord);
            }
            return View(model);

        }


        public ActionResult BlackCustomerInfo(string userId)
        {
            List<BlackListViewModel> model = null;
            //取json對應userId data
            var userSearch = JsonConvert.DeserializeObject<BlackListViewModel>(userId);
            //取得該id詳細資訊
            model = (List<BlackListViewModel>)new AdminFactory().GetBlackCustomerInformById(userSearch.userId);
            return View(model);
        }

        //User 取消黑名單
        [HttpPost]
        public ActionResult ReleaseBlack(string blackAdd)
        {
            var db = new ApplicationDbContext();

            //取json對應blackAdd data
            var BlackSearch = JsonConvert.DeserializeObject<BlackList>(blackAdd);
            var BlackSuccess = JsonConvert.DeserializeObject<ApplicationUser>(blackAdd);
            //取json對應userId data
            BlackList blackRls = db.BlackList.FirstOrDefault(b => b.FK_ApplicationUser == BlackSearch.FK_ApplicationUser);
            //黑名單移除
            if (blackRls != null)
            {
                db.BlackList.Remove(blackRls);
                db.SaveChanges();
            }

            string success = SweetAlert.SuccessAlert("黑名單", $"{BlackSuccess.UserName} 已加入黑名單", "");
            //重新導向
            return JavaScript(success);
        }


        //User Detail Page end


        //Show detail page start
        public ActionResult ShowInformPage(string showID)
        {
            List<ShowListViewModel> model = null;
            //取json對應userId data
            var userSearch = JsonConvert.DeserializeObject<ShowListViewModel>(showID);
            //取得該id詳細資訊
            model = (List<ShowListViewModel>)new AdminFactory().GetShowInformById(userSearch.Id.ToString());
              return View(model);
        }

        public ActionResult ShowDelete(string showDele)
        {
            var db = new ApplicationDbContext();

            //取json對應blackAdd data
            var userSearch = JsonConvert.DeserializeObject<ShowListViewModel>(showDele);
            //取json對應userId data
            ShowPage showpageDel = db.ShowPage.FirstOrDefault(b => b.Id == userSearch.Id);
            //黑名單移除
            if (showpageDel != null)
            {
                db.ShowPage.Remove(showpageDel);
                db.SaveChanges();
            }

            string success = SweetAlert.SuccessAlert("黑名單", $"{userSearch.Title} 已下架", "");
            //重新導向
            return JavaScript(success);
        }
        

        //Show detail page end

        //Inform detail page start
        public ActionResult ShowInform()
        {
            return View();
        }
        public ActionResult ShowReport()
        {
            return View();
        }
        public ActionResult PostInform()
        {
            return View();
        }
        public ActionResult PostsReport()
        {
            return View();
        }
        public ActionResult FeedBack()
        {
            return View();
        }
        public ActionResult feedBackPage()
        {
            return View();
        }

        //Inform detail page end

        //dataStatisticsManage detail page start
        public ActionResult WebFlow()
        {
            return View();
        }
        public ActionResult UserFlow()
        {
            return View();
        }
        public ActionResult ShowFlow()
        {
            return View();
        }
        //dataStatisticsManage detail page end


        //修改個人資料頁面start
        public ActionResult AdminSetup()
        {
            // 取得model
            IndexViewModel model = (IndexViewModel)TempData["model"];
            TempData.Keep("model");

            // 取得目前user id
            int Userid = User.Identity.GetUserId<int>();
            var user = new AdminFactory().getUserById(Userid);

            // 轉換user.birthay型別以符合前端需求
            string Bday = "";
            if (!string.IsNullOrEmpty(user.Birthday.ToString()))
            {
                DateTime Birthday = (DateTime)(user.Birthday);
                Bday = Birthday.ToString("yyyy-MM-dd");
            }
            else
            {
                Bday = DateTime.Now.ToString("yyyy-MM-dd");
            }


            // 取得user資料模型
            SetupViewModel viewModel = new SetupViewModel
            {
                HasPassword = model.HasPassword,
                Logins = model.Logins,
                TwoFactor = model.TwoFactor,
                BrowserRemembered = model.BrowserRemembered,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                AccountAddress = user.AccountAddress,
                Birthday = Bday,
                AvatarUrl = "/SaveFiles/Admin/Avatar/",
                AvatarName = user.Avatar,
                Email = user.Email
            };

            // 判斷是否使用預設頭像
            if (string.IsNullOrEmpty(user.Avatar))
            {
                viewModel.AvatarUrl += "avatar_default.png";
                viewModel.AvatarName = "avatar_default.png";
            }
            else
            {
                viewModel.AvatarUrl += user.Avatar;
                viewModel.AvatarName = user.Avatar;
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AdminSetup(SetupViewModel viewModel, HttpPostedFileBase avatarFile)
        {
            // 取得viewModel存入資料庫
            ApplicationDbContext db = new ApplicationDbContext();
            int userId = User.Identity.GetUserId<int>();
            ApplicationUser user = db.Users.Where(u => u.Id == userId).FirstOrDefault();
            var delAvatar="";
            if (user != null)
            {
                //取得Avatar圖檔舊名
                if (user.Avatar != viewModel.AvatarName)
                {
                    delAvatar = user.Avatar;
                }
                user.Avatar = viewModel.AvatarName;
                user.UserName = viewModel.UserName;
                user.Birthday = DateTime.Parse(viewModel.Birthday);
                user.AccountAddress = viewModel.AccountAddress;
                user.PhoneNumber = viewModel.PhoneNumber;
                user.TwoFactorEnabled = viewModel.TwoFactor;
            }

            db.SaveChanges();

            // 取得檔案存入指定資料夾
            new AdminFactory().saveAvatarToFolder(user, avatarFile);
            // 移除舊檔案
            new AdminFactory().DeleteAvatarFromFolder(delAvatar);

            return RedirectToAction("Index", "Admin");
        }
        //修改個人資料頁面end

        public ActionResult _AdminLayout()
        {
            return View();
        }
    }
}