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
using static arTWander.Models.CommonViewModel;

namespace arTWander.Controllers
{
    public class CommonController : Controller
    {

        //基本資料設定 starts
        public ActionResult SetUp()
        {
            // 取得model
            IndexViewModel model = (IndexViewModel)TempData["model"];
            TempData.Keep("model");

            // 取得viewModel
            int userId = User.Identity.GetUserId<int>();
            CommonInfoViewModel viewModel = new userFactory().createViewModel(model, userId);
            
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

            new userFactory().updateToDB(user, viewModel);
            db.SaveChanges();

            // 取得檔案存入指定資料夾
            new userFactory().saveAvatarToFolder(user, avatarFile);

            // return
            return RedirectToAction("Index", "Home");
        }
        //基本資料設定 ends

        //=========================================================================================

        //Common首頁 start
        public ActionResult Index()
        {
            // 取得使用者頭像及姓名
            int userId = User.Identity.GetUserId<int>();

            ApplicationDbContext db = new ApplicationDbContext();
            ApplicationUser user = db.Users.Where(u => u.Id == userId).FirstOrDefault();

            ViewBag.userName = user.UserName;
            ViewBag.avatarUrl = "/image/avatar/" + user.Avatar;

            // return
            return View();
        }

        [HttpPost]
        public ActionResult Index(CommonShowViewModel viewModel)
        {
            // 取得使用者頭像及姓名
            int userId = User.Identity.GetUserId<int>();

            ApplicationDbContext db = new ApplicationDbContext();
            ApplicationUser user = db.Users.Where(u => u.Id == userId).FirstOrDefault();

            ViewBag.userName = user.UserName;
            ViewBag.avatarUrl = "/image/avatar/" + user.Avatar;

            //=============================================

            IQueryable<CommonShowViewModel> q = new userFactory().queryAllShow();
            List<CommonShowViewModel> viewModels = q.ToList();
            if (viewModel.isSelectedCity == "true")
            {
                viewModels = q.Where(s => s.showCity == viewModel.showCity).DefaultIfEmpty().ToList();
            }

            // return
            return View(viewModels);
        }

        public ActionResult ShowList(CommonShowViewModel viewModel)
        {
            IQueryable<CommonShowViewModel> q = new userFactory().queryAllShow();
            List<CommonShowViewModel> viewModels = q.ToList();
            if (viewModel.isSelectedCity == "true")
            {
                viewModels = q.Where(s => s.showCity == viewModel.showCity).DefaultIfEmpty().ToList();
            }

            return View(viewModels);
        }


        //[HttpPost]
        //public ActionResult ShowList(CommonShowViewModel viewModel)
        //{
        //    IQueryable<CommonShowViewModel> q = new userFactory().queryAllShow();
        //    List<CommonShowViewModel> viewModels = q.ToList();
        //    if (viewModel.isSelectedCity == "true")
        //    {
        //        viewModels = q.Where(s => s.showCity == viewModel.showCity).DefaultIfEmpty().ToList();
        //    }

        //    return View(viewModels);
        //}

        public ActionResult GalleryList()
        {
            return View();
        }

        //Common首頁 end

        //=========================================================================================

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