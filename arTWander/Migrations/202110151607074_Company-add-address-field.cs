namespace arTWander.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Companyaddaddressfield : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Company", "Address", c => c.String(nullable: false, maxLength: 255));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Company", "Address");
        }
    }
}
