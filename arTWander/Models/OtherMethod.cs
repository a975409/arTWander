using PagedList;
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

        /// <summary>
        /// 依據指定頁數，以及指定單一頁面的數量，列出範圍內的資料
        /// </summary>
        /// <typeparam name="T">限定只有參考型態（class）可使用</typeparam>
        /// <param name="datalist">list資料</param>
        /// <param name="page">指定頁數</param>
        /// <param name="pageSize">單頁呈現多少資料筆數</param>
        /// <returns></returns>
        public static IPagedList<T> getCurrentPagedList<T>(IEnumerable<T> datalist, int page = 1 ,int pageSize=3) where T : class
        {
            int currentPage = page < 1 ? 1 : page;

            var showPages = datalist.ToPagedList(currentPage, pageSize);

            return showPages;
        }
    }

    public class SearchShowPagesViewModel
    {
        public int FK_City { get; set; }

        public int FK_District { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public CostStatus Cost { get; set; }

        public OrderSortField OrderSortField { get; set; }

    }

    public enum CostStatus
    {
        none = 0, yes = 1, no = 2
    }

    public enum OrderSortField
    {
        PrimaryKey = 0, DateSort = 1, HotSort = 2
    }
}