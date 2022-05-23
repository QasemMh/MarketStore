using MarketStore.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace MarketStore.constants
{
    public static class DefaultData
    {
        public async static Task SeedAsync(ModelContext context)
        {

            //seed roles
            if (!context.Roles.Any())
            {
                await context.Roles.AddRangeAsync(
                    new Role { Name = "Admin" },
                    new Role { Name = "Customer" }
                );

                await context.SaveChangesAsync();

            }

            //seed default users
            var defaultUser = new User
            {
                Username = "admin",
                HashPassword = SecurePasswordHasher.Hash("tahaluf"),
                RoleId = context.Roles.FirstOrDefault(r => r.Name == "Admin").Id
            };

            var user = await context.Users
                .FirstOrDefaultAsync(u => u.Username == defaultUser.Username);

            if (user == null)
            {
                await context.Users.AddAsync(defaultUser);
                await context.SaveChangesAsync();
            }





        }

    }
}
