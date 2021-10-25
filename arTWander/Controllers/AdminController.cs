using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace arTWander.Controllers
{
    public class AdminController : Controller
    {
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