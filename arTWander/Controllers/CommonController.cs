using arTWander.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
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

namespace arTWander.Controllers
{
    public class CommonController : Controller
    {
        // GET: Common


        public ActionResult SetUp()
        {
            // 取得model
            IndexViewModel model = (IndexViewModel)TempData["model"];
            TempData.Keep("model");

            // 取得user
            int Userid = User.Identity.GetUserId<int>();
            ApplicationUser user = new ApplicationUser();
            user = new userFactory().getUserById(Userid);

            // 轉換user.birthay型別以符合前端需求
            DateTime Birthday = (DateTime)(user.Birthday);
            string Bday = Birthday.ToString("yyyy-MM-dd");


            // 取得user資料模型
            CommonInfoViewModel viewModel = new CommonInfoViewModel
            {
                HasPassword = model.HasPassword,
                Logins = model.Logins,
                PhoneNumber = user.PhoneNumber,
                TwoFactor = model.TwoFactor,
                BrowserRemembered = model.BrowserRemembered,
                UserName = user.UserName,
                Birthday = Bday,
                AccountAddress = user.AccountAddress,
                AvatarUrl = "/image/avatar/",
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
        public ActionResult SetUp(CommonInfoViewModel viewModel, HttpPostedFileBase avatarFile)
        {
            // 取得viewModel存入資料庫
            ApplicationDbContext db = new ApplicationDbContext();
            int userId = User.Identity.GetUserId<int>();

            ApplicationUser user = db.Users.Where(u => u.Id == userId).FirstOrDefault();
            user.Avatar = viewModel.AvatarName;
            user.UserName = viewModel.UserName;
            user.Birthday = DateTime.Parse(viewModel.Birthday);
            user.AccountAddress = viewModel.AccountAddress;
            user.PhoneNumber = viewModel.PhoneNumber;
            user.TwoFactorEnabled = viewModel.TwoFactor;

            db.SaveChanges();

            // 取得檔案存入指定資料夾
            new userFactory().saveAvatarToFolder(user, avatarFile);

            return RedirectToAction("Index", "Home");
        }



        //Common首頁畫面start
        public ActionResult Index()
        {
            return View();
        }
        //Common首頁畫面end


        public ActionResult MyshowPag()
        {
            return View();
        }
        

        //Aside 導引畫面start

        public ActionResult MyshowPage()
        {
            return View();
        }

        public ActionResult MyItineraryPage()
        {
            return View();
        }

        public ActionResult MySubscription()
        {
            return View();
        }
        //Aside 導引畫面end
        //訂閱展演單位 畫面 start
        public ActionResult SubscriptionDetail()
        {
            return View();
        }
        public ActionResult ShowInformDetail()
        {
            return View();
        }
        
        //訂閱展演單位 畫面 end

    }
}