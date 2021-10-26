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
    }
}