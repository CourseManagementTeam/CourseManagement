namespace CourseManagementSystem.ViewModels
{
    public class CategoryDetailsVM
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int TotalCourses { get; set; }
        public int TotalStudents { get; set; }
        public double AverageRating { get; set; }
        public List<CourseListVM> Courses { get; set; } = new();
    }
}
