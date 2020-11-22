using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MoneyManagerService.Entities;

namespace MoneyManagerService.Data
{
    public class DataContext : IdentityDbContext<User, Role, int,
        IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>,
        IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<TickerTimeSeries> TickerTimeSeries => Set<TickerTimeSeries>();
        public DbSet<Budget> Budgets => Set<Budget>();
        public DbSet<Expense> Expenses => Set<Expense>();
        public DbSet<Income> Incomes => Set<Income>();
        public DbSet<TaxLiability> TaxLiabilities => Set<TaxLiability>();

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

            modelBuilder.Entity<Budget>()
                .Property(budget => budget.TaxFilingStatus)
                .HasConversion<string>();

            modelBuilder.Entity<Income>()
               .Property(income => income.IncomeType)
               .HasConversion<string>();
        }
    }
}