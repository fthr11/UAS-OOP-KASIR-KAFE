using kasirkafe.Models;
using Microsoft.EntityFrameworkCore;

namespace kasirkafe.Data
{
    public class CafeDbContext : DbContext
    {
        public CafeDbContext(DbContextOptions<CafeDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Transaction> Transactions { get; set; } = null!;
        public DbSet<TransactionDetail> TransactionDetails { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Konfigurasi decimal
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<FoodProduct>()
                .Property(f => f.JenisMakanan)
                .HasMaxLength(50);
            
            modelBuilder.Entity<DrinkProduct>()
                .Property(d => d.JenisMinuman)
                .HasMaxLength(50);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.TotalAmount)
                .HasPrecision(18, 2);
                

            // SEEDER ADMIN
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    Username = "admin",
                    FullName = "Administrator",
                    Email = "admin@kafe.com",
                    Password = "admin123",     
                    Role = "Admin",
                    CreatedAt = DateTime.Now
                }
            );
        }
    }
}
