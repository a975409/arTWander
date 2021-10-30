﻿using arTWander.Models;
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
        public ActionResult Index()
        {
            int userId = User.Identity.GetUserId<int>();
            var company = DbContext.Company.Where(m => m.FK_ApplicationUser == userId).FirstOrDefault();

            if (company == null)
                return RedirectToAction("Edit", "Company");

            if (company.ShowPages == null || company.ShowPages.Count <= 0)
            {
                return View(new List<ShowMinViewModel>());
            }

            var shows = company.ShowPages.Select(m => new ShowMinViewModel
            {
                Description = m.Description,
                cityName = m.City.CityName,
                Id = m.Id,
                Title = m.Title,
                Comment = m.ShowComments.Count(),
                fileName = m.ShowPageFiles.Count() <= 0 ? "/image/exhibiton/Null.png" : $"/SaveFiles/Company/{m.Company.Id}/show/{m.Id}/{m.ShowPageFiles.FirstOrDefault().fileName}"
            });

            return View(shows);
        }

        public ActionResult DisplayInfo(int showId)
        {
            var show = DbContext.ShowPage.Find(showId);

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
                images = show.ShowPageFiles?.Select(m => $"/SaveFiles/Company/{show.Company.Id}/show/{show.Id}/{m.fileName}").ToArray()
            };
            
            return View(model);
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

            int userId = User.Identity.GetUserId<int>();
            var company = DbContext.Company.Where(m => m.FK_ApplicationUser == userId).FirstOrDefault();

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
            if (!string.IsNullOrEmpty(model.searchKeyword))
            {
                foreach (string item in model.searchKeyword.Split(','))
                {
                    if (DbContext.Keywords.Where(m => m.Name == item).Any())
                    {
                        var keyword = DbContext.Keywords.Where(m => m.Name == item).FirstOrDefault();
                        showPage.KeywordsList.Add(keyword);
                    }
                    else
                    {
                        showPage.KeywordsList.Add(new Keywords { Name = item });
                    }
                }

                DbContext.ShowPage.Add(showPage);
                await DbContext.SaveChangesAsync();
            }

            //新增該展演對應的開放時段
            foreach (int item in model.Todays)
            {
                DbContext.PageToTodays.Add(new PageToTodays { FK_ShowPage = showPage.Id, Today = item });
            }
            await DbContext.SaveChangesAsync();

            //上傳主視覺圖檔
            string saveDir = Path.Combine(Server.MapPath("~/SaveFiles/Company"), company.Id.ToString(), "show", showPage.Id.ToString());
            Directory.CreateDirectory(saveDir);

            if (imgFiles != null && imgFiles.Length > 0)
            {
                //圖片檔名
                int count = 1;
                foreach (var item in imgFiles)
                {
                    if (item != null && item.ContentLength > 0)
                    {
                        byte[] ImageData = new byte[item.ContentLength];
                        item.InputStream.Read(ImageData, 0, item.ContentLength);

                        using (MemoryStream stream = new MemoryStream(ImageData))
                        {
                            //判斷上傳的檔案是否為圖片檔
                            if (IsImage(stream))
                            {
                                //取得副檔名
                                string extension = Path.GetExtension(item.FileName);

                                //設定檔名
                                string img = count + extension;

                                //完整另存路徑
                                string savePath = Path.Combine(saveDir, img);

                                //server端下載檔案
                                item.SaveAs(savePath);

                                DbContext.ShowPageFile.Add(new ShowPageFile { fileName = img, FK_ShowPage = showPage.Id });

                                count++;
                            }
                        }
                    }
                }
                await DbContext.SaveChangesAsync();
            }
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
                Title = show.Title
            };
            string dirPath = $"/SaveFiles/Company/{show.Company.Id}/show/{show.Id}";

            ViewBag.Images = show.ShowPageFiles.Select(m => $"{dirPath}/{m.fileName}");

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

            int userId = User.Identity.GetUserId<int>();
            var company = DbContext.Company.Where(m => m.FK_ApplicationUser == userId).FirstOrDefault();
            var showPage = DbContext.ShowPage.Find(model.Id);

            showPage.Address = model.Address;
            showPage.AgeRange = model.AgeRange;
            showPage.Cost = model.Cost;
            showPage.Description = model.Description;
            showPage.EndDate = model.EndDate;
            showPage.EndTime = model.EndTime;
            showPage.FK_City = model.FK_City;
            showPage.FK_Company = company.Id;
            showPage.FK_District = model.FK_District;
            showPage.Price = model.Price;
            showPage.Remark = model.Remark;
            showPage.StartDate = model.StartDate;
            showPage.StartTime = model.StartTime;
            showPage.Title = model.Title;
            await DbContext.SaveChangesAsync();

            string saveDir = Path.Combine(Server.MapPath("~/SaveFiles/Company"), company.Id.ToString(), "show", showPage.Id.ToString());
            Directory.CreateDirectory(saveDir);

            if (imgFiles != null && imgFiles.Length > 0)
            {
                //圖片檔名
                int count = 1;
                foreach (var item in imgFiles)
                {
                    if (item != null && item.ContentLength > 0)
                    {
                        byte[] ImageData = new byte[item.ContentLength];
                        item.InputStream.Read(ImageData, 0, item.ContentLength);

                        using (MemoryStream stream = new MemoryStream(ImageData))
                        {
                            //判斷上傳的檔案是否為圖片檔
                            if (IsImage(stream))
                            {
                                //取得副檔名
                                string extension = Path.GetExtension(item.FileName);

                                //設定檔名
                                string img = count + extension;

                                //完整另存路徑
                                string savePath = Path.Combine(saveDir, img);

                                //server端下載檔案
                                item.SaveAs(savePath);

                                DbContext.ShowPageFile.Add(new ShowPageFile { fileName = img, FK_ShowPage = showPage.Id });

                                count++;
                            }
                        }
                    }
                }
                await DbContext.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        private bool IsImage(Stream stream)
        {
            try
            {
                System.Drawing.Image img = System.Drawing.Image.FromStream(stream);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}