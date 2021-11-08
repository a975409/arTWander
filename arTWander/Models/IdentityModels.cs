using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace arTWander.Models
{
    // 您可將更多屬性新增至 ApplicationUser 類別，藉此為使用者新增設定檔資料，如需深入了解，請瀏覽 https://go.microsoft.com/fwlink/?LinkID=317594。

    //如果有對Model做任何異動，或者新增Model，要將其異動更新至資料庫，就要執行下列流程
    //在套件管理主控台輸入下列指令
    //如果是該專案初次執行（沒有Migrations資料夾）：

    //1.Enable-Migrations –EnableAutomaticMigrations，
    //執行成功會出現Migrations資料夾，以及底下會出現Configuration.cs

    //之後只要資料庫有異動，只要執行步驟2～3即可

    //2.Add-Migration xxx，xxx代表Name，隨意輸入即可，作用很像Git的Commit

    //3.update-database，更新資料庫，作用很像Git的push

    //=========================Model資料表===============================

    //如果要在下列預設的資料表新增欄位和建立關聯，直接在類別（Model）內新增就好
    public class ApplicationUserLogin : IdentityUserLogin<int> { }
    public class ApplicationUserClaim : IdentityUserClaim<int> { }
    public class ApplicationUserRole : IdentityUserRole<int> { }

    public class ApplicationRole : IdentityRole<int, ApplicationUserRole>, IRole<int>
    {
        public string Description { get; set; }

        public ApplicationRole() : base() { }
        public ApplicationRole(string name)
            : this()
        {
            this.Name = name;
        }

        public ApplicationRole(string name, string description)
            : this(name)
        {
            this.Description = description;
        }
    }

    public class ApplicationUser : IdentityUser<int, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>, IUser<int>
    {
        public async Task<ClaimsIdentity>
            GenerateUserIdentityAsync(UserManager<ApplicationUser, int> manager)
        {
            // 注意 authenticationType 必須符合 CookieAuthenticationOptions.AuthenticationType 中定義的項目
            var userIdentity = await manager
                .CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // 在這裡新增自訂使用者宣告
            return userIdentity;
        }

        public ApplicationUser()
        {
            PageViewCounts = new HashSet<PageViewCount>();
        }

        //建立與Posts一對多的關聯，Posts是額外建立的Model
        //public ICollection<Posts> Posts { get; set; }

        //[Required]
        //[StringLength(255)]
        public string AccountAddress { get; set; }

        //[Required]
        //[StringLength(30)]
        public string Avatar { get; set; }

        public DateTime? Birthday { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BlackList> BlackLists { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Company> Companies { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LogingLog> LogingLogs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Reports> ReportsList { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ResponseShowComment> ResponseShowComments { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ShowComment> ShowComments { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ShowPage> ShowPage { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Company> CompanySubs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PageViewCount> PageViewCounts { get; set; }
    }

    //=========================Model資料表===============================

    /// <summary>
    /// DbContext指定資料庫連線字串，以及建立DbSet的地方
    /// </summary>
    public class ApplicationDbContext
        : IdentityDbContext<ApplicationUser, ApplicationRole, int,
        ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>
    {
        public ApplicationDbContext()
            : base(Environment.GetEnvironmentVariable("AzureConnstring"))
        {
            string a = Environment.GetEnvironmentVariable("AzureConnstring");
        Console.WriteLine(a);
        }

    static ApplicationDbContext()
        {
            Database.SetInitializer<ApplicationDbContext>(new ApplicationDbInitializer());
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        //如果有新增其他Model更新至資料庫，那就在底下新增DbSet即可
        //public virtual DbSet<Posts> Posts { get; set; }
        //public virtual DbSet<ApplicationUser> ApplicationUser { get; set; }
        public virtual DbSet<BlackList> BlackList { get; set; }
        public virtual DbSet<City> City { get; set; }
        public virtual DbSet<Company> Company { get; set; }
        public virtual DbSet<District> District { get; set; }
        public virtual DbSet<Keywords> Keywords { get; set; }
        public virtual DbSet<LogingLog> LogingLog { get; set; }
        public virtual DbSet<PageToTodays> PageToTodays { get; set; }
        public virtual DbSet<PageViewCount> PageViewCount { get; set; }
        public virtual DbSet<Reports> Reports { get; set; }
        public virtual DbSet<ResponseShowComment> ResponseShowComment { get; set; }
        public virtual DbSet<ShowComment> ShowComment { get; set; }
        public virtual DbSet<ShowPage> ShowPage { get; set; }
        public virtual DbSet<ShowPageFile> ShowPageFile { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }

        //設定資料表關聯
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(e => e.BlackLists)
                .WithRequired(e => e.ApplicationUser)
                .HasForeignKey(e => e.FK_ApplicationUser)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(e => e.Companies)
                .WithRequired(e => e.ApplicationUser)
                .HasForeignKey(e => e.FK_ApplicationUser)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(e => e.LogingLogs)
                .WithOptional(e => e.ApplicationUser)
                .HasForeignKey(e => e.FK_ApplicationUser);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(e => e.ReportsList)
                .WithOptional(e => e.ApplicationUser)
                .HasForeignKey(e => e.FK_ApplicationUser);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(e => e.ResponseShowComments)
                .WithRequired(e => e.ApplicationUser)
                .HasForeignKey(e => e.FK_ApplicationUser)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(e => e.ShowComments)
                .WithRequired(e => e.ApplicationUser)
                .HasForeignKey(e => e.FK_ApplicationUser)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(e => e.ShowPage)
                .WithMany(e => e.ApplicationUsers)
                .Map(m => m.ToTable("MyShow"));

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(e => e.CompanySubs)
                .WithMany(e => e.ApplicationUserSubs)
                .Map(m => m.ToTable("Subscription").MapLeftKey("AccountUserSub_Id").MapRightKey("CompanySub_Id"));

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(e => e.PageViewCounts)
                .WithOptional(e => e.ApplicationUser)
                .HasForeignKey(e => e.FK_ApplicationUser);

            modelBuilder.Entity<City>()
                .HasMany(e => e.Companies)
                .WithRequired(e => e.City)
                .HasForeignKey(e => e.FK_City)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<City>()
                .HasMany(e => e.Districts)
                .WithRequired(e => e.City)
                .HasForeignKey(e => e.FK_City)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<City>()
                .HasMany(e => e.ShowPages)
                .WithRequired(e => e.City)
                .HasForeignKey(e => e.FK_City)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Company>()
                .HasMany(e => e.ReportsList)
                .WithOptional(e => e.Company)
                .HasForeignKey(e => e.FK_Company);

            modelBuilder.Entity<Company>()
                .HasMany(e => e.ResponseShowComments)
                .WithRequired(e => e.Company)
                .HasForeignKey(e => e.FK_Company)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Company>()
                .HasMany(e => e.ShowPages)
                .WithRequired(e => e.Company)
                .HasForeignKey(e => e.FK_Company)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<District>()
                .HasMany(e => e.ShowPage)
                .WithRequired(e => e.District)
                .HasForeignKey(e => e.FK_District)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<District>()
                .HasMany(e => e.Companies)
                .WithRequired(e => e.District)
                .HasForeignKey(e => e.FK_District)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Keywords>()
                .HasMany(e => e.ShowPages)
                .WithMany(e => e.KeywordsList)
                .Map(m => m.ToTable("PageToKeyword"));

            modelBuilder.Entity<ResponseShowComment>()
                .HasMany(e => e.ReportsList)
                .WithOptional(e => e.ResponseShowComment)
                .HasForeignKey(e => e.FK_ResponseShowComment);

            modelBuilder.Entity<ShowComment>()
                .HasMany(e => e.ReportsList)
                .WithOptional(e => e.ShowComment)
                .HasForeignKey(e => e.FK_ShowComment);

            modelBuilder.Entity<ShowPage>()
                .HasMany(e => e.PageToTodaysList)
                .WithRequired(e => e.ShowPage)
                .HasForeignKey(e => e.FK_ShowPage)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<ShowPage>()
                .HasMany(e => e.PageViewCounts)
                .WithOptional(e => e.ShowPage)
                .HasForeignKey(e => e.FK_ShowPage);

            modelBuilder.Entity<ShowPage>()
                .HasMany(e => e.ReportsList)
                .WithOptional(e => e.ShowPage)
                .HasForeignKey(e => e.FK_ShowPage);

            modelBuilder.Entity<ShowPage>()
                .HasMany(e => e.ShowComments)
                .WithRequired(e => e.ShowPage)
                .HasForeignKey(e => e.FK_ShowPage)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<ShowPage>()
                .HasMany(e => e.ShowPageFiles)
                .WithRequired(e => e.ShowPage)
                .HasForeignKey(e => e.FK_ShowPage)
                .WillCascadeOnDelete(true);
        }
    }


    public class ApplicationUserStore :
    UserStore<ApplicationUser, ApplicationRole, int,
    ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>, IUserStore<ApplicationUser, int>, IDisposable
    {
        public ApplicationUserStore()
            : this(new IdentityDbContext())
        {
            base.DisposeContext = true;
        }

        public ApplicationUserStore(DbContext context)
            : base(context)
        {
        }
    }


    public class ApplicationRoleStore
    : RoleStore<ApplicationRole, int, ApplicationUserRole>,
    IQueryableRoleStore<ApplicationRole, int>,
    IRoleStore<ApplicationRole, int>, IDisposable
    {
        public ApplicationRoleStore()
            : base(new IdentityDbContext())
        {
            base.DisposeContext = true;
        }

        public ApplicationRoleStore(DbContext context)
            : base(context)
        {
        }
    }
}