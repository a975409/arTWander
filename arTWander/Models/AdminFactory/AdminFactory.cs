using arTWander.Models.AdminViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace arTWander.Models.AdminFactory
{
    public class AdminFactory
    {
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


        public IEnumerable<UserListViewModel> GetUserListAll()
        {
            var db = new ApplicationDbContext();
            BlackList blackList = new BlackList();
            List<UserListViewModel> model = new List<UserListViewModel>();

            string Bday = "";
            string nodata = "";
            string blist = "";
            var AllUserId = db.Users.Select(m => m.Id).ToList();

            //判斷是否有搜尋字串

            foreach (var userId in AllUserId)
            {
                if (string.IsNullOrEmpty(blackList.ToString())) { blist = "正常"; }
                else { blist = "黑名單"; }
                //left join +into ps from login in ps.DefaultIfEmpty()
                var q = from users in db.Users
                        join login in db.LogingLog on users.Id equals login.FK_ApplicationUser into ps
                        from login in ps.DefaultIfEmpty()
                        join sComment in db.ShowComment on users.Id equals sComment.FK_ApplicationUser into pt
                        from sComment in pt.DefaultIfEmpty()
                        join sPage in db.ShowPage on sComment.FK_ShowPage equals sPage.Id
                        join bList in db.BlackList on users.Id equals bList.FK_ApplicationUser into pu
                        from bList in pu.DefaultIfEmpty()
                        where users.Id == userId
                        select new UserListViewModel
                        {
                            FK_ApplicationUser = users.Id,
                            users = users,
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
                            Title = sPage.Title,
                            Comment = sComment.Comment,
                            Star = sComment.Star
                        };
                List<UserListViewModel> viewmodel = q.ToList();
                model.AddRange(viewmodel);
            }
            return model;
        }


        public IEnumerable<UserListViewModel> GetUserListBySearch(string searchWord)
        {
            var db = new ApplicationDbContext();
            BlackList blackList = new BlackList();
            List<UserListViewModel> model = new List<UserListViewModel>();

            string Bday = "";
            string nodata = "";
            string blist = "";
            //判斷是否有搜尋字串
            if (string.IsNullOrEmpty(blackList.ToString())) { blist = "正常"; }
            else { blist = "黑名單"; }
            //left join +into ps from login in ps.DefaultIfEmpty()
            var q = from users in db.Users
                    join login in db.LogingLog on users.Id equals login.FK_ApplicationUser into ps
                    from login in ps.DefaultIfEmpty()
                    join sComment in db.ShowComment on users.Id equals sComment.FK_ApplicationUser into pt
                    from sComment in pt.DefaultIfEmpty()
                    join sPage in db.ShowPage on sComment.FK_ShowPage equals sPage.Id
                    join bList in db.BlackList on users.Id equals bList.FK_ApplicationUser into pu
                    from bList in pu.DefaultIfEmpty()
                    where users.UserName.Contains(searchWord) || 
                    users.Email.Contains(searchWord)
                    //login.RegisterTime.ToString().Contains(searchWord)||
                    //users.Id.ToString().Contains(searchWord)
                    select new UserListViewModel
                    {
                        FK_ApplicationUser = users.Id,
                        users = users,
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
                        Title = sPage.Title,
                        Comment = sComment.Comment,
                        Star = sComment.Star
                    };
            List<UserListViewModel> viewmodel = q.ToList();
            model.AddRange(viewmodel);
            return model;
        }



        public IEnumerable<UserListViewModel> GetUserInformById(string searchWord)
        {
            var db = new ApplicationDbContext();
            List<UserListViewModel> model = new List<UserListViewModel>();
            string nodata;
            //string Bday = "";
            string blist = "";
            //判斷是否有搜尋字串
            var black = from b in db.BlackList where b.FK_ApplicationUser.ToString() == searchWord select new { nodata = b.FK_ApplicationUser.ToString() };
            
            if (string.IsNullOrEmpty(black.ToString())) 
            { blist = "正常"; }
            else { blist = "黑名單"; }
            //left join +into ps from login in ps.DefaultIfEmpty()
            var q = from users in db.Users
                    join login in db.LogingLog on users.Id equals login.FK_ApplicationUser into ps
                    from login in ps.DefaultIfEmpty()
                    join sComment in db.ShowComment on users.Id equals sComment.FK_ApplicationUser into pt
                    from sComment in pt.DefaultIfEmpty()
                    join sPage in db.ShowPage on sComment.FK_ShowPage equals sPage.Id
                    join bList in db.BlackList on users.Id equals bList.FK_ApplicationUser into pu
                    from bList in pu.DefaultIfEmpty()
                    where users.Id.ToString() == searchWord
                    //login.RegisterTime.ToString().Contains(searchWord)||
                    //users.Id.ToString().Contains(searchWord)
                    select new UserListViewModel
                    {
                        FK_ApplicationUser = users.Id,
                        users = users,
                        PhoneNumber = users.PhoneNumber,
                        UserName = users.UserName,
                        Birthday = users.Birthday.ToString(),
                        AccountAddress = users.AccountAddress,
                        AvatarUrl = "/SaveFiles/Admin/Avatar/",
                        AvatarName = users.Avatar,
                        Email = users.Email,
                        RegisterTime = login.RegisterTime,
                        LastloginTime = login.LastloginTime,
                        LoginOutTime = login.LoginOutTime,
                        LogingCount = login.LogingCount,
                        IsBlackList = blist,
                        Title = sPage.Title,
                        Comment = sComment.Comment,
                        Star = sComment.Star
                    };
            List<UserListViewModel> viewmodel = q.ToList();
            model.AddRange(viewmodel);
            return model;
        }


        //public List<UserListViewModel> GetUserInform()
        //{
        //    var db = new ApplicationDbContext();
        //    ApplicationUser userData = new ApplicationUser();
        //    LogingLog loginLog = new LogingLog();
        //    ShowComment showComment = new ShowComment();
        //    ShowPage showPage = new ShowPage();
        //    BlackList blackList = new BlackList();
        //    List<UserListViewModel> model = new List<UserListViewModel>();
        //    string blist = "";
        //    string Bday = "";
        //    string nodata = "";
        //    var AllUserId = db.Users.Select(m => m.Id).ToList();

        //    // 轉換user.birthay型別以符合前端需求
        //    if (!string.IsNullOrEmpty(userData.Birthday.ToString()))
        //    {
        //        DateTime Birthday = (DateTime)(userData.Birthday);
        //        Bday = Birthday.ToString("yyyy-MM-dd");
        //    }
        //    else
        //    {
        //        Bday = DateTime.Now.ToString("yyyy-MM-dd");
        //    }
        //    if (string.IsNullOrEmpty(blackList.ToString())) { blist = "正常"; }
        //    else { blist = "黑名單"; }
        //    if (string.IsNullOrEmpty(loginLog.RegisterTime.ToString()))
        //    {

        //    }
        //    if (string.IsNullOrEmpty(loginLog.LastloginTime.ToString()))
        //    {

        //    }
        //    if (string.IsNullOrEmpty(loginLog.LoginOutTime.ToString()))
        //    {

        //    }
        //    if (string.IsNullOrEmpty(loginLog.LogingCount.ToString()))
        //    {

        //    }
        //    if (string.IsNullOrEmpty(loginLog.Statue.ToString()))
        //    {

        //    }
        //    if (string.IsNullOrEmpty(loginLog.Statue.ToString()))
        //    {

        //    }
        //    if (string.IsNullOrEmpty(showPage.Title.ToString()))
        //    {

        //    }
        //    if (string.IsNullOrEmpty(showComment.Comment.ToString()))
        //    {

        //    }
        //    if (string.IsNullOrEmpty(showComment.Star.ToString()))
        //    {

        //    }

        //    foreach (var userId in AllUserId)
        //    {
        //        userData = db.Users.Where(u => u.Id == userId).FirstOrDefault();
        //        loginLog = db.LogingLog.Where(u => u.FK_ApplicationUser == userId).FirstOrDefault();
        //        showComment = db.ShowComment.Where(u => u.FK_ApplicationUser == userId).FirstOrDefault();

        //        model.Add(new UserListViewModel()
        //        {
        //            FK_ApplicationUser = userData.Id,
        //            users = userData,
        //            PhoneNumber = userData.PhoneNumber,
        //            UserName = userData.UserName,
        //            Birthday = Bday,
        //            AccountAddress = userData.AccountAddress,
        //            AvatarUrl = "/SaveFiles/Admin/Avatar/",
        //            AvatarName = userData.Avatar,
        //            Email = userData.Email,
        //            RegisterTime = loginLog.RegisterTime,
        //            LastloginTime = loginLog.LastloginTime,
        //            LoginOutTime = loginLog.LoginOutTime,
        //            LogingCount = loginLog.LogingCount,
        //            Statue = loginLog.Statue,
        //            Title = showPage.Title,
        //            Comment = showComment.Comment,
        //            Star = showComment.Star
        //            IsBlackList = blist,
        //        });
        //    }
        //    return model;
        //}
    }
}