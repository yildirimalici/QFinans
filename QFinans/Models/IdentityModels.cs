using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using QFinans.Areas.Api.Models;

namespace QFinans.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "İsim")]
        public string Name { get; set; }

        [Display(Name = "Soyisim")]
        public string SurName { get; set; }
        public bool SoundOff { get; set; }

        public bool IsAdmin { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base() { }
        public ApplicationRole(string roleName) : base(roleName) { }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            //for auto migrations
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Migrations.Configuration>("DefaultConnection"));
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        //for removing s from model name
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }

        //models
        public DbSet<AccountInfo> AccountInfo { get; set; }
        public DbSet<AccountTransactions> AccountTransactions { get; set; }
        public DbSet<ApiUsers> ApiUsers { get; set; }
        public DbSet<AccountInfoType> AccountInfoType { get; set; }
        public DbSet<DeletedUser> DeletedUser { get; set; }
        public DbSet<AccountInfoStatus> AccountInfoStatus { get; set; }
        public DbSet<SimLocation> SimLocation { get; set; }
        public DbSet<BlackList> BlackList { get; set; }
        public DbSet<AccountAmountRedirect> AccountAmountRedirect { get; set; }
        public DbSet<ShiftType> ShiftType { get; set; }
        public DbSet<ShiftTypeAccountInfo> ShiftTypeAccountInfo { get; set; }
        public DbSet<CashFlowType> CashFlowType { get; set; }
        public DbSet<CashFlow> CashFlow { get; set; }
        public DbSet<DrawSplit> DrawSplit { get; set; }

        public DbSet<SystemParameters> SystemParameters { get; set; }
        public DbSet<Log> Log { get; set; }

        public DbSet<CoinParameters> CoinParameters { get; set; }
        public DbSet<Cryptocurrency> Cryptocurrency { get; set; }

        public DbSet<BankType> BankType { get; set; }
        public DbSet<BankInfo> BankInfo { get; set; }
        public DbSet<CustomerBankInfo> CustomerBankInfo { get; set; }

        public DbSet<CallbackUrl> CallbackUrl { get; set; }
    }
}