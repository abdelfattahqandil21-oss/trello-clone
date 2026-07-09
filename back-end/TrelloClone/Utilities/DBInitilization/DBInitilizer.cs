
using TrelloClone.Utilities.DBInitilization;
using Microsoft.IdentityModel.Tokens;

namespace TrelloClone.Utilities.DBInitilization
{
    public class DBInitilizer : IDBInitilizer
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDpContext _context;

        public DBInitilizer(RoleManager<IdentityRole> roleManager,
            UserManager<AppUser> userManager, AppDpContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
        }
        public async Task Initialize()
        {
            if ((await _context.Database.GetPendingMigrationsAsync()).Any())
            {
                await _context.Database.MigrateAsync();
            }

            if (!await _roleManager.RoleExistsAsync(SD.SUPER_ADMIN_ROLE))
            {
                await _roleManager.CreateAsync(new IdentityRole(SD.SUPER_ADMIN_ROLE));
                await _roleManager.CreateAsync(new IdentityRole(SD.ADMIN_ROLE));
                await _roleManager.CreateAsync(new IdentityRole(SD.EMPLOYEE_ROLE));
                await _roleManager.CreateAsync(new IdentityRole(SD.CUSTOMER_ROLE));

                await _userManager.CreateAsync(
                    new AppUser
                    {
                        DisplayName = "Super Admin",
                        Email = "superadmin@project.com",
                        IsEmailVerified = true,
                        EmailConfirmed = true,
                        IsActive = true,
                        UserName = "SuperAdmin"
                    }, password: "SuperAdmin1234$"
                );
                await _userManager.CreateAsync(
                    new AppUser
                    {
                        DisplayName = "Admin",
                        Email = "admin@project.com",
                        IsEmailVerified = true,
                        EmailConfirmed = true,
                        IsActive = true,
                        UserName = "Admin"
                    }, password: "Admin1234$"
                );
                await _userManager.CreateAsync(
                    new AppUser
                    {
                        DisplayName = "Employee 1",
                        Email = "employee1@project.com",
                        IsEmailVerified = true,
                        EmailConfirmed = true,
                        IsActive = true,
                        UserName = "Employee1"
                    }, password: "Employee1234$"
                );
                await _userManager.CreateAsync(
                    new AppUser
                    {
                        DisplayName = "Employee 2",
                        Email = "employee2@project.com",
                        IsEmailVerified = true,
                        EmailConfirmed = true,
                        IsActive = true,
                        UserName = "Employee2"
                    }, password: "Employee1234$"
                );
                var user1 = await _userManager.FindByNameAsync("SuperAdmin");
                var user2 = await _userManager.FindByNameAsync("Admin");
                var user3 = await _userManager.FindByNameAsync("Employee1");
                var user4 = await _userManager.FindByNameAsync("Employee2");

                if (user1 is not null && user2 is not null && user3 is not null && user4 is not null)
                {
                    await _userManager.AddToRoleAsync(user1, SD.SUPER_ADMIN_ROLE);
                    await _userManager.AddToRoleAsync(user2, SD.ADMIN_ROLE);
                    await _userManager.AddToRoleAsync(user3, SD.EMPLOYEE_ROLE);
                    await _userManager.AddToRoleAsync(user4, SD.EMPLOYEE_ROLE);
                }
            }
        }
    }
}
