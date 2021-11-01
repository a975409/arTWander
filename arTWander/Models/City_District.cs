using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace arTWander.Models
{
    public class City_District
    {
        public static ViewCity[] getCities(ApplicationDbContext dbContext)
        {
            return dbContext.City.Select(m => new ViewCity { Id = m.Id, CityName = m.CityName }).ToArray();
        }

        public async static Task<ViewDistrict[]> getDistricts(int cityId, ApplicationDbContext dbContext)
        {
            var city = await dbContext.City.FindAsync(cityId);

            if (city != null)
                return city.Districts.Select(m => new ViewDistrict { Id = m.Id, DistrictName = m.DistrictName, FK_City = m.FK_City }).ToArray();

            return null;
        }
    }

    public class ViewCity
    {
        public int Id { get; set; }

        public string CityName { get; set; }
    }

    public class ViewDistrict
    {
        public int Id { get; set; }

        public string DistrictName { get; set; }

        public int FK_City { get; set; }
    }
}