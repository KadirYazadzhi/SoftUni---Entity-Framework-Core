namespace AcademicRecordsApp.Data {
    using Microsoft.EntityFrameworkCore;
    using AcademicRecordsApp.Data.Models;

    public class AcademicRecordsDbContext : DbContext {
        public AcademicRecordsDbContext() { }

        public AcademicRecordsDbContext(DbContextOptions options) : base(options) { }

        public virtual DbSet<Exam> Exams { get; set; } = null!;
        public virtual DbSet<Grade> Grades { get; set; } = null!;
        public virtual DbSet<Student> Students { get; set; } = null!;
        public virtual DbSet<Course> Courses { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            if (!optionsBuilder.IsConfigured) {
                optionsBuilder.UseSqlServer("Server=.;Database=AcademicRecordsDB;Integrated Security=True;Encrypt=False;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Exam>(entity => {
                entity.Property(e => e.Name).HasMaxLength(100);
            });

            modelBuilder.Entity<Student>(entity => {
                entity.Property(s => s.FullName).HasMaxLength(100);
                
                entity
                    .HasMany(s => s.Courses)
                    .WithMany(c => c.Students)
                    .UsingEntity<Dictionary<string, object>>(
                        "StudentsCourses",
                        j => j.HasOne<Course>().WithMany().HasForeignKey("CoursesId"),
                        j => j.HasOne<Student>().WithMany().HasForeignKey("StudentsId"));
            });

            modelBuilder.Entity<Grade>(entity => {
                entity.Property(e => e.Value).HasColumnType("decimal(3, 2)");
            });
        }
    }
}
