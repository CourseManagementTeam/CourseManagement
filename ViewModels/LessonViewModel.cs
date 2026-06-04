using System.ComponentModel.DataAnnotations;

namespace CourseManagementSystem.ViewModels
{
    public class LessonViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Lesson title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; }

        public int SectionId { get; set; }

        [Display(Name = "Video URL")]
        [Url(ErrorMessage = "Please enter a valid URL")] 
        public string? VideoUrl { get; set; }

        [Display(Name = "Is Free Preview")]
        public bool IsFreePreview { get; set; }
    }
}