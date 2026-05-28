using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace CourseManagementSystem.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<Section> Sections { get; set; }

        public DbSet<Lesson> Lessons { get; set; }

        public DbSet<Enrollment> Enrollments { get; set; }

        public DbSet<Review> Reviews { get; set; }

        public DbSet<Wishlist> Wishlists { get; set; }

        public DbSet<LessonProgress> LessonProgresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Enrollment Composite Key
            modelBuilder.Entity<Enrollment>()
                .HasKey(e => new { e.StudentId, e.CourseId });

            // Wishlist Composite Key
            modelBuilder.Entity<Wishlist>()
                .HasKey(w => new { w.StudentId, w.CourseId });

            // Course -> Instructor
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Instructor)
                .WithMany(u => u.Courses)
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Enrollment -> Student
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(u => u.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Review -> Student
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Student)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.StudentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
