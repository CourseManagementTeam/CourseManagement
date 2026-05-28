using System.ComponentModel.DataAnnotations;

namespace CourseManagementSystem.Models
{
    public class Review
    {
        public int Id { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }

        [Range(1, 5)]
        public int Rate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // ********** Foreign Keys **********
        public string StudentId { get; set; }

        public int CourseId { get; set; }

        //************** Navigation Properties ****************
        public ApplicationUser? Student { get; set; }

        public Course? Course { get; set; }

    }
}
