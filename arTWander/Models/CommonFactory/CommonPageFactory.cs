using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace arTWander.Models.CommonFactory
{
    public class CommonPageFactory
    {
        private ApplicationDbContext _dbContext;

        public CommonPageFactory(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        private const int pageSize = 12;

        public IQueryable<CommonCompanyViewModel> queryAllCustomer()
        {
            var companyList = _dbContext.Company.Select(m => new CommonCompanyViewModel
            {
                Id = m.Id,
                CompanyName = m.CompanyName,
                CompanyCity = m.City.CityName,
                PhotoSticker = "/SaveFiles/Company/" + m.Id + "/Info/" + m.PhotoStickerImage,
            });
            return companyList;
        }
        public IQueryable<CommonCompanyViewModel> queryCustomerByCity(int? CityId)
        {
            var companyList = _dbContext.Company.Where(m=>m.FK_City == CityId).Select(m => new CommonCompanyViewModel
            {
                Id = m.Id,
                CompanyName = m.CompanyName,
                CompanyCity = m.City.CityName,
                PhotoSticker = "/SaveFiles/Company/" + m.Id + "/Info/" + m.PhotoStickerImage,
            });
            return companyList;
        }

        public IPagedList<CommonCompanyViewModel> getGalleryPages(int page = 1, int? cityId=null)
        {
            //依據搜尋條件取得該展演單位的展演
            var shows = OtherMethod.searchCustomerPage(_dbContext.Company, cityId).Select(m => new CommonCompanyViewModel
            {
                Id = m.Id,
                CompanyName = m.CompanyName,
                CompanyCity = m.City.CityName,
                PhotoSticker = "/SaveFiles/Company/" + m.Id + "/Info/" + m.PhotoStickerImage,
            });

            var showPages = OtherMethod.getCurrentPagedList(shows, page, pageSize);

            return showPages;
        }

        public IPagedList<Company> getMyGalleryPages(int page = 1, ICollection<Company> model = null)
        {
            //依據搜尋條件取得該展演單位的展演
            var shows = OtherMethod.searchMyCustomerPage(_dbContext.Company, model).Select(m => m);
            var showPages = OtherMethod.getCurrentPagedList(shows, page, pageSize);
            return showPages;
        }

    }
}