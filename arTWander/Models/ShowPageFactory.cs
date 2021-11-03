
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace arTWander.Models
{
    public class ShowPageFactory
    {
        private ApplicationDbContext _dbContext;

        private const int pageSize = 3;

        public ShowPageFactory(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IPagedList<ShowMinViewModel> getCompanyShowPages(Company company,int page=1)
        {
            if (company.ShowPages == null || company.ShowPages.Count <= 0)
            {
                return null;
            }

            int currentPage = page < 1 ? 1 : page;

            var shows = company.ShowPages.OrderByDescending(m => m.Created_At).Select(m => new ShowMinViewModel
            {
                Description = m.Description,
                cityName = m.City.CityName,
                Id = m.Id,
                Title = m.Title,
                Comment = m.ShowComments.Count(),
                fileName = m.ShowPageFiles.Count() <= 0 ? "/image/exhibiton/Null.png" : $"/SaveFiles/Company/{m.Company.Id}/show/{m.Id}/{m.ShowPageFiles.FirstOrDefault().fileName}"
            });

            var showPages = shows.ToPagedList(currentPage, pageSize);

            return showPages;
        }

        /// <summary>
        /// 新增該展演對應的關鍵字
        /// </summary>
        /// <param name="searchKeyword">關鍵字，以","隔開</param>
        /// <param name="showPage">該展演的物件</param>
        /// <returns></returns>
        public async Task insertKeywords(string searchKeyword, ShowPage showPage)
        {
            if (!string.IsNullOrEmpty(searchKeyword))
            {
                foreach (string item in searchKeyword.Split(','))
                {
                    if (_dbContext.Keywords.Where(m => m.Name == item).Any())
                    {
                        var keyword = _dbContext.Keywords.Where(m => m.Name == item).FirstOrDefault();
                        showPage.KeywordsList.Add(keyword);
                    }
                    else
                    {
                        showPage.KeywordsList.Add(new Keywords { Name = item });
                    }
                }
                await _dbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// 新增該展演對應的開放時段
        /// </summary>
        /// <param name="todays">指定開放時段</param>
        /// <param name="showPage">該展演的物件</param>
        /// <returns></returns>
        public async Task setOpenTodays(int[] todays, ShowPage showPage)
        {
            foreach (int item in todays)
            {
                _dbContext.PageToTodays.Add(new PageToTodays { FK_ShowPage = showPage.Id, Today = item });
            }
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 上傳主視覺圖檔
        /// </summary>
        /// <param name="imgFiles">指定上傳的檔案</param>
        /// <param name="saveDir">存檔資料夾路徑</param>
        /// <param name="showPage">該展演的物件</param>
        /// <returns></returns>
        public async Task SaveShowPageFiles(HttpPostedFileBase[] imgFiles,string saveDir, ShowPage showPage)
        {
            if (imgFiles != null && imgFiles.Length > 0)
            {
                foreach (var item in imgFiles)
                {
                    if (item != null && item.ContentLength > 0 && item.FileName.Length <= 20 && OtherMethod.checkFileName(item.FileName))
                    {
                        byte[] ImageData = new byte[item.ContentLength];
                        item.InputStream.Read(ImageData, 0, item.ContentLength);

                        using (MemoryStream stream = new MemoryStream(ImageData))
                        {
                            //判斷上傳的檔案是否為圖片檔
                            if (OtherMethod.IsImage(stream))
                            {
                                //完整另存路徑
                                string savePath = Path.Combine(saveDir, Path.GetFileName(item.FileName));

                                //server端下載檔案
                                item.SaveAs(savePath);

                                _dbContext.ShowPageFile.Add(new ShowPageFile { fileName = Path.GetFileName(item.FileName), FK_ShowPage = showPage.Id });
                            }
                        }
                    }
                }
                await _dbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// 新增或移除主視覺圖檔
        /// </summary>
        /// <param name="imgFiles">指定上傳的檔案</param>
        /// <param name="saveDir">存檔資料夾路徑</param>
        /// <param name="showPage">該展演的物件</param>
        /// <returns></returns>
        public async Task AddOrUpdateShowPageFiles(HttpPostedFileBase[] imgFiles, string saveDir, ShowPage showPage)
        {
            if (imgFiles != null && imgFiles.Length > 0)
            {
                //儲存新增的圖檔
                foreach (var item in imgFiles)
                {
                    if (item != null && item.ContentLength > 0 && item.FileName.Length <= 20 && OtherMethod.checkFileName(item.FileName))
                    {
                        byte[] ImageData = new byte[item.ContentLength];
                        item.InputStream.Read(ImageData, 0, item.ContentLength);

                        using (MemoryStream stream = new MemoryStream(ImageData))
                        {
                            //判斷上傳的檔案是否為圖片檔
                            if (OtherMethod.IsImage(stream))
                            {
                                //查看該檔名如果已存在於資料庫，就不在資料庫新增資料
                                if (!showPage.ShowPageFiles.Any(m => m.fileName == Path.GetFileName(item.FileName)))
                                {
                                    _dbContext.ShowPageFile.Add(new ShowPageFile { fileName = Path.GetFileName(item.FileName), FK_ShowPage = showPage.Id });
                                }

                                //完整另存路徑
                                string savePath = Path.Combine(saveDir, Path.GetFileName(item.FileName));

                                //server端下載檔案，檔名重複則直接覆蓋
                                item.SaveAs(savePath);
                            }
                        }
                    }
                }
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}