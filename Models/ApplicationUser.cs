using Microsoft.AspNetCore.Identity;

namespace CourseManagementSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }

        public string? ProfileImage { get; set; }

        public string? Bio { get; set; }

        //****************** Navigation Properties **********************

        // Instructor Courses
        public ICollection<Course>? Courses { get; set; }

        // Student Enrollments
        public ICollection<Enrollment>? Enrollments { get; set; }

        // Student Reviews
        public ICollection<Review>? Reviews { get; set; }

        //* Student Wishlist
        public ICollection<Wishlist>? Wishlists { get; set; }

        //* Student Lesson Progress
        public ICollection<LessonProgress>? LessonProgresses { get; set; }
    }
   
}
