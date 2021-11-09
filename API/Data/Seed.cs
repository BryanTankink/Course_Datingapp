using System.Text.Json;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            if (await userManager.Users.AnyAsync()) return;
            string userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
            List<AppUser> users = JsonSerializer.Deserialize<List<AppUser>>(userData);
            if(users == null) return;

            var roles = new List<AppRole> {
                new AppRole {Name="Member"},
                new AppRole {Name="Admin"},
                new AppRole {Name="Moderator"}
            };

            foreach(AppRole role in roles) {
                await roleManager.CreateAsync(role);
            }

            foreach(AppUser user in users) {
                user.UserName = user.UserName.ToLower();
                await userManager.CreateAsync(user, "Password1234!");
                await userManager.AddToRoleAsync(user, "Member");
            }

            var admin = new AppUser {
                UserName = "admin"
            };
            
            await userManager.CreateAsync(admin, "Password1234!");
            await userManager.AddToRolesAsync(admin, new string[] {"Admin", "Moderator" });
        }


    }
}