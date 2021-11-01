namespace arTWander.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initdb1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.BlackList", "FK_ApplicationUser", "dbo.AspNetUsers");
            DropForeignKey("dbo.ResponseShowComment", "FK_ApplicationUser", "dbo.AspNetUsers");
            DropForeignKey("dbo.ShowComment", "FK_ApplicationUser", "dbo.AspNetUsers");
            DropForeignKey("dbo.ResponseShowComment", "FK_Company", "dbo.Company");
            DropForeignKey("dbo.ShowPage", "FK_Company", "dbo.Company");
            DropForeignKey("dbo.PageToTodays", "FK_ShowPage", "dbo.ShowPage");
            DropForeignKey("dbo.ShowComment", "FK_ShowPage", "dbo.ShowPage");
            DropForeignKey("dbo.ShowPageFiles", "FK_ShowPage", "dbo.ShowPage");
            AddForeignKey("dbo.BlackList", "FK_ApplicationUser", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ResponseShowComment", "FK_ApplicationUser", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ShowComment", "FK_ApplicationUser", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ResponseShowComment", "FK_Company", "dbo.Company", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ShowPage", "FK_Company", "dbo.Company", "Id", cascadeDelete: true);
            AddForeignKey("dbo.PageToTodays", "FK_ShowPage", "dbo.ShowPage", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ShowComment", "FK_ShowPage", "dbo.ShowPage", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ShowPageFiles", "FK_ShowPage", "dbo.ShowPage", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ShowPageFiles", "FK_ShowPage", "dbo.ShowPage");
            DropForeignKey("dbo.ShowComment", "FK_ShowPage", "dbo.ShowPage");
            DropForeignKey("dbo.PageToTodays", "FK_ShowPage", "dbo.ShowPage");
            DropForeignKey("dbo.ShowPage", "FK_Company", "dbo.Company");
            DropForeignKey("dbo.ResponseShowComment", "FK_Company", "dbo.Company");
            DropForeignKey("dbo.ShowComment", "FK_ApplicationUser", "dbo.AspNetUsers");
            DropForeignKey("dbo.ResponseShowComment", "FK_ApplicationUser", "dbo.AspNetUsers");
            DropForeignKey("dbo.BlackList", "FK_ApplicationUser", "dbo.AspNetUsers");
            AddForeignKey("dbo.ShowPageFiles", "FK_ShowPage", "dbo.ShowPage", "Id");
            AddForeignKey("dbo.ShowComment", "FK_ShowPage", "dbo.ShowPage", "Id");
            AddForeignKey("dbo.PageToTodays", "FK_ShowPage", "dbo.ShowPage", "Id");
            AddForeignKey("dbo.ShowPage", "FK_Company", "dbo.Company", "Id");
            AddForeignKey("dbo.ResponseShowComment", "FK_Company", "dbo.Company", "Id");
            AddForeignKey("dbo.ShowComment", "FK_ApplicationUser", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.ResponseShowComment", "FK_ApplicationUser", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.BlackList", "FK_ApplicationUser", "dbo.AspNetUsers", "Id");
        }
    }
}
