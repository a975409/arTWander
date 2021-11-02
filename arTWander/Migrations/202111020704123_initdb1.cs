namespace arTWander.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initdb1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Company", "CompanyDescription", c => c.String(nullable: false));
            AlterColumn("dbo.ShowPage", "Description", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ShowPage", "Description", c => c.String(nullable: false, maxLength: 1000));
            AlterColumn("dbo.Company", "CompanyDescription", c => c.String(nullable: false, maxLength: 1000));
        }
    }
}
