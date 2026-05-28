namespace CourseManagementSystem.Models
{
    public class Enrollment
    {
        // Composite Key
        public string StudentId { get; set; }

        public int CourseId { get; set; }
        //******************** Additional Properties ***********************
        public DateTime EnrolledAt { get; set; } = DateTime.Now;

        public int ProgressPercentage { get; set; } = 0;

        public bool IsCompleted { get; set; } = false;

        // Navigation Properties
        public ApplicationUser? Student { get; set; }

        public Course? Course { get; set; }
      
    }
}
