namespace arTWander.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InsertDBSet : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BlackList",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Reason = c.String(nullable: false, maxLength: 255),
                        Created_At = c.DateTime(),
                        FK_ApplicationUser = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.FK_ApplicationUser)
                .Index(t => t.FK_ApplicationUser);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccountAddress = c.String(nullable: false, maxLength: 255),
                        Avatar = c.String(nullable: false, maxLength: 30),
                        Birthday = c.DateTime(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Company",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CompanyName = c.String(nullable: false, maxLength: 10),
                        CompanyDescription = c.String(maxLength: 255),
                        FK_City = c.Int(nullable: false),
                        FK_ApplicationUser = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.City", t => t.FK_City)
                .ForeignKey("dbo.AspNetUsers", t => t.FK_ApplicationUser)
                .Index(t => t.FK_City)
                .Index(t => t.FK_ApplicationUser);
            
            CreateTable(
                "dbo.City",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CityName = c.String(maxLength: 10),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.District",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DistrictName = c.String(maxLength: 10),
                        FK_City = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.City", t => t.FK_City)
                .Index(t => t.FK_City);
            
            CreateTable(
                "dbo.ShowPage",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 30),
                        Description = c.String(nullable: false, maxLength: 255),
                        ImgFileName = c.String(nullable: false, maxLength: 30),
                        ImgContent = c.String(nullable: false, maxLength: 30),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        StartTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                        Cost = c.Boolean(nullable: false),
                        Price = c.Int(),
                        AgeRange = c.Boolean(nullable: false),
                        Address = c.String(nullable: false, maxLength: 255),
                        Remark = c.String(nullable: false, maxLength: 255),
                        Created_At = c.DateTime(),
                        FK_Company = c.Int(nullable: false),
                        FK_City = c.Int(nullable: false),
                        FK_District = c.Int(nullable: false),
                        FK_Post = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Posts", t => t.FK_Post)
                .ForeignKey("dbo.District", t => t.FK_District)
                .ForeignKey("dbo.City", t => t.FK_City)
                .ForeignKey("dbo.Company", t => t.FK_Company)
                .Index(t => t.FK_Company)
                .Index(t => t.FK_City)
                .Index(t => t.FK_District)
                .Index(t => t.FK_Post);
            
            CreateTable(
                "dbo.Keywords",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 7),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PageToTodays",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Today = c.Int(nullable: false),
                        FK_ShowPage = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ShowPage", t => t.FK_ShowPage)
                .Index(t => t.FK_ShowPage);
            
            CreateTable(
                "dbo.PageViewCount",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Count = c.Int(),
                        FK_Company = c.Int(),
                        FK_ShowPage = c.Int(),
                        FK_Posts = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Posts", t => t.FK_Posts)
                .ForeignKey("dbo.ShowPage", t => t.FK_ShowPage)
                .ForeignKey("dbo.Company", t => t.FK_Company)
                .Index(t => t.FK_Company)
                .Index(t => t.FK_ShowPage)
                .Index(t => t.FK_Posts);
            
            CreateTable(
                "dbo.Posts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 255),
                        Content = c.String(nullable: false, maxLength: 255),
                        ReleaseDate = c.DateTime(),
                        FK_Company = c.Int(nullable: false),
                        FK_PostPic = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PostsPic", t => t.FK_PostPic)
                .ForeignKey("dbo.Company", t => t.FK_Company)
                .Index(t => t.FK_Company)
                .Index(t => t.FK_PostPic);
            
            CreateTable(
                "dbo.PostComment",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Comment = c.String(nullable: false, maxLength: 255),
                        FK_ApplicationUser = c.Int(nullable: false),
                        FK_Posts = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Posts", t => t.FK_Posts)
                .ForeignKey("dbo.AspNetUsers", t => t.FK_ApplicationUser)
                .Index(t => t.FK_ApplicationUser)
                .Index(t => t.FK_Posts);
            
            CreateTable(
                "dbo.ResponseComment",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Comment = c.String(nullable: false, maxLength: 255),
                        FK_ResponsePost = c.Int(nullable: false),
                        FK_ApplicationUser = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PostComment", t => t.FK_ResponsePost)
                .ForeignKey("dbo.AspNetUsers", t => t.FK_ApplicationUser)
                .Index(t => t.FK_ResponsePost)
                .Index(t => t.FK_ApplicationUser);
            
            CreateTable(
                "dbo.ResponseCommentLike",
                c => new
                    {
                        Statue = c.Boolean(nullable: false),
                        FK_ResponsePost = c.Int(nullable: false),
                        FK_ApplicationUser = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Statue, t.FK_ResponsePost, t.FK_ApplicationUser })
                .ForeignKey("dbo.PostComment", t => t.FK_ResponsePost)
                .ForeignKey("dbo.AspNetUsers", t => t.FK_ApplicationUser)
                .Index(t => t.FK_ResponsePost)
                .Index(t => t.FK_ApplicationUser);
            
            CreateTable(
                "dbo.PostLike",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Statue = c.Boolean(nullable: false),
                        FK_ApplicationUser = c.Int(nullable: false),
                        FK_Posts = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Posts", t => t.FK_Posts)
                .ForeignKey("dbo.AspNetUsers", t => t.FK_ApplicationUser)
                .Index(t => t.FK_ApplicationUser)
                .Index(t => t.FK_Posts);
            
            CreateTable(
                "dbo.PostsPic",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        fileName = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Reports",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Reason = c.String(nullable: false, maxLength: 255),
                        Created_At = c.DateTime(),
                        ResponseStatus = c.String(maxLength: 10),
                        ResponseComment = c.String(maxLength: 255),
                        Response_At = c.DateTime(),
                        FK_ApplicationUser = c.Int(),
                        FK_Company = c.Int(),
                        FK_ShowPage = c.Int(),
                        FK_ResponseShowComment = c.Int(),
                        FK_ShowComment = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ResponseShowComment", t => t.FK_ResponseShowComment)
                .ForeignKey("dbo.ShowComment", t => t.FK_ShowComment)
                .ForeignKey("dbo.ShowPage", t => t.FK_ShowPage)
                .ForeignKey("dbo.Company", t => t.FK_Company)
                .ForeignKey("dbo.AspNetUsers", t => t.FK_ApplicationUser)
                .Index(t => t.FK_ApplicationUser)
                .Index(t => t.FK_Company)
                .Index(t => t.FK_ShowPage)
                .Index(t => t.FK_ResponseShowComment)
                .Index(t => t.FK_ShowComment);
            
            CreateTable(
                "dbo.ResponseShowComment",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Comment = c.String(nullable: false, maxLength: 255),
                        ResponseDate = c.DateTime(),
                        FK_ShowComment = c.Int(nullable: false),
                        FK_Company = c.Int(nullable: false),
                        FK_ApplicationUser = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Company", t => t.FK_Company)
                .ForeignKey("dbo.AspNetUsers", t => t.FK_ApplicationUser)
                .Index(t => t.FK_Company)
                .Index(t => t.FK_ApplicationUser);
            
            CreateTable(
                "dbo.ShowComment",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Comment = c.String(nullable: false, maxLength: 255),
                        Star = c.Int(nullable: false),
                        CommentDate = c.DateTime(),
                        FK_ShowPage = c.Int(nullable: false),
                        FK_ApplicationUser = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ShowPage", t => t.FK_ShowPage)
                .ForeignKey("dbo.AspNetUsers", t => t.FK_ApplicationUser)
                .Index(t => t.FK_ShowPage)
                .Index(t => t.FK_ApplicationUser);
            
            CreateTable(
                "dbo.CompanyLink",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 10),
                        link = c.String(nullable: false, maxLength: 255),
                        FK_Company = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Company", t => t.FK_Company)
                .Index(t => t.FK_Company);
            
            CreateTable(
                "dbo.LogingLog",
                c => new
                    {
                        Statue = c.Boolean(nullable: false),
                        LastloginTime = c.DateTime(),
                        LoginOutTime = c.DateTime(),
                        LogingCount = c.Int(),
                        FK_ApplicationUser = c.Int(),
                    })
                .PrimaryKey(t => t.Statue)
                .ForeignKey("dbo.AspNetUsers", t => t.FK_ApplicationUser)
                .Index(t => t.FK_ApplicationUser);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        RoleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.sysdiagrams",
                c => new
                    {
                        diagram_id = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 128),
                        principal_id = c.Int(nullable: false),
                        version = c.Int(),
                        definition = c.Binary(),
                    })
                .PrimaryKey(t => t.diagram_id);
            
            CreateTable(
                "dbo.PageToKeyword",
                c => new
                    {
                        Keywords_Id = c.Int(nullable: false),
                        ShowPage_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Keywords_Id, t.ShowPage_Id })
                .ForeignKey("dbo.Keywords", t => t.Keywords_Id, cascadeDelete: true)
                .ForeignKey("dbo.ShowPage", t => t.ShowPage_Id, cascadeDelete: true)
                .Index(t => t.Keywords_Id)
                .Index(t => t.ShowPage_Id);
            
            CreateTable(
                "dbo.Subscription",
                c => new
                    {
                        AccountUserSub_Id = c.Int(nullable: false),
                        CompanySub_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.AccountUserSub_Id, t.CompanySub_Id })
                .ForeignKey("dbo.AspNetUsers", t => t.AccountUserSub_Id, cascadeDelete: true)
                .ForeignKey("dbo.Company", t => t.CompanySub_Id, cascadeDelete: true)
                .Index(t => t.AccountUserSub_Id)
                .Index(t => t.CompanySub_Id);
            
            CreateTable(
                "dbo.MyShow",
                c => new
                    {
                        ApplicationUser_Id = c.Int(nullable: false),
                        ShowPage_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationUser_Id, t.ShowPage_Id })
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id, cascadeDelete: true)
                .ForeignKey("dbo.ShowPage", t => t.ShowPage_Id, cascadeDelete: true)
                .Index(t => t.ApplicationUser_Id)
                .Index(t => t.ShowPage_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.MyShow", "ShowPage_Id", "dbo.ShowPage");
            DropForeignKey("dbo.MyShow", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.ShowComment", "FK_ApplicationUser", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ResponseShowComment", "FK_ApplicationUser", "dbo.AspNetUsers");
            DropForeignKey("dbo.ResponseCommentLike", "FK_ApplicationUser", "dbo.AspNetUsers");
            DropForeignKey("dbo.ResponseComment", "FK_ApplicationUser", "dbo.AspNetUsers");
            DropForeignKey("dbo.Reports", "FK_ApplicationUser", "dbo.AspNetUsers");
            DropForeignKey("dbo.PostLike", "FK_ApplicationUser", "dbo.AspNetUsers");
            DropForeignKey("dbo.PostComment", "FK_ApplicationUser", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.LogingLog", "FK_ApplicationUser", "dbo.AspNetUsers");
            DropForeignKey("dbo.Subscription", "CompanySub_Id", "dbo.Company");
            DropForeignKey("dbo.Subscription", "AccountUserSub_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Company", "FK_ApplicationUser", "dbo.AspNetUsers");
            DropForeignKey("dbo.ShowPage", "FK_Company", "dbo.Company");
            DropForeignKey("dbo.ResponseShowComment", "FK_Company", "dbo.Company");
            DropForeignKey("dbo.Reports", "FK_Company", "dbo.Company");
            DropForeignKey("dbo.Posts", "FK_Company", "dbo.Company");
            DropForeignKey("dbo.PageViewCount", "FK_Company", "dbo.Company");
            DropForeignKey("dbo.CompanyLink", "FK_Company", "dbo.Company");
            DropForeignKey("dbo.ShowPage", "FK_City", "dbo.City");
            DropForeignKey("dbo.District", "FK_City", "dbo.City");
            DropForeignKey("dbo.ShowPage", "FK_District", "dbo.District");
            DropForeignKey("dbo.ShowComment", "FK_ShowPage", "dbo.ShowPage");
            DropForeignKey("dbo.Reports", "FK_ShowPage", "dbo.ShowPage");
            DropForeignKey("dbo.Reports", "FK_ShowComment", "dbo.ShowComment");
            DropForeignKey("dbo.Reports", "FK_ResponseShowComment", "dbo.ResponseShowComment");
            DropForeignKey("dbo.PageViewCount", "FK_ShowPage", "dbo.ShowPage");
            DropForeignKey("dbo.ShowPage", "FK_Post", "dbo.Posts");
            DropForeignKey("dbo.Posts", "FK_PostPic", "dbo.PostsPic");
            DropForeignKey("dbo.PostLike", "FK_Posts", "dbo.Posts");
            DropForeignKey("dbo.PostComment", "FK_Posts", "dbo.Posts");
            DropForeignKey("dbo.ResponseCommentLike", "FK_ResponsePost", "dbo.PostComment");
            DropForeignKey("dbo.ResponseComment", "FK_ResponsePost", "dbo.PostComment");
            DropForeignKey("dbo.PageViewCount", "FK_Posts", "dbo.Posts");
            DropForeignKey("dbo.PageToTodays", "FK_ShowPage", "dbo.ShowPage");
            DropForeignKey("dbo.PageToKeyword", "ShowPage_Id", "dbo.ShowPage");
            DropForeignKey("dbo.PageToKeyword", "Keywords_Id", "dbo.Keywords");
            DropForeignKey("dbo.Company", "FK_City", "dbo.City");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.BlackList", "FK_ApplicationUser", "dbo.AspNetUsers");
            DropIndex("dbo.MyShow", new[] { "ShowPage_Id" });
            DropIndex("dbo.MyShow", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Subscription", new[] { "CompanySub_Id" });
            DropIndex("dbo.Subscription", new[] { "AccountUserSub_Id" });
            DropIndex("dbo.PageToKeyword", new[] { "ShowPage_Id" });
            DropIndex("dbo.PageToKeyword", new[] { "Keywords_Id" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.LogingLog", new[] { "FK_ApplicationUser" });
            DropIndex("dbo.CompanyLink", new[] { "FK_Company" });
            DropIndex("dbo.ShowComment", new[] { "FK_ApplicationUser" });
            DropIndex("dbo.ShowComment", new[] { "FK_ShowPage" });
            DropIndex("dbo.ResponseShowComment", new[] { "FK_ApplicationUser" });
            DropIndex("dbo.ResponseShowComment", new[] { "FK_Company" });
            DropIndex("dbo.Reports", new[] { "FK_ShowComment" });
            DropIndex("dbo.Reports", new[] { "FK_ResponseShowComment" });
            DropIndex("dbo.Reports", new[] { "FK_ShowPage" });
            DropIndex("dbo.Reports", new[] { "FK_Company" });
            DropIndex("dbo.Reports", new[] { "FK_ApplicationUser" });
            DropIndex("dbo.PostLike", new[] { "FK_Posts" });
            DropIndex("dbo.PostLike", new[] { "FK_ApplicationUser" });
            DropIndex("dbo.ResponseCommentLike", new[] { "FK_ApplicationUser" });
            DropIndex("dbo.ResponseCommentLike", new[] { "FK_ResponsePost" });
            DropIndex("dbo.ResponseComment", new[] { "FK_ApplicationUser" });
            DropIndex("dbo.ResponseComment", new[] { "FK_ResponsePost" });
            DropIndex("dbo.PostComment", new[] { "FK_Posts" });
            DropIndex("dbo.PostComment", new[] { "FK_ApplicationUser" });
            DropIndex("dbo.Posts", new[] { "FK_PostPic" });
            DropIndex("dbo.Posts", new[] { "FK_Company" });
            DropIndex("dbo.PageViewCount", new[] { "FK_Posts" });
            DropIndex("dbo.PageViewCount", new[] { "FK_ShowPage" });
            DropIndex("dbo.PageViewCount", new[] { "FK_Company" });
            DropIndex("dbo.PageToTodays", new[] { "FK_ShowPage" });
            DropIndex("dbo.ShowPage", new[] { "FK_Post" });
            DropIndex("dbo.ShowPage", new[] { "FK_District" });
            DropIndex("dbo.ShowPage", new[] { "FK_City" });
            DropIndex("dbo.ShowPage", new[] { "FK_Company" });
            DropIndex("dbo.District", new[] { "FK_City" });
            DropIndex("dbo.Company", new[] { "FK_ApplicationUser" });
            DropIndex("dbo.Company", new[] { "FK_City" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.BlackList", new[] { "FK_ApplicationUser" });
            DropTable("dbo.MyShow");
            DropTable("dbo.Subscription");
            DropTable("dbo.PageToKeyword");
            DropTable("dbo.sysdiagrams");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.LogingLog");
            DropTable("dbo.CompanyLink");
            DropTable("dbo.ShowComment");
            DropTable("dbo.ResponseShowComment");
            DropTable("dbo.Reports");
            DropTable("dbo.PostsPic");
            DropTable("dbo.PostLike");
            DropTable("dbo.ResponseCommentLike");
            DropTable("dbo.ResponseComment");
            DropTable("dbo.PostComment");
            DropTable("dbo.Posts");
            DropTable("dbo.PageViewCount");
            DropTable("dbo.PageToTodays");
            DropTable("dbo.Keywords");
            DropTable("dbo.ShowPage");
            DropTable("dbo.District");
            DropTable("dbo.City");
            DropTable("dbo.Company");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.BlackList");
        }
    }
}
