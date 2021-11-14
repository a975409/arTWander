using arTWander.Models.AdminViewModel;
using Microsoft.AspNet.Identity.EntityFramework;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;

namespace arTWander.Models.AdminFactory
{

    public class AdminFactory
    {
        private ApplicationDbContext _dbContext;

        public AdminFactory(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private const int pageSize = 8;

        //取得當前user資料 in ApplicationUser
        public ApplicationUser getUserById(int id)
        {
            ApplicationUser userData = (new ApplicationDbContext()).Users.Where(u => u.Id == id).FirstOrDefault();
            return userData;
        }

        //新增Avatar檔案
        public void saveAvatarToFolder(ApplicationUser user, HttpPostedFileBase avatarFile)
        {
            if (avatarFile != null)
            {
                bool fileValid = true;
                if (avatarFile.ContentLength <= 0 || avatarFile.ContentLength > 5242880)
                {
                    fileValid = false;
                }

                if (fileValid == true)
                {
                    //取得副檔名
                    //string extension = Path.GetExtension(avatarFile.FileName);
                    //設定檔名
                    string fileName = user.Avatar;
                    string savePath = Path.Combine(HttpContext.Current.Server.MapPath("~/SaveFiles/Admin/Avatar"), fileName);
                    avatarFile.SaveAs(savePath);
                }
            }
        }

        //刪除Avatar舊檔
        public void DeleteAvatarFromFolder(string originAvatar)
        {
            if (!string.IsNullOrEmpty(originAvatar))
            {
                string deletePath = Path.Combine(HttpContext.Current.Server.MapPath("~/SaveFiles/Admin/Avatar"), originAvatar);
                //string path = $"~/SaveFiles/Admin/Avatar/{originAvatar}";
                bool result = File.Exists(deletePath);
                if (result == true)
                {
                    File.Delete(deletePath);
                }
            }

        }

        //取得所有user
        public IPagedList<UserListViewModel> GetUserListAll(int page=1)
        {
            //var db = new ApplicationDbContext();
            var CompanyId = _dbContext.Company.Select(m => m.FK_ApplicationUser);
            var blackListFkId = _dbContext.BlackList.Select(m=>m.FK_ApplicationUser);
            List<UserListViewModel> model = new List<UserListViewModel>();
            //string Bday = "";
            string blist = "";
            var AllUserId = _dbContext.Users.OrderBy(m => m.Id).Select(m => m.Id).ToList();

            //判斷是否有搜尋字串
            bool xx;
            foreach (var userId in AllUserId)
            {
                xx = blackListFkId.Contains(userId);
                if (blackListFkId.Contains(userId) ==false) { blist = "正常"; }
            else { blist = "黑名單"; }
            //left join +into ps from login in ps.DefaultIfEmpty()
            //var q = from users in db.Users
            //        join login in db.LogingLog on users.Id equals login.FK_ApplicationUser into ps
            //        from login in ps.DefaultIfEmpty()
            //        join bList in db.BlackList on users.Id equals bList.FK_ApplicationUser into pu
            //        from bList in pu.DefaultIfEmpty()
            //        join cCompany in db.Company on users.Id equals cCompany.FK_ApplicationUser into cu
            //        from cCompany in cu.DefaultIfEmpty()
            //        where users.Id == userId && bList.FK_ApplicationUser == null && cCompany.FK_ApplicationUser != userId
            //        select new UserListViewModel
            //        {
            //            FK_ApplicationUser = users.Id,
            //            users = users,
            //            PhoneNumber = users.PhoneNumber,
            //            UserName = users.UserName,
            //            Birthday = Bday,
            //            AccountAddress = users.AccountAddress,
            //            AvatarUrl = "/SaveFiles/Admin/Avatar/",
            //            AvatarName = users.Avatar,
            //            Email = users.Email,
            //            RegisterTime = login.RegisterTime,
            //            LastloginTime = login.LastloginTime,
            //            LoginOutTime = login.LoginOutTime,
            //            LogingCount = login.LogingCount,
            //            IsBlackList = blist,
            //            //Title = sPage.Title,
            //            //Comment = sComment.Comment,
            //            //Star = sComment.Star
            //        };

            //        join login in db.LogingLog on users.Id equals login.FK_ApplicationUser into ps
            //        from login in ps.DefaultIfEmpty()

            var q = _dbContext.Users.Where(m=>m.Id == userId && blist == "正常" && !CompanyId.Contains(userId)).GroupJoin(_dbContext.LogingLog, m=>m.Id, l=>l.FK_ApplicationUser, (m,l) => new { _m = m, _l=l}).SelectMany(x => x._l.DefaultIfEmpty(), (x, y) => new { user = x._m, log = y }).Select(ml => new UserListViewModel
                {
                    FK_ApplicationUser = ml.user.Id,
                    users = ml.user,
                    PhoneNumber = ml.user.PhoneNumber,
                    UserName = ml.user.UserName,
                    Birthday = ml.user.Birthday,
                    AccountAddress = ml.user.AccountAddress,
                    AvatarUrl = "/SaveFiles/Admin/Avatar/",
                    AvatarName = ml.user.Avatar,
                    Email = ml.user.Email,
                    RegisterTime = ml.log.RegisterTime,
                    LastloginTime = ml.log.LastloginTime,
                    LoginOutTime = ml.log.LoginOutTime,
                    LogingCount = ml.log.LogingCount,
                    IsBlackList = blist,
                });

                List<UserListViewModel> viewmodel = q.ToList();
                model.AddRange(viewmodel);
            }

            var userPages = OtherMethod.getCurrentPagedList(model, page, pageSize);
            return userPages;
        }


        public IEnumerable<UserListViewModel> GetUserListBySearch(string searchWord, int page = 1)
        {
           
            //var db = new ApplicationDbContext();
            var CompanyId = _dbContext.Company.Select(m => m.FK_ApplicationUser);
            var blackListFkId = _dbContext.BlackList.Select(m => m.FK_ApplicationUser);
            List<UserListViewModel> model = new List<UserListViewModel>();
            //string Bday = "";
            string blist = "";
            var AllUserId = _dbContext.Users.Where(m=> m.Email.Contains(searchWord) || m.UserName.Contains(searchWord)).OrderBy(m => m.Id).Select(m => m.Id).ToList();

            //判斷是否有搜尋字串
            bool xx;
            foreach (var userId in AllUserId)
            {
                xx = blackListFkId.Contains(userId);
                if (blackListFkId.Contains(userId) == false) { blist = "正常"; }
                else { blist = "黑名單"; }
                
                var q = _dbContext.Users.Where(m => m.Id == userId && blist == "正常" && !CompanyId.Contains(userId)).GroupJoin(_dbContext.LogingLog, m => m.Id, l => l.FK_ApplicationUser, (m, l) => new { _m = m, _l = l }).SelectMany(x => x._l.DefaultIfEmpty(), (x, y) => new { user = x._m, log = y }).Select(ml => new UserListViewModel
                {
                    FK_ApplicationUser = ml.user.Id,
                    users = ml.user,
                    PhoneNumber = ml.user.PhoneNumber,
                    UserName = ml.user.UserName,
                    Birthday = ml.user.Birthday,
                    AccountAddress = ml.user.AccountAddress,
                    AvatarUrl = "/SaveFiles/Admin/Avatar/",
                    AvatarName = ml.user.Avatar,
                    Email = ml.user.Email,
                    RegisterTime = ml.log.RegisterTime,
                    LastloginTime = ml.log.LastloginTime,
                    LoginOutTime = ml.log.LoginOutTime,
                    LogingCount = ml.log.LogingCount,
                    IsBlackList = blist,
                });
                List<UserListViewModel> viewmodel = q.ToList();
                model.AddRange(viewmodel);
            }
            var userPages = OtherMethod.getCurrentPagedList(model, page, pageSize);
            return userPages;
        }



        public IEnumerable<UserListViewModel> GetUserInformById(string searchWord)
        {
            var db = new ApplicationDbContext();
            List<UserListViewModel> model = new List<UserListViewModel>();
            string blist = "";

            //判斷是否黑名單
            var black = db.BlackList.Where(m => m.FK_ApplicationUser.ToString() == searchWord).FirstOrDefault();

            if (black == null)
            { blist = "正常"; }
            else { blist = "黑名單"; }
            //left join +into ps from login in ps.DefaultIfEmpty()
            var q = from users in db.Users
                    join login in db.LogingLog on users.Id equals login.FK_ApplicationUser into ps
                    from login in ps.DefaultIfEmpty()
                    join bList in db.BlackList on users.Id equals bList.FK_ApplicationUser into pu
                    from bList in pu.DefaultIfEmpty()
                    where users.Id.ToString() == searchWord
                    select new UserListViewModel
                    {
                        FK_ApplicationUser = users.Id,
                        users = users,
                        PhoneNumber = users.PhoneNumber,
                        UserName = users.UserName,
                        //Birthday = users.Birthday.ToString(),
                        AccountAddress = users.AccountAddress,
                        AvatarUrl = "/SaveFiles/Admin/Avatar/",
                        AvatarName = users.Avatar,
                        Email = users.Email,
                        RegisterTime = login.RegisterTime,
                        LastloginTime = login.LastloginTime,
                        LoginOutTime = login.LoginOutTime,
                        LogingCount = login.LogingCount,
                        IsBlackList = blist,
                        //Title = sPage.Title,
                        //Comment = sComment.Comment,
                        //Star = sComment.Star
                    };
            List<UserListViewModel> viewmodel = q.ToList();
            model.AddRange(viewmodel);
            return model;
        }



        //取得所有展演單位
        public IPagedList<CustomerListViewModel> GetCustomerListAll(int page)
        {
            var db = new ApplicationDbContext();
            var role = new ApplicationUserRole();
            BlackList blackList = new BlackList();
            List<CustomerListViewModel> model = new List<CustomerListViewModel>();
            string Bday = "";
            string blist = "";
            var AllUserId = db.Users.OrderBy(m => m.Id).Select(m => m.Id).ToList();
            //判斷是否有搜尋字串

            foreach (var userId in AllUserId)
            {
                if (string.IsNullOrEmpty(blackList.ToString())) { blist = "正常"; }
                else { blist = "黑名單"; }
                //left join +into ps from login in ps.DefaultIfEmpty()
                //var roleId = from r in role where r.UserId == userId select r;
                var companyId = db.Company.Where(m => m.FK_ApplicationUser == userId).Select(m => m.Id).FirstOrDefault();
                var count = db.ShowPage.Where(m => m.FK_Company == companyId).Select(m => m.Id).Count();
                var q = from cCompany in db.Company
                        join users in db.Users on cCompany.FK_ApplicationUser equals users.Id
                        join login in db.LogingLog on users.Id equals login.FK_ApplicationUser into ps
                        from login in ps.DefaultIfEmpty()
                            //join sComment in db.ShowComment on users.Id equals sComment.FK_ApplicationUser
                            //join sPage in db.ShowPage on cCompany.Id equals sPage.FK_Company
                        join bList in db.BlackList on users.Id equals bList.FK_ApplicationUser into pu
                        from bList in pu.DefaultIfEmpty()

                            //join uRole in roles on users.Id equals roles.UserId
                            //join roles in db.Roles on 
                        where users.Id == userId && bList.FK_ApplicationUser == null && cCompany.FK_ApplicationUser == userId
                        select new CustomerListViewModel
                        {
                            CompanyName = cCompany.CompanyName,
                            CompanyDescription = cCompany.CompanyDescription,
                            Fax = cCompany.Fax,
                            FK_ApplicationUser = users.Id,
                            PhoneNumber = users.PhoneNumber,
                            UserName = users.UserName,
                            Birthday = Bday,
                            CompanyAddress = users.AccountAddress,
                            AvatarUrl = $"/SaveFiles/Company/1/Info/",
                            AvatarName = cCompany.PhotoStickerImage,
                            Email = users.Email,
                            RegisterTime = login.RegisterTime,
                            LastloginTime = login.LastloginTime,
                            LoginOutTime = login.LoginOutTime,
                            LogingCount = login.LogingCount,
                            IsBlackList = blist,
                            //Title = sPage.Title,
                            //Comment = sComment.Comment,
                            //Star = sComment.Star,
                            ShowCount = count
                        };
                List<CustomerListViewModel> viewmodel = q.ToList();
                model.AddRange(viewmodel);
            }
            var userPages = OtherMethod.getCurrentPagedList(model, page, pageSize);
            return userPages;
        }

        //取得搜尋展演單位
        public IPagedList<CustomerListViewModel> GetCustomerBySearch(string searchWord, int page)
        {
            var db = new ApplicationDbContext();
            BlackList blackList = new BlackList();
            List<CustomerListViewModel> model = new List<CustomerListViewModel>();

            string Bday = "";
            string blist = "";
            //判斷是否有搜尋字串
            var AllUserId = db.Users.OrderBy(m => m.Id).Select(m => m.Id).ToList();
            if (string.IsNullOrEmpty(blackList.ToString())) { blist = "正常"; }
            else { blist = "黑名單"; }
            //left join +into ps from login in ps.DefaultIfEmpty()
            foreach (var userId in AllUserId)
            {
                var companyId = db.Company.Where(m => m.FK_ApplicationUser == userId).Select(m => m.Id).FirstOrDefault();
                var count = db.ShowPage.Where(m => m.FK_Company == companyId).Select(m => m.Id).Count();
                var q = from cCompany in db.Company
                        join users in db.Users on cCompany.FK_ApplicationUser equals users.Id
                        join login in db.LogingLog on users.Id equals login.FK_ApplicationUser into ps
                        from login in ps.DefaultIfEmpty()
                        join bList in db.BlackList on users.Id equals bList.FK_ApplicationUser into pu
                        from bList in pu.DefaultIfEmpty()
                        join City in db.City on cCompany.FK_City equals City.Id
                        join District in db.District on cCompany.FK_District equals District.Id
                        where users.Id == userId && bList.FK_ApplicationUser == null && cCompany.FK_ApplicationUser == userId
                        where cCompany.CompanyName.Contains(searchWord) || users.Email.Contains(searchWord)
                        select new CustomerListViewModel
                        {
                            CompanyName = cCompany.CompanyName,
                            CompanyDescription = cCompany.CompanyDescription,
                            Fax = cCompany.Fax,
                            FK_ApplicationUser = users.Id,
                            PhoneNumber = users.PhoneNumber,
                            UserName = users.UserName,
                            Birthday = Bday,
                            CompanyAddress = users.AccountAddress,
                            AvatarUrl = $"/SaveFiles/Company/"+ users.Id,
                            AvatarName = "/Info/"+cCompany.PhotoStickerImage,
                            Email = users.Email,
                            RegisterTime = login.RegisterTime,
                            LastloginTime = login.LastloginTime,
                            LoginOutTime = login.LoginOutTime,
                            LogingCount = login.LogingCount,
                            IsBlackList = blist,
                            CityName = City.CityName,
                            DistrictName = District.DistrictName,
                            ShowCount = count
                        };
                List<CustomerListViewModel> viewmodel = q.ToList();
                model.AddRange(viewmodel);
            }
            var userPages = OtherMethod.getCurrentPagedList(model, page, pageSize);
            return userPages;
        }

        //取得展演單位詳細資訊
        public IEnumerable<CustomerListViewModel> GetCustomerInformById(string searchWord)
        {
            var db = new ApplicationDbContext();
            List<CustomerListViewModel> model = new List<CustomerListViewModel>();
            string blist = "";

            //判斷是否黑名單
            var black = db.BlackList.Where(m => m.FK_ApplicationUser.ToString() == searchWord).FirstOrDefault();

            if (black == null)
            { blist = "正常"; }
            else { blist = "黑名單"; }
            //left join +into ps from login in ps.DefaultIfEmpty()
            var companyId = db.Company.Where(m => m.FK_ApplicationUser.ToString() == searchWord).Select(m => m.Id).FirstOrDefault();
            var count = db.ShowPage.Where(m => m.FK_Company == companyId).Select(m => m.Id).Count();

            var q = from cCompany in db.Company
                    join users in db.Users on cCompany.FK_ApplicationUser equals users.Id
                    join login in db.LogingLog on users.Id equals login.FK_ApplicationUser into ps
                    from login in ps.DefaultIfEmpty()
                    join bList in db.BlackList on users.Id equals bList.FK_ApplicationUser into pu
                    from bList in pu.DefaultIfEmpty()
                    join City in db.City on cCompany.FK_City equals City.Id
                    join District in db.District on cCompany.FK_District equals District.Id
                    where users.Id.ToString() == searchWord
                    select new CustomerListViewModel
                    {
                        Id=cCompany.Id,
                        CompanyName = cCompany.CompanyName,
                        CompanyDescription = cCompany.CompanyDescription,
                        Fax = cCompany.Fax,
                        FK_ApplicationUser = users.Id,
                        PhoneNumber = users.PhoneNumber,
                        UserName = users.UserName,
                        CompanyAddress = cCompany.Address,
                        AvatarUrl = $"/SaveFiles/Company/1/Info/",
                        AvatarName = cCompany.PhotoStickerImage,
                        Email = users.Email,
                        RegisterTime = login.RegisterTime,
                        LastloginTime = login.LastloginTime,
                        LoginOutTime = login.LoginOutTime,
                        LogingCount = login.LogingCount,
                        IsBlackList = blist,
                        CityName = City.CityName,
                        DistrictName = District.DistrictName,
                        ShowCount = count
                    };
            List<CustomerListViewModel> viewmodel = q.ToList();
            model.AddRange(viewmodel);
            return model;
        }




        //黑名單用戶
        public IPagedList<arTWander.Models.AdminViewModel.BlackListViewModel> GetBlackUserAll(int page)
        {
            var db = new ApplicationDbContext();
            BlackList blackList = new BlackList();
            List<BlackListViewModel> model = new List<BlackListViewModel>();

            string Bday = "";
            string blist = "";
            var AllUserId = db.Users.OrderBy(m => m.Id).Select(m => m.Id).ToList();

            //判斷是否有搜尋字串

            foreach (var userId in AllUserId)
            {
                if (string.IsNullOrEmpty(blackList.ToString())) { blist = "正常"; }
                else { blist = "黑名單"; }
                //left join +into ps from login in ps.DefaultIfEmpty()
                var q = from users in db.Users
                        join login in db.LogingLog on users.Id equals login.FK_ApplicationUser into ps
                        from login in ps.DefaultIfEmpty()
                        //join sComment in db.ShowComment on users.Id equals sComment.FK_ApplicationUser into pt
                        //from sComment in pt.DefaultIfEmpty()
                        //join sPage in db.ShowPage on sComment.FK_ShowPage equals sPage.Id
                        join bList in db.BlackList on users.Id equals bList.FK_ApplicationUser into pu
                        from bList in pu.DefaultIfEmpty()
                        join cCompany in db.Company on users.Id equals cCompany.FK_ApplicationUser into cu
                        from cCompany in cu.DefaultIfEmpty()
                        where users.Id == userId && bList.FK_ApplicationUser != null && cCompany.FK_ApplicationUser != userId
                        select new BlackListViewModel
                        {
                            FK_ApplicationUser = users.Id,
                            PhoneNumber = users.PhoneNumber,
                            UserName = users.UserName,
                            Birthday = Bday,
                            AccountAddress = users.AccountAddress,
                            AvatarUrl = "/SaveFiles/Admin/Avatar/",
                            AvatarName = users.Avatar,
                            Email = users.Email,
                            RegisterTime = login.RegisterTime,
                            LastloginTime = login.LastloginTime,
                            LoginOutTime = login.LoginOutTime,
                            LogingCount = login.LogingCount,
                            IsBlackList = blist,
                            Id = bList.Id,
                            Reason = bList.Reason,
                            Created_At = bList.Created_At
                        };
                List<BlackListViewModel> viewmodel = q.ToList();
                model.AddRange(viewmodel);
            }
            var userPages = OtherMethod.getCurrentPagedList(model, page, pageSize);
            return userPages;
        }

        public IPagedList<BlackListViewModel> GetBlackUserBySearch(string searchWord, int page)
        {
            var db = new ApplicationDbContext();
            BlackList blackList = new BlackList();
            List<BlackListViewModel> model = new List<BlackListViewModel>();

            string Bday = "";
            string blist = "";
            //判斷是否有搜尋字串
            if (string.IsNullOrEmpty(blackList.ToString())) { blist = "正常"; }
            else { blist = "黑名單"; }
            //left join +into ps from login in ps.DefaultIfEmpty()
            var q = from users in db.Users
                    join login in db.LogingLog on users.Id equals login.FK_ApplicationUser into ps
                    from login in ps.DefaultIfEmpty()
                    //join sComment in db.ShowComment on users.Id equals sComment.FK_ApplicationUser into pt
                    //from sComment in pt.DefaultIfEmpty()
                    //join sPage in db.ShowPage on sComment.FK_ShowPage equals sPage.Id
                    join bList in db.BlackList on users.Id equals bList.FK_ApplicationUser into pu
                    from bList in pu.DefaultIfEmpty()
                    join cCompany in db.Company on users.Id equals cCompany.FK_ApplicationUser into cu
                    from cCompany in cu.DefaultIfEmpty()
                    where (users.UserName.Contains(searchWord) ||
                    users.Email.Contains(searchWord)) && bList.FK_ApplicationUser != null && cCompany.FK_ApplicationUser != users.Id
                    select new BlackListViewModel
                    {
                        FK_ApplicationUser = users.Id,
                        PhoneNumber = users.PhoneNumber,
                        UserName = users.UserName,
                        Birthday = Bday,
                        AccountAddress = users.AccountAddress,
                        AvatarUrl = "/SaveFiles/Admin/Avatar/",
                        AvatarName = users.Avatar,
                        Email = users.Email,
                        RegisterTime = login.RegisterTime,
                        LastloginTime = login.LastloginTime,
                        LoginOutTime = login.LoginOutTime,
                        LogingCount = login.LogingCount,
                        IsBlackList = blist,
                        Id = bList.Id,
                        Reason = bList.Reason,
                        Created_At = bList.Created_At
                    };
            List<BlackListViewModel> viewmodel = q.ToList();
            model.AddRange(viewmodel);
            var userPages = OtherMethod.getCurrentPagedList(model, page, pageSize);
            return userPages;
        }

        public IEnumerable<BlackListViewModel> GetBlackUserInformById(string searchWord)
        {
            var db = new ApplicationDbContext();
            BlackList blackList = new BlackList();
            List<BlackListViewModel> model = new List<BlackListViewModel>();

            //判斷是否黑名單
            var black = db.BlackList.Where(m => m.FK_ApplicationUser.ToString() == searchWord).FirstOrDefault();
            string blist = "";
            string Bday = "";
            if (black == null)
            { blist = "正常"; }
            else { blist = "黑名單"; }
            //left join +into ps from login in ps.DefaultIfEmpty()
            var q = from users in db.Users
                    join login in db.LogingLog on users.Id equals login.FK_ApplicationUser into ps
                    from login in ps.DefaultIfEmpty()
                    //join sComment in db.ShowComment on users.Id equals sComment.FK_ApplicationUser into pt
                    //from sComment in pt.DefaultIfEmpty()
                    //join sPage in db.ShowPage on sComment.FK_ShowPage equals sPage.Id
                    join bList in db.BlackList on users.Id equals bList.FK_ApplicationUser into pu
                    from bList in pu.DefaultIfEmpty()
                    where users.Id.ToString() == searchWord
                    select new BlackListViewModel
                    {
                        FK_ApplicationUser = users.Id,
                        PhoneNumber = users.PhoneNumber,
                        UserName = users.UserName,
                        Birthday = Bday,
                        AccountAddress = users.AccountAddress,
                        AvatarUrl = "/SaveFiles/Admin/Avatar/",
                        AvatarName = users.Avatar,
                        Email = users.Email,
                        RegisterTime = login.RegisterTime,
                        LastloginTime = login.LastloginTime,
                        LoginOutTime = login.LoginOutTime,
                        LogingCount = login.LogingCount,
                        IsBlackList = blist,
                        Id = bList.Id,
                        Reason = bList.Reason,
                        Created_At = bList.Created_At
                    };
            List<BlackListViewModel> viewmodel = q.ToList();
            model.AddRange(viewmodel);
            return model;
        }


        //取得所有黑名單展演單位
        public IPagedList<BlackListViewModel> GetBlackCustomerAll(int page)
        {
            var db = new ApplicationDbContext();
            var role = new ApplicationUserRole();
            BlackList blackList = new BlackList();
            List<BlackListViewModel> model = new List<BlackListViewModel>();
            //list.ToList();
            string Bday = "";
            string blist = "";
            var AllUserId = db.Users.OrderBy(m => m.Id).Select(m => m.Id).ToList();
            //判斷是否有搜尋字串

            foreach (var userId in AllUserId)
            {
                if (string.IsNullOrEmpty(blackList.ToString())) { blist = "正常"; }
                else { blist = "黑名單"; }
                //left join +into ps from login in ps.DefaultIfEmpty()
                //var roleId = from r in role where r.UserId == userId select r;
                var companyId = db.Company.Where(m => m.FK_ApplicationUser == userId).Select(m => m.Id).FirstOrDefault();
                var count = db.ShowPage.Where(m => m.FK_Company == companyId).Select(m => m.Id).Count();
                var q = from cCompany in db.Company
                        join users in db.Users on cCompany.FK_ApplicationUser equals users.Id
                        join login in db.LogingLog on users.Id equals login.FK_ApplicationUser into ps
                        from login in ps.DefaultIfEmpty()
                            //join sComment in db.ShowComment on users.Id equals sComment.FK_ApplicationUser
                            //join sPage in db.ShowPage on cCompany.Id equals sPage.FK_Company
                        join bList in db.BlackList on users.Id equals bList.FK_ApplicationUser into pu
                        from bList in pu.DefaultIfEmpty()

                            //join uRole in roles on users.Id equals roles.UserId
                            //join roles in db.Roles on 
                        where users.Id == userId && bList.FK_ApplicationUser != null && cCompany.FK_ApplicationUser == userId
                        select new BlackListViewModel
                        {
                            CompanyName = cCompany.CompanyName,
                            CompanyDescription = cCompany.CompanyDescription,
                            Fax = cCompany.Fax,
                            FK_ApplicationUser = users.Id,
                            PhoneNumber = users.PhoneNumber,
                            UserName = users.UserName,
                            Birthday = Bday,
                            CompanyAddress = users.AccountAddress,
                            AvatarUrl = $"/SaveFiles/Company/1/Info/",
                            AvatarName = cCompany.PhotoStickerImage,
                            Email = users.Email,
                            RegisterTime = login.RegisterTime,
                            LastloginTime = login.LastloginTime,
                            LoginOutTime = login.LoginOutTime,
                            LogingCount = login.LogingCount,
                            IsBlackList = blist,
                            ShowCount = count,
                            Id = bList.Id,
                            Reason = bList.Reason,
                            Created_At = bList.Created_At
                        };
                List<BlackListViewModel> viewmodel = q.ToList();
                model.AddRange(viewmodel);
            }
            var userPages = OtherMethod.getCurrentPagedList(model, page, pageSize);
            return userPages;
        }

        //取得展演黑名單詳細資訊
        public IEnumerable<BlackListViewModel> GetBlackCustomerInformById(string searchWord)
        {
            var db = new ApplicationDbContext();
            List<BlackListViewModel> model = new List<BlackListViewModel>();
            string blist = "";
            string Bday = "";
            //判斷是否黑名單
            var black = db.BlackList.Where(m => m.FK_ApplicationUser.ToString() == searchWord).FirstOrDefault();

            if (black == null)
            { blist = "正常"; }
            else { blist = "黑名單"; }
            //left join +into ps from login in ps.DefaultIfEmpty()
            var companyId = db.Company.Where(m => m.FK_ApplicationUser.ToString() == searchWord).Select(m => m.Id).FirstOrDefault();
            var count = db.ShowPage.Where(m => m.FK_Company == companyId).Select(m => m.Id).Count();

            var q = from cCompany in db.Company
                    join users in db.Users on cCompany.FK_ApplicationUser equals users.Id
                    join login in db.LogingLog on users.Id equals login.FK_ApplicationUser into ps
                    from login in ps.DefaultIfEmpty()
                    join bList in db.BlackList on users.Id equals bList.FK_ApplicationUser into pu
                    from bList in pu.DefaultIfEmpty()
                    join City in db.City on cCompany.FK_City equals City.Id
                    join District in db.District on cCompany.FK_District equals District.Id
                    where users.Id.ToString() == searchWord
                    select new BlackListViewModel
                    {
                        CompanyName = cCompany.CompanyName,
                        CompanyDescription = cCompany.CompanyDescription,
                        Fax = cCompany.Fax,
                        FK_ApplicationUser = users.Id,
                        PhoneNumber = users.PhoneNumber,
                        UserName = users.UserName,
                        Birthday = Bday,
                        CompanyAddress = users.AccountAddress,
                        AvatarUrl = $"/SaveFiles/Company/1/Info/",
                        AvatarName = cCompany.PhotoStickerImage,
                        Email = users.Email,
                        RegisterTime = login.RegisterTime,
                        LastloginTime = login.LastloginTime,
                        LoginOutTime = login.LoginOutTime,
                        LogingCount = login.LogingCount,
                        IsBlackList = blist,
                        ShowCount = count,
                        Id = bList.Id,
                        Reason = bList.Reason,
                        Created_At = bList.Created_At
                    };
            List<BlackListViewModel> viewmodel = q.ToList();
            model.AddRange(viewmodel);
           
            return model;
        }


        //取得展演資訊
        //取得所有user
        public IEnumerable<ShowListViewModel> GetShowListAll(int page)
        {
            List<ShowListViewModel> model = new List<ShowListViewModel>();
            //var showId = db.ShowPage.Select(m => m.Id).ToList();
            //foreach (var showid in showId)
            //{
            var showPage = _dbContext.ShowPage.OrderBy(m => m.Id).Select(m => new ShowListViewModel
            {
                Address = m.Address,
                CityName = m.City.CityName,
                DistrictName = m.District.DistrictName,
                AgeRange = m.AgeRange,
                Cost = m.Cost,
                Description = m.Description,
                EndDate = m.EndDate,
                EndTime = m.EndTime,
                Id = m.Id,
                Price = m.Price,
                Remark = m.Remark,
                StartDate = m.StartDate,
                StartTime = m.StartTime,
                Title = m.Title,
                Created_At = m.Created_At,
                CompanyName = m.Company.CompanyName,
                PhoneNumber = m.Company.Phone,
                UserName = m.Company.ApplicationUser.UserName,
                ShowImg = "/SaveFiles/Company/" + m.Company.Id + "/show/" + m.Id + "/" + m.ShowPageFiles.Select(n => n.fileName).FirstOrDefault()
                               });
                List<ShowListViewModel> showInfo = showPage.ToList();
                model.AddRange(showInfo);
            //}
            var showPages = OtherMethod.getCurrentPagedList(model, page, pageSize);
            return showPages;
        }


        public IEnumerable<ShowListViewModel> GetShowListBySearch(string searchWord, int page)
        {
            var db = new ApplicationDbContext();
            List<ShowListViewModel> model = new List<ShowListViewModel>();
            var showId = db.ShowPage.OrderBy(m=>m.Id).Select(m => m.Id).ToList();
            var showPage = from show in db.ShowPage
                               //join cCompany in db.Company on show.FK_Company equals cCompany.Id
                               //join sShowPageFile in db.ShowPageFile on show.Id equals sShowPageFile.FK_ShowPage
                               where show.Title.Contains(searchWord) || show.Company.CompanyName.Contains(searchWord) || show.City.CityName.Contains(searchWord)
                               orderby show.Id
                           select new ShowListViewModel
                           {
                               Address = show.Address,
                               CityName = show.City.CityName,
                               DistrictName = show.District.DistrictName,
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
                               Created_At = show.Created_At,
                               CompanyName = show.Company.CompanyName,
                               PhoneNumber = show.Company.Phone,
                               UserName = show.Company.ApplicationUser.UserName
                           };
            List<ShowListViewModel> showInfo = showPage.ToList();
                model.AddRange(showInfo);

            var showPages = OtherMethod.getCurrentPagedList(model, page, pageSize);
            return showPages;
        }


        //取得展演詳細資訊
        public IEnumerable<ShowListViewModel> GetShowInformById(string searchWord)
        {
            //var db = new ApplicationDbContext();
            List<ShowListViewModel> model = new List<ShowListViewModel>();
            //var showId = db.ShowPage.OrderBy(m => m.Id).Select(m => m.Id).ToList();
     
            var showPage = _dbContext.ShowPage.Where(m=>m.Id.ToString() == searchWord).OrderBy(m => m.Id).Select(m => new ShowListViewModel
            {
                Address = m.Address,
                CityName = m.City.CityName,
                DistrictName = m.District.DistrictName,
                AgeRange = m.AgeRange,
                Cost = m.Cost,
                Description = m.Description,
                EndDate = m.EndDate,
                EndTime = m.EndTime,
                Id = m.Id,
                Price = m.Price,
                Remark = m.Remark,
                StartDate = m.StartDate,
                StartTime = m.StartTime,
                Title = m.Title,
                Created_At = m.Created_At,
                CompanyName = m.Company.CompanyName,
                PhoneNumber = m.Company.Phone,
                UserName = m.Company.ApplicationUser.UserName,
                ShowImg = "/SaveFiles/Company/" + m.Company.Id + "/show/" + m.Id + "/" + m.ShowPageFiles.Select(n => n.fileName).FirstOrDefault()
            });
            List<ShowListViewModel> showInfo = showPage.ToList();
            model.AddRange(showInfo);

            return model;
        }
    }
}