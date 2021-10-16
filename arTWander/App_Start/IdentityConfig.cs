using System.Linq;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace arTWander.Models
{
    // 設定此應用程式中使用的應用程式使用者管理員。UserManager 在 ASP.NET Identity 中定義且由應用程式中使用。

    // *** PASS IN TYPE ARGUMENT TO BASE CLASS:
    public class ApplicationUserManager : UserManager<ApplicationUser, int>
    {
        // *** ADD INT TYPE ARGUMENT TO CONSTRUCTOR CALL:
        public ApplicationUserManager(IUserStore<ApplicationUser, int> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(
            IdentityFactoryOptions<ApplicationUserManager> options,
            IOwinContext context)
        {
            // *** PASS CUSTOM APPLICATION USER STORE AS CONSTRUCTOR ARGUMENT:
            var manager = new ApplicationUserManager(
                new ApplicationUserStore(context.Get<ApplicationDbContext>()));

            // 設定使用者名稱的驗證邏輯
            manager.UserValidator = new UserValidator<ApplicationUser, int>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                
                //Email不能重複
                RequireUniqueEmail = true
            };

            // 設定密碼的驗證邏輯
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // 設定使用者鎖定詳細資料
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // 註冊雙因素驗證提供者。此應用程式使用手機和電子郵件接收驗證碼以驗證使用者
            // 您可以撰寫專屬提供者，並將它外掛到這裡。
            manager.RegisterTwoFactorProvider("電話代碼",
                new PhoneNumberTokenProvider<ApplicationUser, int>
                {
                    MessageFormat = "您的安全碼為 {0}"
                });

            // *** ADD INT TYPE ARGUMENT TO METHOD CALL:
            manager.RegisterTwoFactorProvider("電子郵件代碼",
                new EmailTokenProvider<ApplicationUser, int>
                {
                    Subject = "安全碼",
                    BodyFormat = "您的安全碼為 {0}"
                });

            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                // *** ADD INT TYPE ARGUMENT TO METHOD CALL:
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser, int>(
                        dataProtectionProvider.Create("ASP.NET Identity"))
                    {
                        //設定忘記密碼或mail確認的timeout時間
                        TokenLifespan = TimeSpan.FromHours(3)
                    };
            }
            return manager;
        }
    }


    // PASS CUSTOM APPLICATION ROLE AND INT AS TYPE ARGUMENTS TO BASE:
    public class ApplicationRoleManager : RoleManager<ApplicationRole, int>
    {
        // PASS CUSTOM APPLICATION ROLE AND INT AS TYPE ARGUMENTS TO CONSTRUCTOR:
        public ApplicationRoleManager(IRoleStore<ApplicationRole, int> roleStore)
            : base(roleStore)
        {
        }

        // PASS CUSTOM APPLICATION ROLE AS TYPE ARGUMENT:
        public static ApplicationRoleManager Create(
            IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            return new ApplicationRoleManager(
                new ApplicationRoleStore(context.Get<ApplicationDbContext>()));
        }
    }


    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            //Console.WriteLine("SendAsync");
            // 將您的電子郵件服務外掛到這裡以傳送電子郵件。
            //var apiKey = "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";
            //var client = new SendGridClient(apiKey);//admin@artwander.art
            //var from = new EmailAddress("a975409@gmail.com", "arTWander");
            //var subject = "Sending with SendGrid is Fun";
            //var to = new EmailAddress(message.Destination, "Example User");
            //var plainTextContent = "請按一下此連結確認您的帳戶";
            //var htmlContent = message.Body;
            //var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            //return client.SendEmailAsync(msg);
            return Task.FromResult(0);
        }
    }


    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // 將您的 SMS 服務外掛到這裡以傳送簡訊。
            return Task.FromResult(0);
        }
    }

    // This is useful if you do not want to tear down the database each time you run the application.
    // public class ApplicationDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    // This example shows you how to create a new database if the Model changes
    //DropCreateDatabaseAlways=>移除現有DB(如果存在的話)，再建立新DB
    //DropCreateDatabaseIfModelChanges=>DB不存在時建立，若Model與目前存在DB不相符時會自動移除現有DB後再建立新DB
    public class ApplicationDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext> 
    {
        protected override void Seed(ApplicationDbContext context) {
            InitializeIdentityForEF(context);
            base.Seed(context);
        }

        //Create User=Admin@Admin.com with password=Admin@123456 in the Admin role        
        public static void InitializeIdentityForEF(ApplicationDbContext db) {
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var roleManager = HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();
            const string name = "a975409@gmail.com";
            const string password = "@Acs856745";
            const string roleName = "Admin";//系統管理員
            const string CompanyName = "Company";//展演單位
            const string MemberName = "Member";//一般會員
            const string BlacklistName = "Blacklist";//黑名單

            //新增系統管理員權限
            var role = roleManager.FindByName(roleName);
            if (role == null) {
                role = new ApplicationRole(roleName);
                var roleresult = roleManager.Create(role);
            }

            //新增展演單位權限
            var Company = roleManager.FindByName(CompanyName);
            if (Company == null)
            {
                Company = new ApplicationRole(CompanyName);
                var roleresult = roleManager.Create(Company);
            }

            //新增一般會員權限
            var Member = roleManager.FindByName(MemberName);
            if (Member == null)
            {
                Member = new ApplicationRole(MemberName);
                var roleresult = roleManager.Create(Member);
            }

            //新增黑名單權限
            var Blacklist = roleManager.FindByName(BlacklistName);
            if (Blacklist == null)
            {
                Blacklist = new ApplicationRole(BlacklistName);
                var roleresult = roleManager.Create(Blacklist);
            }

            //新增使用者（User）
            var user = userManager.FindByName(name);
            if (user == null) {
                user = new ApplicationUser { UserName = name, Email = name, EmailConfirmed = true };
                var result = userManager.Create(user, password);
                result = userManager.SetLockoutEnabled(user.Id, false);
            }

            //將User加入至系統管理員
            var rolesForUser = userManager.GetRoles(user.Id);
            if (!rolesForUser.Contains(role.Name)) {
                var result = userManager.AddToRole(user.Id, role.Name);
            }
        }
    }

    // 設定在此應用程式中使用的應用程式登入管理員。
    public class ApplicationSignInManager : SignInManager<ApplicationUser, int>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager) : 
            base(userManager, authenticationManager) { }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}