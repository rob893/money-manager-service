using System;
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
        public DbSet<RefreshToken> RefreshTokens => this.Set<RefreshToken>();
        public DbSet<LinkedAccount> LinkedAccounts => this.Set<LinkedAccount>();
        public DbSet<TickerTimeSeries> TickerTimeSeries => this.Set<TickerTimeSeries>();
        public DbSet<Budget> Budgets => this.Set<Budget>();
        public DbSet<Expense> Expenses => this.Set<Expense>();
        public DbSet<Income> Incomes => this.Set<Income>();
        public DbSet<TaxLiability> TaxLiabilities => this.Set<TaxLiability>();
        public DbSet<Tag> Tags => this.Set<Tag>();

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            base.OnModelCreating(builder);

            builder.Entity<UserRole>(userRole =>
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

            builder.Entity<RefreshToken>(rToken =>
            {
                rToken.HasKey(k => new { k.UserId, k.DeviceId });
            });

            builder.Entity<TickerTimeSeries>(tickerTimeSeries =>
            {
                tickerTimeSeries.HasIndex(tss => new { tss.Ticker, tss.Date });
            });

            builder.Entity<Expense>()
                .Property(expense => expense.Frequency)
                .HasConversion<string>();

            builder.Entity<Budget>()
                .Property(budget => budget.TaxFilingStatus)
                .HasConversion<string>();

            builder.Entity<Income>()
               .Property(income => income.IncomeType)
               .HasConversion<string>();

            builder.Entity<LinkedAccount>(linkedAccount =>
            {
                linkedAccount.HasKey(account => new { account.Id, account.LinkedAccountType });
                linkedAccount.Property(account => account.LinkedAccountType).HasConversion<string>();
            });
        }
    }
}