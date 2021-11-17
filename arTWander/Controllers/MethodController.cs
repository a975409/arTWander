using arTWander.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace arTWander.Controllers
{
    public class MethodController : Controller
    {
        public MethodController()
        {
        }

        public MethodController(ApplicationDbContext dbContext)
        {
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

        public ActionResult GetCities()
        {
            ViewCity[] cities = City_District.getCities(DbContext);

            return Json(cities, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetDistricts(int cityId)
        {
            var districts = await City_District.getDistricts(cityId, DbContext);

            return Json(districts, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AutocompleteKeywords(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
                return Json(new string[] { });

            var result = DbContext.Keywords.Where(m => m.Name.Contains(keyword)).Select(m=>m.Name);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}