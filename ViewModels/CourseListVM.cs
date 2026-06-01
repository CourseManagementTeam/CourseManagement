namespace CourseManagementSystem.ViewModels
{
    public class CourseListVM
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        public string? CategoryName { get; set; }

        public string? InstructorName { get; set; }

        public double AverageRating { get; set; }
    }
}
