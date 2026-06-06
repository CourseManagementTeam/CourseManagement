using CourseManagementSystem.Models;

namespace CourseManagementSystem.ViewModels
{
    public class HomeViewModel
    {
        public List<Course> FeaturedCourses { get; set; } = new();
        public List<Course> TrendingCourses { get; set; } = new();
        public List<Course> NewCourses { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public List<Review> TopReviews { get; set; } = new();
        public List<Course> MostPopularCourses { get; set; } = new();

        public int TotalCourses { get; set; }
        public int TotalStudents { get; set; }
        public int TotalInstructors { get; set; }
        public int TotalCategories { get; set; }
    }
}
