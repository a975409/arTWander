using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace arTWander.Models
{
    public class CompanyFactory
    {
        private ApplicationDbContext _dbContext;

        public CompanyFactory(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Company GetCompany(int userId)
        {
            return _dbContext.Company.Where(m => m.FK_ApplicationUser == userId).FirstOrDefault();
        }

        public async Task<string> GetFullAddress(int cityId,int districtId,string address)
        {
            string CityName = City_District.getCities(_dbContext).Where(m => m.Id == cityId).Select(m => m.CityName).FirstOrDefault();

            string DistrictName = "";
            var districts = await City_District.getDistricts(cityId, _dbContext);

            if (districts != null)
                DistrictName = districts.Where(m => m.Id == districtId).Select(m => m.DistrictName).FirstOrDefault();

            return $"{CityName}{DistrictName}{address}";
        }

        public bool SaveCompanyPageImage(HttpPostedFileBase fileBase,string saveDir, string savefileName)
        {
            if (fileBase != null && fileBase.ContentLength > 0)
            {
                byte[] ImageData = new byte[fileBase.ContentLength];
                fileBase.InputStream.Read(ImageData, 0, fileBase.ContentLength);

                using (MemoryStream stream = new MemoryStream(ImageData))
                {
                    //判斷上傳的檔案是否為圖片檔
                    if (IsImage(stream))
                    {
                        //移除原先儲存在Server上的封面圖
                        var PromotionalImages = Directory.GetFiles(saveDir).Where(m => Path.GetFileNameWithoutExtension(m) == Path.GetFileNameWithoutExtension(savefileName));

                        foreach (var item in PromotionalImages)
                        {
                            FileInfo info = new FileInfo(item);

                            if (info.Exists)
                            {
                                try
                                {
                                    info.Delete();
                                }
                                catch { }
                            }
                        }

                        //取得副檔名
                        string extension = Path.GetExtension(fileBase.FileName);

                        //完整另存路徑
                        string savePath = Path.Combine(saveDir, savefileName);

                        //server端下載檔案
                        fileBase.SaveAs(savePath);
                        return true;
                    }
                }
            }
            return false;
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