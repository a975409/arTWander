using arTWander.Models;
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
    }
}