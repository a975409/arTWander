using System.Net.Mime;
using System.Web.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Data.Entity;
using static arTWander.Models.CommonViewModel;
using System.Threading.Tasks;
using PagedList;

namespace arTWander.Models
{
<<<<<<< HEAD
    public class userFactory
    {
=======
  public class userFactory
  {
>>>>>>> 1b6f208bd47d16df1510d5083fc25768bc85f91a

        private ApplicationDbContext _dbContext;

        public userFactory(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private const int pageSize = 12;

        public ApplicationUser getUserById(int id)
        {
            var q = from nowUser in _dbContext.Users
                    where nowUser.Id == id
                    select nowUser;
            var person = q.ToList()[0];
            return person;
        }
<<<<<<< HEAD
=======

    public void updateToDB(ApplicationUser user, CommonInfoViewModel viewModel)
    {
      user.Avatar = viewModel.AvatarName;
      user.UserName = viewModel.UserName;
      user.Birthday = DateTime.Parse(viewModel.Birthday);
      user.AccountAddress = viewModel.AccountAddress;
      user.PhoneNumber = viewModel.PhoneNumber;
      user.TwoFactorEnabled = viewModel.TwoFactor;
    }
>>>>>>> 1b6f208bd47d16df1510d5083fc25768bc85f91a

        public void updateToDB(ApplicationUser user, CommonInfoViewModel viewModel)
        {
            user.Avatar = viewModel.AvatarName;
            user.UserName = viewModel.UserName;
            user.Birthday = DateTime.Parse(viewModel.Birthday);
            user.AccountAddress = viewModel.AccountAddress;
            user.PhoneNumber = viewModel.PhoneNumber;
            user.TwoFactorEnabled = viewModel.TwoFactor;
        }

        public void saveAvatarToFolder(ApplicationUser user, HttpPostedFileBase avatarFile)
        {
<<<<<<< HEAD
            if (avatarFile != null)
            {
                bool fileValid = true;
                if (avatarFile.ContentLength <= 0 || avatarFile.ContentLength > 5242880)
                {
                    fileValid = false;
                }

                if (fileValid == true)
                {
                    string fileName = user.Avatar;
                    string savePath = Path.Combine(HttpContext.Current.Server.MapPath("~/image/avatar"), fileName);
                    avatarFile.SaveAs(savePath);

                }
            }
        }

        public CommonInfoViewModel createViewModel(IndexViewModel model, int Userid)
        {
            // 取得user
            ApplicationDbContext db = new ApplicationDbContext();
            var user = db.Users.Where(u => u.Id == Userid).FirstOrDefault();

            // 轉換user.birthay型別以符合前端需求
            DateTime Birthday = (DateTime)(user.Birthday);
            string Bday = Birthday.ToString("yyyy-MM-dd");


            // 取得user資料模型
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


            return viewModel;
=======
          string fileName = user.Avatar;
          string savePath = Path.Combine(HttpContext.Current.Server.MapPath("~/image/avatar"), fileName);
          avatarFile.SaveAs(savePath);

>>>>>>> 1b6f208bd47d16df1510d5083fc25768bc85f91a
        }

      }
    }
        public IPagedList<CommonShowViewModel> getShowPages(int page = 1, SearchShowPagesViewModel model = null)
        {
            //依據搜尋條件取得該展演單位的展演
            var shows = OtherMethod.searchShowPage(_dbContext.ShowPage, model).Select(m => new CommonShowViewModel
            {
                showDiscription = m.Description,
                showCity = m.City.CityName,
                showId = m.Id,
                showTitle = m.Title,
                showImg = m.ShowPageFiles.Count() <= 0 ? "/image/exhibiton/Null.png" : $"/SaveFiles/Company/{m.Company.Id}/show/{m.Id}/{m.ShowPageFiles.FirstOrDefault().fileName}",
                showCompany = m.Company.CompanyName
            });

            var showPages = OtherMethod.getCurrentPagedList(shows, page, pageSize);

            return showPages;
        }

<<<<<<< HEAD
        public IQueryable<CommonShowViewModel> queryAllShow()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var q = from show in db.ShowPage
                        //join city in db.City on show.FK_City equals city.Id
                        //join company in db.Company on show.FK_Company equals company.Id
                    join showImg in db.ShowPageFile on show.Id equals showImg.FK_ShowPage
                    select new CommonShowViewModel
                    {
                        showCity = show.City.CityName,
                        showTitle = show.Title,
                        showDiscription = show.Description,
                        showCompany = show.Company.CompanyName,
                        showImg = showImg.fileName
                    };

            return q;
        }
=======
        public List<CommonShowViewModel> createMyShowList(int? cityId, int userId, ApplicationUser user)
        {
            List<CommonShowViewModel> myShowList = new List<CommonShowViewModel>();
            IEnumerable<ShowPage> ShowPages;

            if (cityId != null)
            {
                ShowPages = user.ShowPage.Where(p => p.City.Id == cityId).Select(x => x);
            }
            else
            {
                ShowPages = user.ShowPage;
            }

            int index = 0;
            foreach (var show in ShowPages)
            {
                // 取得此展覽之圖片list
                List<string> fileName = new List<string>();

                if (show.ShowPageFiles.Count() > 0)
                {
                    foreach (var file in show.ShowPageFiles)
                    {
                        fileName.Add($"/SaveFiles/Company/{show.Company.Id}/show/{show.Id}/{file.fileName}");
                    }
                }

                // 建立此展覽viewmodel
                CommonShowViewModel aShow = new CommonShowViewModel
                {
                    showCity = show.City.CityName,
                    showTitle = show.Title,
                    showDiscription = show.Description,
                    showCompany = show.Company.CompanyName,
                    showImg = fileName.Count() > 0 ? fileName[0] : "", // 預設選擇使用此展覽第一張圖片
                    showId = show.Id
                };

                // 加入喜愛展覽清單
                myShowList.Add(aShow);
                index++;

            }
            return myShowList;
        }

        public CommonMyShowViewNodel createMyShowViewNodel(List<City> cityList, List<CommonShowViewModel> myShowList)
        {
            CommonMyShowViewNodel viewModel = new CommonMyShowViewNodel
            {
                allCity = cityList,
                allShow = myShowList
            };

            return viewModel;
        }

        public CommonInfoViewModel createViewModel(IndexViewModel model, int Userid)
    {
      // 取得user
      ApplicationDbContext db = new ApplicationDbContext();
      var user = db.Users.Where(u => u.Id == Userid).FirstOrDefault();

      // 轉換user.birthay型別以符合前端需求
      DateTime Birthday = (DateTime)(user.Birthday);
      string Bday = Birthday.ToString("yyyy-MM-dd");


      // 取得user資料模型
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


      return viewModel;
    }

    public IQueryable<CommonShowViewModel> queryAllShow()
    {
      ApplicationDbContext db = new ApplicationDbContext();
      var q = from show in db.ShowPage
              //join city in db.City on show.FK_City equals city.Id
              //join company in db.Company on show.FK_Company equals company.Id
              join showImg in db.ShowPageFile on show.Id equals showImg.FK_ShowPage
              select new CommonShowViewModel
              {
                showCity = show.City.CityName,
                showTitle = show.Title,
                showDiscription = show.Description,
                showCompany = show.Company.CompanyName,
                showImg = showImg.fileName
              };

      return q;
>>>>>>> 1b6f208bd47d16df1510d5083fc25768bc85f91a
    }
}