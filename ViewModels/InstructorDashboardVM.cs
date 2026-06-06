using CourseManagementSystem.Models;

namespace CourseManagementSystem.ViewModels
{
    public class InstructorDashboardVM
    {
        public int TotalCourses { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public List<Course> Courses { get; set; } = new();
    }

    public class InstructorStudentVM
    {
        public string StudentId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public DateTime EnrolledAt { get; set; }
        public int ProgressPercentage { get; set; }
    }
}