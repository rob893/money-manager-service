using System.Collections.Generic;
using MoneyManagerService.Entities;
using Newtonsoft.Json;
using System.Linq;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Globalization;

namespace MoneyManagerService.Data
{
    public class Seeder
    {
        private readonly DataContext context;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<Role> roleManager;


        public Seeder(DataContext context, UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            this.context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public void SeedDatabase(bool seedData, bool clearCurrentData, bool applyMigrations, bool dropDatabase)
        {
            if (dropDatabase)
            {
                context.Database.EnsureDeleted();
            }

            if (applyMigrations)
            {
                context.Database.Migrate();
            }

            if (clearCurrentData)
            {
                ClearAllData();
            }

            if (seedData)
            {
                SeedRoles();
                SeedUsers();
                SeedBudgets();
            }
        }

        private void ClearAllData()
        {
            context.RefreshTokens.RemoveRange(context.RefreshTokens);
            context.TickerTimeSeries.RemoveRange(context.TickerTimeSeries);
            context.Budgets.RemoveRange(context.Budgets);
            context.Users.RemoveRange(context.Users);
            context.Roles.RemoveRange(context.Roles);

            context.SaveChanges();
        }

        private void SeedRoles()
        {
            if (this.context.Roles.Any())
            {
                return;
            }

            var data = File.ReadAllText("Data/SeedData/RoleSeedData.json");
            var roles = JsonConvert.DeserializeObject<List<Role>>(data);

            if (roles == null)
            {
                return;
            }

            foreach (var role in roles)
            {
                this.roleManager.CreateAsync(role).Wait();
            }
        }

        private void SeedUsers()
        {
            if (this.userManager.Users.Any())
            {
                return;
            }

            var data = File.ReadAllText("Data/SeedData/UserSeedData.json");
            var users = JsonConvert.DeserializeObject<List<User>>(data);

            if (users == null)
            {
                return;
            }

            foreach (var user in users)
            {
                this.userManager.CreateAsync(user, "password").Wait();

                if (user.UserName.ToUpper(CultureInfo.CurrentCulture) == "ADMIN")
                {
                    this.userManager.AddToRoleAsync(user, "Admin").Wait();
                    this.userManager.AddToRoleAsync(user, "User").Wait();
                }
                else
                {
                    this.userManager.AddToRoleAsync(user, "User").Wait();
                }
            }
        }

        private void SeedBudgets()
        {
            if (this.context.Budgets.Any())
            {
                return;
            }

            var data = File.ReadAllText("Data/SeedData/BudgetSeedData.json");

            var budgets = JsonConvert.DeserializeObject<List<Budget>>(data);

            if (budgets == null)
            {
                return;
            }

            this.context.AddRange(budgets);

            this.context.SaveChanges();
        }
    }
}