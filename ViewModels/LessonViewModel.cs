using System.ComponentModel.DataAnnotations;

namespace CourseManagementSystem.ViewModels
{
    public class LessonViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Lesson title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; }

        [Required]
        public int SectionId { get; set; }
    }
}