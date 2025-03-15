using Auction_Website.Common;
using Auction_Website.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Auction_Website.DAL.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public DbInitializer(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _db = db;
        }

        public async Task InitializeAsync()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Any())
                {
                    await _db.Database.MigrateAsync();
                }
                if (!await _roleManager.RoleExistsAsync(StaticDetails.Role_Admin))
                {
                    await _roleManager.CreateAsync(new IdentityRole(StaticDetails.Role_Admin));
                }

                if (!await _roleManager.RoleExistsAsync(StaticDetails.Role_User))
                {
                    await _roleManager.CreateAsync(new IdentityRole(StaticDetails.Role_User));
                }

                if (await _userManager.FindByEmailAsync("admin@auction.com") == null)
                {
                    var adminUser = new ApplicationUser
                    {
                        UserName = "Admin User",
                        Email = "admin@auction.com",
                        FirstName = "Admin",
                        LastName = "User",
                        WalletBalance = 1000.00m,
                        IsActive = true,
                        EmailConfirmed = true
                    };

                    var result = await _userManager.CreateAsync(adminUser, "Admin@123");

                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(adminUser, StaticDetails.Role_Admin);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Initialization failed: {ex.Message}");
                throw;
            }
        }
    }
}