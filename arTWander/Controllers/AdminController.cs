using arTWander.Models;
using arTWander.Models.AdminFactory;
using arTWander.Models.AdminViewModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace arTWander.Controllers
{
    public class AdminController : Controller
    {
        //    //如果要在其他Controller引用DbContext來對資料表做CRUD，請參考第17 ~33行新增DbContext
        //    // 詳細用法請參考底下 Index 這個 Action
        //    public AdminController(ApplicationUserManager userManager, ApplicationDbContext dbContext)
        //    {
        //        UserManager = userManager;
        //        DbContext = dbContext;
        //    }

        //    private ApplicationDbContext _dbContext;
        //    public ApplicationDbContext DbContext
        //    {
        //        get
        //        {
        //            return _dbContext ?? HttpContext.GetOwinContext().Get<ApplicationDbContext>();
        //        }
        //        private set
        //        {
        //            _dbContext = value;
        //        }
        //    }

        //private ApplicationUserManager _userManager;
        //public ApplicationUserManager UserManager
        //{
        //    get
        //    {
        //        return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
        //    }
        //    private set
        //    {
        //        _userManager = value;
        //    }
        //}

        // GET: Admin
        public ActionResult Index()
        {

            return View();
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
            return View();
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

            // 取得user
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
                AvatarUrl = "~/SaveFiles/Admin/Avatar",
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
            if (user != null)
            {
                user.Avatar = viewModel.AvatarName;
                Console.WriteLine(viewModel.AvatarName);
                user.UserName = viewModel.UserName;
                user.Birthday = DateTime.Parse(viewModel.Birthday);
                user.AccountAddress = viewModel.AccountAddress;
                user.PhoneNumber = viewModel.PhoneNumber;
                user.TwoFactorEnabled = viewModel.TwoFactor;
            }
            db.SaveChanges();

            // 取得檔案存入指定資料夾
            new AdminFactory().saveAvatarToFolder(user, avatarFile);

            return RedirectToAction("Index", "Admin");
        }
        //修改個人資料頁面end
    }
}