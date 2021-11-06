namespace arTWander.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initdb2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ShowPageFiles", "fileName", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ShowPageFiles", "fileName", c => c.String(nullable: false, maxLength: 20));
        }
    }
}
