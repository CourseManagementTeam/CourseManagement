namespace CourseManagementSystem.ViewModels
{
    public class CourseDetailsVM
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        public string? CategoryName { get; set; }

        public string? InstructorName { get; set; }

        public double AverageRating { get; set; }

        public int ReviewsCount { get; set; }

        public int SectionsCount { get; set; }

        public int LessonsCount { get; set; }

        public bool IsEnrolled { get; set; }

        public bool IsInWishlist { get; set; }

        public bool IsInCart { get; set; }
    }
}
