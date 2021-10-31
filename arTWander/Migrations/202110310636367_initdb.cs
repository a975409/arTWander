namespace arTWander.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initdb : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PageViewCount", "FK_Company", "dbo.Company");
            DropIndex("dbo.PageViewCount", new[] { "FK_Company" });
            AddColumn("dbo.PageViewCount", "FK_ApplicationUser", c => c.Int());
            CreateIndex("dbo.PageViewCount", "FK_ApplicationUser");
            AddForeignKey("dbo.PageViewCount", "FK_ApplicationUser", "dbo.AspNetUsers", "Id");
            DropColumn("dbo.PageViewCount", "FK_Company");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PageViewCount", "FK_Company", c => c.Int());
            DropForeignKey("dbo.PageViewCount", "FK_ApplicationUser", "dbo.AspNetUsers");
            DropIndex("dbo.PageViewCount", new[] { "FK_ApplicationUser" });
            DropColumn("dbo.PageViewCount", "FK_ApplicationUser");
            CreateIndex("dbo.PageViewCount", "FK_Company");
            AddForeignKey("dbo.PageViewCount", "FK_Company", "dbo.Company", "Id");
        }
    }
}
