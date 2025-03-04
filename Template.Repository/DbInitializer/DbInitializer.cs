using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Template.Domain.Identity;
using Template.Repository.EntityFrameworkCore.Context;
using Template.Utilities.Identity;
using Template.Utilities.SeedData;

namespace Template.Repository.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly AppDbContext _context ;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public DbInitializer(AppDbContext context, UserManager<ApplicationUser> userManager,RoleManager<ApplicationRole> roleManager) 
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Initialize()
        {
            try
            {
                if (_context.Database.GetPendingMigrations().Any())
                {
                    _context.Database.Migrate();
                }

                if (!_roleManager.RoleExistsAsync(AppUserRoles.RoleAdmin).GetAwaiter().GetResult())
                {
                    _roleManager.CreateAsync(new ApplicationRole() { Name = AppUserRoles.RoleAdmin }).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new ApplicationRole() { Name = AppUserRoles.RoleCustomer }).GetAwaiter().GetResult();
                }

                if (!_context.ApplicationUsers.Any())
                {
                    ApplicationUser user = new();
                    user.FirstName = "ARAMM";
                    user.LastName = "Winners";
                    user.Email = "admin@aramm.com";
                    user.UserName = "admin@aramm.com";
                    user.PasswordHash = "Aramm123@";

                    var result = _userManager.CreateAsync(user, "Aramm123@").GetAwaiter().GetResult();

                    if (result.Succeeded)
                    {
                        _userManager.AddToRoleAsync(user, AppUserRoles.RoleAdmin).GetAwaiter().GetResult();

                        // Email Confirmed
                        var codeToConfirm = _userManager.GenerateEmailConfirmationTokenAsync(user).GetAwaiter().GetResult();
                        codeToConfirm = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(codeToConfirm));

                        codeToConfirm = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(codeToConfirm));
                        _userManager.ConfirmEmailAsync(user, codeToConfirm).GetAwaiter().GetResult();

                         _userManager.ConfirmEmailAsync(user, codeToConfirm).GetAwaiter().GetResult();

                        // Set Lockout Enabled to false
                        _userManager.SetLockoutEnabledAsync(user, false);
                    }
                }

                 if (!_context.Departments.Any())
                {
                    _context.Departments.AddRange(DepartmentData.LoadDepartments());
                    _context.SaveChanges();
                }
                
            }
            catch (Exception ex)
            {
                throw new Exception($"Something got wrong while initializing the database: {ex.Message}");
            }
        }
    }
}

