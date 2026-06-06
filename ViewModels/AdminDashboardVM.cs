using CourseManagementSystem.Models;

namespace CourseManagementSystem.ViewModels
{
    public class AdminDashboardVM
    {
        public int TotalUsers { get; set; }
        public int TotalStudents { get; set; }
        public int TotalInstructors { get; set; }
        public int TotalCourses { get; set; }
        public int TotalCategories { get; set; }
        public int TotalEnrollments { get; set; }
        public int TotalReviews { get; set; }
        public List<Course> RecentCourses { get; set; } = new();
        public List<Enrollment> RecentEnrollments { get; set; } = new();
    }
}