using System.ComponentModel.DataAnnotations;

namespace CourseManagementSystem.ViewModels
{
    public class ReviewVM
    {
        public int CourseId { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 5)]
        public string Comment { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }
    }
}
