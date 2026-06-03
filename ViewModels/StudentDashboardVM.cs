using CourseManagementSystem.Models;

namespace CourseManagementSystem.ViewModels
{
    public class StudentDashboardVM
    {
        public int TotalCourses { get; set; }

        public int CompletedCourses { get; set; }

        public int WishlistCount { get; set; }

        public int ReviewsCount { get; set; }

        public List<Enrollment>? Enrollments { get; set; }

        public List<Course>? WishlistCourses { get; set; }

        public List<Review>? RecentReviews { get; set; }
    }
}
