namespace CJR_FinancialPortal.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<CJR_FinancialPortal.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(CJR_FinancialPortal.Models.ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(
                                new RoleStore<IdentityRole>(context));
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }

            var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));
            if (!context.Users.Any(u => u.Email == "ransomcjjr@gmail.com"))
            {
                userManager.Create(new ApplicationUser
                {

                    UserName = "ransomcjjr@gmail.com",
                    FirstName = "C John",
                    LastName = "Ransom",
                    Address1 = "100 Miles Place",
                    City = "Lyncbure",
                    State = "VA",
                    ZipCode = "24502",
                    CellNumber = "4346618140",
                    Email = "ransomcjjr@gmail.com",
                    EmailConfirmed = true
                }, "change2016!");
            }

            var userId = userManager.FindByEmail("ransomcjjr@gmail.com").Id;
            userManager.AddToRole(userId, "Admin");
        }
        
    }
}
