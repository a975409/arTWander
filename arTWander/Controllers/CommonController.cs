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
            // ���omodel
            IndexViewModel model = (IndexViewModel)TempData["model"];
            TempData.Keep("model");

            // ���ouser
            int Userid = User.Identity.GetUserId<int>();
            ApplicationUser user = new ApplicationUser();
            user = new userFactory().getUserById(Userid);

            // �ഫuser.birthay���O�H�ŦX�e�ݻݨD
            DateTime Birthday = (DateTime)(user.Birthday);
            string Bday = Birthday.ToString("yyyy-MM-dd");


            // ���ouser��Ƽҫ�
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

            // �P�_�O�_�ϥιw�]�Y��
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
            // ���oviewModel�s�J��Ʈw
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

            // ���o�ɮצs�J���w��Ƨ�
            new userFactory().saveAvatarToFolder(user, avatarFile);

            return RedirectToAction("Index", "Home");
        }



        //Common�����e��start
        public ActionResult Index()
        {
            return View();
        }
        //Common�����e��end


        public ActionResult MyshowPag()
        {
            return View();
        }
        

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