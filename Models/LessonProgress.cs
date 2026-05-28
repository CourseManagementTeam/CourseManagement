namespace CourseManagementSystem.Models
{
    public class LessonProgress
    {
        public int Id { get; set; }

        public string StudentId { get; set; }

        public int LessonId { get; set; }

        public bool IsCompleted { get; set; } = false;

        public DateTime? WatchedAt { get; set; }

        //********************** Navigation Properties ***********************
        public ApplicationUser? Student { get; set; }

        public Lesson? Lesson { get; set; }
    }
}
