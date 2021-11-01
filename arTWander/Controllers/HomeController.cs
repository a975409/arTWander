using System.Collections.Generic;
using System.Web.Mvc;

namespace arTWander.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult homeIndexPage() {
            return View();
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult test() {

            return View();
        }

        [HttpPost]
        public ActionResult test(List<Service> Service) {

            return Json(Service);
        }
    }

    public class Service { 
        public string Name { get; set; }
        public string Price { get; set; }
    }
}
