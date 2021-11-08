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


    }
}