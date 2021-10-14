namespace arTWander.Migrations
{
    using arTWander.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Web;
    using Newtonsoft.Json;
    using System.IO;

    
    internal sealed class Configuration : DbMigrationsConfiguration<arTWander.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "arTWander.Models.ApplicationDbContext";
        }

        protected override void Seed(arTWander.Models.ApplicationDbContext context)
        {
            //Configuration.cs不會隨著網站一起執行，主要功用是將資料更新至資料庫而已

            //如果在這裡新增預設資料，只要在套件管理器主控台輸入update-database，就可將資料寫入至資料庫
            //但如果Model有做異動，例如欄位名稱、變更關聯等等...請查看IdentityModels.cs的說明操作

            //  This method will be called after migrating to the latest version.
            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.

            //將縣市鄉鎮資料新增至資料庫
            CreateCityCountyData(context);
        }

        public void CreateCityCountyData(ApplicationDbContext context)
        {
            //將CityCountyData.json裡面的縣市資料寫入至資料庫
            int cityIdx = 1;
            int areaIdx = 1;

            //請自行指定正確路徑，檔案已附在專案內
            string path = @"D:\MyWork\arTWander\arTWander\CityCountyData.json";

            //重設欄位的自動增量(auto increment)的識別值，下一筆資料編號由1開始
            context.Database.ExecuteSqlCommand("dbcc checkIDENT(City, RESEED, 1)");
            context.Database.ExecuteSqlCommand("dbcc checkIDENT(District, RESEED, 1)");

            StreamReader r = new StreamReader(path);
            string jsonString = r.ReadToEnd();
            CityCountyData[] cityCountyDatas = JsonConvert.DeserializeObject<CityCountyData[]>(jsonString);

            foreach (var city in cityCountyDatas)
            {
                context.City.AddOrUpdate(x => x.Id, new City { Id = cityIdx, CityName = city.CityName });

                foreach (var area in city.AreaList)
                {
                    context.District.AddOrUpdate(x => x.Id, new District { Id = areaIdx, DistrictName = area.AreaName, FK_City = cityIdx });
                    areaIdx++;
                }
                cityIdx++;
            }
        }
    }

    public class CityCountyData
    {
        public string CityName { get; set; }
        public string CityEngName { get; set; }
        public AreaList[] AreaList { get; set; }
    }

    public class AreaList
    {
        public int ZipCode { get; set; }
        public string AreaName { get; set; }
        public string AreaEngName { get; set; }
    }
}
