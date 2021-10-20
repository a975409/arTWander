using arTWander.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static arTWander.Models.CompanyViewModel;

namespace arTWander.Controllers
{
    public class CompanyController : Controller
    {
        //如果要在其他Controller引用DbContext來對資料表做CRUD，請參考第17~33行新增DbContext
        //詳細用法請參考底下 Index 這個 Action
        public CompanyController(ApplicationUserManager userManager, ApplicationDbContext dbContext) {
            UserManager = userManager;
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

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // GET: Company
        public async Task<ActionResult> Index(IndexViewModel model)
        {
            //取得登入的用戶Id
            int userId = User.Identity.GetUserId<int>();
            var user= await UserManager.FindByIdAsync(User.Identity.GetUserId<int>());

            //透過DbContext讀取Company資料表資料
            var company = DbContext.Company.Where(m => m.FK_ApplicationUser == userId).FirstOrDefault() ?? new Company();

            var viewModel = new CompanyInfoViewModel
            {
                Address = company.Address,
                BrowserRemembered = model.BrowserRemembered,
                CityName = company.City.CityName,
                CompanyDescription = company.CompanyDescription,
                CompanyName = company.CompanyName,
                DistrictName = company.District.DistrictName,
                Email = user.Email,
                HasPassword = model.HasPassword,
                Logins = model.Logins,
                PhoneNumber = model.PhoneNumber,
                TwoFactor = model.TwoFactor,
                companyLinks = company.CompanyLinks.Select(m => new CompanyLinkViewModel { Title = m.Title, link = m.link }).ToArray()
            };

            return View(viewModel);
        }
    }
}