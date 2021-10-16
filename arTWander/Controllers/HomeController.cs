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

        [Authorize]
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


            ModelState.AddModelError("error", "錯誤訊息測試");
            return View();
        }
    }
}
