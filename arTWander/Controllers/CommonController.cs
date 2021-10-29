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

        //�򥻸�Ƴ]�w starts
        public ActionResult SetUp()
        {
            // ���omodel
            IndexViewModel model = (IndexViewModel)TempData["model"];
            TempData.Keep("model");

            // ���oviewModel
            int userId = User.Identity.GetUserId<int>();
            CommonInfoViewModel viewModel = new userFactory().createViewModel(model, userId);
            
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

            new userFactory().updateToDB(user, viewModel);
            db.SaveChanges();

            // ���o�ɮצs�J���w��Ƨ�
            new userFactory().saveAvatarToFolder(user, avatarFile);

            // return
            return RedirectToAction("Index", "Home");
        }
        //�򥻸�Ƴ]�w ends

        //=========================================================================================

        //Common���� start
        public ActionResult Index()
        {
            // ���o�ϥΪ��Y���Ωm�W
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
            // ���o�ϥΪ��Y���Ωm�W
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

        //Common���� end

        //=========================================================================================

        //Aside �ɤ޵e��start
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
        //Aside �ɤ޵e��end
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