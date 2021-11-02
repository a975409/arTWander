using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace arTWander.Models
{
    public class OtherMethod
    {
        public static bool IsImage(Stream stream)
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

        /// <summary>
        /// 檢查檔名是否為合法檔名，不包含特殊符號
        /// 參考資料：https://www.itread01.com/content/1548633799.html
        /// </summary>
        /// <param name="fileName">僅限檔名+副檔名，不包含路徑</param>
        /// <returns>回傳結果</returns>
        public static bool checkFileName(string fileName)
        {
            Regex regex = new Regex(@"^[^\/\:\*\?\""\<\>\|\,]+$");

            Match m = regex.Match(fileName);

            return m.Success;
        }
    }
}