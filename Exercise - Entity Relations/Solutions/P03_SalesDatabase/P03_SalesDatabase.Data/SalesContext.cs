namespace P03_SalesDatabase.Data {
    using Microsoft.EntityFrameworkCore;
    using P03_SalesDatabase.Data.Models;

    public class SalesContext : DbContext {
        public SalesContext() { }

        public SalesContext(DbContextOptions options) : base(options) { }

        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Store> Stores { get; set; } = null!;
        public DbSet<Sale> Sales { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            if (!optionsBuilder.IsConfigured) {
                optionsBuilder.UseSqlServer("Server=.;Database=SalesDatabase;Integrated Security=True;Encrypt=False;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Customer>(entity => {
                entity.Property(e => e.Email).IsUnicode(false);
            });

            modelBuilder.Entity<Product>(entity => {
                entity.Property(p => p.Description).HasMaxLength(250).HasDefaultValue("No description");
            });

            modelBuilder.Entity<Sale>(entity => {
                entity.Property(s => s.Date).HasDefaultValueSql("GETDATE()");
            });
        }
    }
}
