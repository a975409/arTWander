﻿using arTWander.Models;
using arTWander.Models.AdminFactory;
using arTWander.Models.AdminViewModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
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
        //如果要在其他Controller引用DbContext來對資料表做CRUD，請參考第17~33行新增DbContext
        //詳細用法請參考底下 Index 這個 Action
        public AdminController()
        {
        }

        public AdminController(ApplicationUserManager userManager, ApplicationDbContext dbContext)
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
        public ActionResult AllShowList()
        {
            return View();
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

            //判斷是否有搜尋字串
            var searchWord = Request.Form["txtKeyword"];
            if (string.IsNullOrEmpty(searchWord))
            {
                List<UserListViewModel> model = (List<UserListViewModel>)new AdminFactory().GetUserListAll();
                    return View(model);
            }
            else
            {
                //var searchData =from p in db.Users where p.UserName.Contains(searchWord) select p;
                //foreach (ApplicationUser userName in searchData)
                //{

                //    if (!string.IsNullOrEmpty(userName.Birthday.ToString()))
                //    {
                //        DateTime Birthday = (DateTime)(userName.Birthday);
                //        Bday = Birthday.ToString("yyyy-MM-dd");
                //    }
                //    else
                //    {
                //        Bday = DateTime.Now.ToString("yyyy-MM-dd");
                //    }

                //    model.Add(new UserListViewModel()
                //    {
                //        FK_ApplicationUser = userName.Id,
                //        users = userName,
                //        PhoneNumber = userName.PhoneNumber,
                //        UserName = userName.UserName,
                //        Birthday = Bday,
                //        AccountAddress = userName.AccountAddress,
                //        AvatarUrl = "/SaveFiles/Admin/Avatar/",
                //        AvatarName = userName.Avatar,
                //        Email = userName.Email,
                //        RegisterTime = loginLog.RegisterTime,
                //        LastloginTime = loginLog.LastloginTime,
                //        LoginOutTime = loginLog.LoginOutTime,
                //        LogingCount = loginLog.LogingCount,
                //        Statue = loginLog.Statue,
                //        Title = showPage.Title,
                //        Comment = showComment.Comment,
                //        Star = showComment.Star
                //    });

                //}
                return View();
            }

        }


        public ActionResult UserInform()
        {
            return View();
        }
        public ActionResult ShowCustomList()
        {
            return View();
        }
        public ActionResult CustomerInform()
        {
            return View();
        }
        public ActionResult BlackList()
        {
            return View();
        }
        public ActionResult BlackInform()
        {
            return View();
        }


        //User Detail Page end


        //Show detail page start
        public ActionResult ShowInformPage()
        {
            return View();
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