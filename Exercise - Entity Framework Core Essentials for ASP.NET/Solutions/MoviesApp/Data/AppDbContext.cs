namespace MoviesApp.Data {
    using Microsoft.EntityFrameworkCore;
    using MoviesApp.Data.Models;

    public class AppDbContext : DbContext {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Movie> Movies { get; set; } = null!;
        public DbSet<Watchlist> Watchlists { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Watchlist>()
                .HasKey(w => new { w.UserId, w.MovieId });

            // Further relationships can be configured here if a User model is introduced.
        }
    }
}
