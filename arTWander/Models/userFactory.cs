using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace arTWander.Models
{
    public class userFactory
    {
        public ApplicationUser getUserById(int id)
        {
            // 查詢user
            ApplicationDbContext db = new ApplicationDbContext();
            var q = from nowUser in db.Users
                    where nowUser.Id == id
                    select nowUser;
            var person = q.ToList()[0];
            return person;
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
                    string fileName = user.Avatar;
                    string savePath = Path.Combine(HttpContext.Current.Server.MapPath("~/image/avatar"), fileName);
                    avatarFile.SaveAs(savePath);

                }

            }
        }
    }
}