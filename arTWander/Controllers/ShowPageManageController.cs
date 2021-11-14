using arTWander.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace arTWander.Controllers
{
    [Authorize(Roles = "Company")]
    public class ShowPageManageController : Controller
    {
        public ShowPageManageController() { }

        public ShowPageManageController(ApplicationUserManager userManager, ApplicationDbContext dbContext)
        {
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
        // GET: ShowPageManage
        public ActionResult Index(int page = 1)
        {
            int userId = User.Identity.GetUserId<int>();

            var company = new CompanyFactory(DbContext).GetCompany(userId);
            ViewBag.PhotoStickerImage = string.IsNullOrEmpty(company.PhotoStickerImage) ? "/image/avatar/頭像_展演單位.png" : $"/SaveFiles/Company/{company.Id}/Info/{company.PhotoStickerImage}";
            ViewBag.userName = company.CompanyName;

            if (company == null)
                return RedirectToAction("Edit", "Company");

            var model = new ShowPageFactory(DbContext).getCompanyShowPages(company, page);

            return View(model);
        }

        /// <summary>
        /// 進階（條件）搜尋展覽 & 關鍵字搜尋
        /// </summary>
        /// <param name="searchModel"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult getShowPages(SearchShowPagesViewModel searchModel, int page = 1, string keyword = "")
        {
            int userId = User.Identity.GetUserId<int>();
            var company = new CompanyFactory(DbContext).GetCompany(userId);

            if (company == null)
                return RedirectToAction("Edit", "Company");

            SearchShowPagesViewModel search;

            if (string.IsNullOrEmpty(keyword))
            {
                search = searchModel;
                TempData["SearchModel"] = search;
                TempData.Keep("SearchModel");
            }
            else
            {
                search = new SearchShowPagesViewModel
                {
                    Cost = CostStatus.none,
                    OrderSortField = OrderSortField.AllData
                };

                TempData["keyword"] = keyword;
                TempData.Keep("keyword");
            }

            var model = new ShowPageFactory(DbContext).getCompanyShowPages(company, page, search, keyword);
            return PartialView("~/Views/Shared/CompanyPartial/_ShowPagePartial.cshtml", model);
        }

        public ActionResult DisplayInfo(int showId)
        {
            var show = DbContext.ShowPage.Find(showId);

            if (show == null)
            {
                return HttpNotFound("該展覽不存在或已被移除");
            }

            ShowPageViewModel model = new ShowPageViewModel
            {
                Address = $"{show.City.CityName}{show.District.DistrictName}{show.Address}",
                AgeRange = show.AgeRange,
                Cost = show.Cost,
                Description = show.Description,
                EndDate = show.EndDate,
                EndTime = show.EndTime,
                Id = show.Id,
                Price = show.Price,
                Remark = show.Remark,
                StartDate = show.StartDate,
                StartTime = show.StartTime,
                Title = show.Title,
                Todays = show.PageToTodaysList?.Select(m => m.Today).ToArray(),
                Keywords = show.KeywordsList?.Select(m => m.Name).ToArray(),
                images = show.ShowPageFiles?.Select(m => $"/SaveFiles/Company/{show.Company.Id}/show/{show.Id}/{m.fileName}").ToArray(),
                ViewCount = (ulong)show.PageViewCounts?.Select(m => m.Count).Sum()
            };

            return View(model);
        }

        public ActionResult getShowPageComment(int showPageId)
        {
            var showPage = DbContext.ShowPage.Find(showPageId);

            if (showPage.ShowComments.Count <= 0)
            {
                return new EmptyResult();
            }

            var model = showPage.ShowComments.Select(m => new ShowCommentViewModel
            {
                showCommentId = m.Id,
                userComment = m.Comment,
                userName = m.ApplicationUser.UserName,
                userStar = m.Star,
                CommentDate = m.CommentDate
            });

            for (int i = 0; i < model.Count(); i++)
            {
                var response = DbContext.ResponseShowComment.Where(m => m.FK_ShowComment == model.ElementAt(i).showCommentId).FirstOrDefault();

                if (response != null)
                {
                    model.ElementAt(i).ResponseDate = response.ResponseDate;
                    model.ElementAt(i).CompanyComment = response.Comment;
                    model.ElementAt(i).CompanyName = response.Company.CompanyName;
                }
            }

            return PartialView("~/Views/Shared/CompanyPartial/_ShowCommentPartial.cshtml", model);
        }

        public ActionResult Create()
        {
            return View(new ShowPageEditViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ShowPageEditViewModel model, HttpPostedFileBase[] imgFiles)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string temp = model.Description.ToLower();

            if (temp.Contains("<script>") || temp.Contains("</script>"))
            {
                ModelState.AddModelError("Description", "該欄位疑似有出現不安全的程式碼");
                return View(model);
            }

            int userId = User.Identity.GetUserId<int>();
            var company = new CompanyFactory(DbContext).GetCompany(userId);

            //將展演資料新增至資料庫
            var showPage = new ShowPage
            {
                Address = model.Address,
                AgeRange = model.AgeRange,
                Cost = model.Cost,
                Description = model.Description,
                EndDate = model.EndDate,
                EndTime = model.EndTime,
                FK_City = model.FK_City,
                FK_Company = company.Id,
                FK_District = model.FK_District,
                Price = model.Price,
                Remark = model.Remark,
                StartDate = model.StartDate,
                StartTime = model.StartTime,
                Title = model.Title,
                Created_At = DateTime.Now
            };

            DbContext.ShowPage.Add(showPage);
            await DbContext.SaveChangesAsync();

            //新增該展演對應的關鍵字
            await new ShowPageFactory(DbContext).insertKeywords(model.searchKeyword, showPage);

            //新增該展演對應的開放時段
            await new ShowPageFactory(DbContext).setOpenTodays(model.Todays, showPage);

            //上傳主視覺圖檔
            string saveDir = Path.Combine(Server.MapPath("~/SaveFiles/Company"), company.Id.ToString(), "show", showPage.Id.ToString());
            Directory.CreateDirectory(saveDir);

            await new ShowPageFactory(DbContext).SaveShowPageFiles(imgFiles, saveDir, showPage);

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int showId)
        {
            var show = DbContext.ShowPage.Find(showId);

            if (show == null)
                return RedirectToAction("Index");

            var model = new ShowPageEditViewModel
            {
                Id = showId,
                Address = show.Address,
                AgeRange = show.AgeRange,
                Cost = show.Cost,
                Description = show.Description,
                EndDate = show.EndDate,
                EndTime = show.EndTime,
                FK_City = show.FK_City,
                FK_District = show.FK_District,
                Price = show.Price,
                Remark = show.Remark,
                StartDate = show.StartDate,
                StartTime = show.StartTime,
                Title = show.Title,
                Todays = show.PageToTodaysList?.Select(m => m.Today).ToArray(),
                searchKeyword = string.Join(",", show.KeywordsList?.Select(m => m.Name).ToArray())
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ShowPageEditViewModel model, HttpPostedFileBase[] imgFiles)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string temp = model.Description.ToLower();

            if (temp.Contains("<script>") || temp.Contains("</script>"))
            {
                ModelState.AddModelError("Description", "該欄位疑似有出現不安全的程式碼");
                return View(model);
            }

            var showPage = DbContext.ShowPage.Find(model.Id);

            //更新展演資料
            showPage.Address = model.Address;
            showPage.AgeRange = model.AgeRange;
            showPage.Cost = model.Cost;
            showPage.Description = model.Description;
            showPage.EndDate = model.EndDate;
            showPage.EndTime = model.EndTime;
            showPage.FK_City = model.FK_City;
            showPage.FK_Company = showPage.Company.Id;
            showPage.FK_District = model.FK_District;
            showPage.Price = model.Price;
            showPage.Remark = model.Remark;
            showPage.StartDate = model.StartDate;
            showPage.StartTime = model.StartTime;
            showPage.Title = model.Title;
            await DbContext.SaveChangesAsync();

            //新增該展演對應的關鍵字
            showPage.KeywordsList.Clear();
            await DbContext.SaveChangesAsync();

            await new ShowPageFactory(DbContext).insertKeywords(model.searchKeyword, showPage);

            string saveDir = Path.Combine(Server.MapPath("~/SaveFiles/Company"), showPage.Company.Id.ToString(), "show", showPage.Id.ToString());

            Directory.CreateDirectory(saveDir);

            await new ShowPageFactory(DbContext).AddOrUpdateShowPageFiles(imgFiles, saveDir, showPage);
            return RedirectToAction("Index");
        }

        public ActionResult getShowImages(int showPageId) {

            var showPage = DbContext.ShowPage.Find(showPageId);
            var model = showPage.ShowPageFiles.Select(m => m);
            ViewBag.ImgDir = $"/SaveFiles/Company/{showPage.Company.Id}/show/{showPage.Id}/";

            return PartialView("~/Views/Shared/CompanyPartial/_showPageImagePartial.cshtml", model);
        }

        public async Task<ActionResult> removeShowImage(int imgId,int showPageId)
        {
            var showPage = DbContext.ShowPage.Find(showPageId);

            if(showPage==null)
                return HttpNotFound("該展覽不存在或已被移除");

            string saveDir = Path.Combine(Server.MapPath("~/SaveFiles/Company"), showPage.Company.Id.ToString(), "show", showPage.Id.ToString());

            var img = showPage.ShowPageFiles.Where(m => m.Id == imgId).FirstOrDefault();

            string imgPath = Path.Combine(saveDir, img.fileName);

            if (!Directory.Exists(saveDir))
            {
                return new HttpStatusCodeResult(200, "移除成功!");
            }

            if (img != null)
            {
                DbContext.ShowPageFile.Remove(img);
                await DbContext.SaveChangesAsync();
            }

            FileInfo fileInfo = new FileInfo(imgPath);

            if (fileInfo.Exists)
            {
                try
                {
                    fileInfo.Delete();
                    return new HttpStatusCodeResult(200, "移除成功!");
                }
                catch
                {
                    return new HttpStatusCodeResult(500);
                }
            }
            return new HttpStatusCodeResult(200, "移除成功!");
        }

        public async Task<ActionResult> Delete(int showPageId)
        {
            var show = DbContext.ShowPage.Find(showPageId);

            if (show == null)
            {
                return HttpNotFound("該展覽不存在或已被移除");
            }

            string saveDir = Path.Combine(Server.MapPath("~/SaveFiles/Company"), show.Company.Id.ToString(), "show", show.Id.ToString());

            if (Directory.Exists(saveDir))
            {
                try
                {
                    Directory.Delete(saveDir, true);
                }
                catch
                {
                    return new HttpStatusCodeResult(500, "移除失敗!");
                }
            }

            DbContext.ShowPage.Remove(show);

            try
            {
                await DbContext.SaveChangesAsync();
            }
            catch(Exception ex) {
                return new HttpStatusCodeResult(500, "移除失敗!");
            }
            return new HttpStatusCodeResult(200, "移除成功!");
        }
    }
}