using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MoneyManagerService.Models.Domain;

namespace MoneyManagerService.Data
{
    public class DataContext : IdentityDbContext<User, Role, int,
        IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>,
        IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<TickerTimeSeries> TickerTimeSeries { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<Expense> Expenses { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserRole>(userRole =>
            {
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

                userRole.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                userRole.HasOne(ur => ur.User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });

            modelBuilder.Entity<RefreshToken>(rToken =>
            {
                rToken.HasKey(k => k.UserId);
            });

            modelBuilder.Entity<TickerTimeSeries>(tickerTimeSeries =>
            {
                tickerTimeSeries.HasIndex(tss => new { tss.Ticker, tss.Date });
            });

            modelBuilder.Entity<Expense>()
                .Property(expense => expense.Frequency)
                .HasConversion<string>();
        }
    }
}