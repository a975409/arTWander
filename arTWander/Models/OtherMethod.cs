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
        public static IPagedList<T> getCurrentPagedList<T>(IEnumerable<T> datalist, int page = 1, int pageSize = 3) where T : class
        {
            int currentPage = page < 1 ? 1 : page;

            var showPages = datalist.ToPagedList(currentPage, pageSize);

            return showPages;
        }

        public static IEnumerable<ShowPage> searchShowPage(IEnumerable<ShowPage> showPages, SearchShowPagesViewModel model)
        {
            if (model == null)
            {
                return showPages.OrderByDescending(m => m.Created_At);
            }
            else
            {

                if (model.FK_City > 0)
                {
                    showPages = showPages.Where(m => m.FK_City == model.FK_City);

                    if (model.FK_District > 0)
                        showPages = showPages.Where(m => m.FK_District == model.FK_District);
                }

                //找出在開始日期與結束日期範圍內的展演
                if (model.StartDate != null && model.EndDate != null)
                {
                    if (!model.StartDate.Equals(DateTime.MinValue) && !model.EndDate.Equals(DateTime.MinValue))
                    {
                        showPages = showPages.Where(m => DateTime.Compare(model.StartDate.Date, m.StartDate.Date) <= 0 && DateTime.Compare(m.EndDate.Date, model.EndDate.Date) <= 0);
                    }
                    else if (!model.StartDate.Equals(DateTime.MinValue))
                    {
                        showPages = showPages.Where(m => DateTime.Compare(model.StartDate.Date, m.StartDate.Date) <= 0);
                    }
                    else if (!model.EndDate.Equals(DateTime.MinValue))
                    {
                        showPages = showPages.Where(m => DateTime.Compare(m.EndDate.Date, model.EndDate.Date) <= 0);
                    }
                    else { }
                }

                if (model.Cost != CostStatus.none)
                {

                    if (model.Cost == CostStatus.yes)
                        showPages = showPages.Where(m => m.Cost);
                    else
                        showPages = showPages.Where(m => !m.Cost);
                }

                switch (model.OrderSortField)
                {
                    //熱門展演，以好評數＆留言數做判斷
                    case OrderSortField.HotSort:
                        //先取得平均好評數做排序，再取得留言數做排序
                        showPages = showPages.Where(m => DateTime.Compare(DateTime.Now, m.StartDate) >= 0 && DateTime.Compare(DateTime.Now, m.EndDate) <= 0).OrderByDescending(m =>
                        {

                            if (m.ShowComments.Count() > 0)
                                return m.ShowComments.Sum(s => s.Star) / m.ShowComments.Count();
                            else
                                return 0;
                        }).ThenByDescending(m => m.ShowComments.Count());
                        break;
                    case OrderSortField.DateSort:
                        //最新展演
                        showPages = showPages.Where(m => DateTime.Compare(DateTime.Now, m.StartDate) >= 0 && DateTime.Compare(DateTime.Now, m.EndDate) <= 0).OrderByDescending(m => m.Created_At);
                        break;
                    case OrderSortField.AllData:
                        showPages = showPages.OrderByDescending(m => m.Created_At);
                        break;

                    default:
                        break;
                }
            }

            return showPages;
        }


        public static IEnumerable<Company> searchCustomerPage(IEnumerable<Company> company, int? cityId)
        {
            if (cityId == null)
            {
                return company.OrderBy(m => m.FK_City);
            }
            else
            {
                if (cityId != null)
                {
                    company = company.Where(m => m.FK_City == cityId).OrderBy(m => m.FK_City);
                }
            }
            return company;
        }
        public static IEnumerable<Company> searchMyCustomerPage(IEnumerable<Company> company, ICollection<Company> model)
        {
            if (model != null)
            {
                    company = model.OrderBy(m => m.Id);
            }
            return company;
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
    public class SearchCustomerPagesViewModel
    {
        public int FK_City { get; set; }

        public OrderSortField OrderSortField { get; set; }
    }

    public enum CostStatus
    {
        none = 0, yes = 1, no = 2
    }

    public enum OrderSortField
    {
        AllData = 0, DateSort = 1, HotSort = 2
    }
}