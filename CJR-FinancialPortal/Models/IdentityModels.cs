using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CJR_FinancialPortal.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public int? HouseHoldId { get; set; }
        public bool? HHAuthorized { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "Full Name")]
        public string FullName
        {
            get
            {
                return string.Format("{0} {1}", FirstName, LastName);
            }
        }

        [Display(Name = "Address Line 1")]
        public string Address1 { get; set; }
        [Display(Name = "Address Line 2")]
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        [Display(Name = "Zip")]
        public string ZipCode { get; set; }
        [Display(Name = "Cell#")]
        public string CellNumber { get; set; }

        public ApplicationUser()
        {
            this.HouseHolds = new HashSet<HouseHold>();
            this.Budgets = new HashSet<Budget>();
            this.BankAccounts = new HashSet<BankAccount>();
            this.Transactions = new HashSet<Transaction>();
            this.Notificaitions = new HashSet<Notification>();
            this.Merchants = new HashSet<Merchant>();
            this.AuditBudgets = new HashSet<AuditBudget>();
            this.AuditAccounts = new HashSet<AuditAccount>();
            this.AuditTransactions = new HashSet<AuditTransaction>();
            this.Invitations = new HashSet<AuditTransaction>();
        }

        public virtual ICollection<HouseHold> HouseHolds { get; set; }
        public virtual ICollection<Budget> Budgets { get; set; }
        public virtual ICollection<BankAccount> BankAccounts { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual ICollection<Notification> Notificaitions { get; set; }

        public virtual ICollection<Merchant> Merchants { get; set; }
        public virtual ICollection<AuditBudget> AuditBudgets { get; set; }
        public virtual ICollection<AuditAccount> AuditAccounts { get; set; }
        public virtual ICollection<AuditTransaction> AuditTransactions { get; set; }
        public virtual ICollection<AuditTransaction> Invitations { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            userIdentity.AddClaim(new Claim("Name", FullName));

            userIdentity.AddClaim(new Claim("HouseHoldId", HouseHoldId.ToString()));

            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        //Add all Database Tables/Models Here
        public DbSet<HouseHold> HouseHolds { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<BudgetCategory> BudgetCategories { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<AuditBudget> AuditBudgets { get; set; }
        public DbSet<AuditAccount> AuditAccounts { get; set; }
        public DbSet<AuditTransaction> AuditTransactions { get; set; }
        public DbSet<Merchant> Merchants { get; set; }
        public DbSet<PaymentType> PaymentTypes { get; set; }
        public DbSet<AccountType> AccountTypes { get; set; }
        public DbSet<Invitation> Invitations { get; set; }

       // public System.Data.Entity.DbSet<CJR_FinancialPortal.Models.ApplicationUser> ApplicationUsers { get; set; }

        // public System.Data.Entity.DbSet<CJR_FinancialPortal.Models.ApplicationUser> ApplicationUsers { get; set; }
    }
}