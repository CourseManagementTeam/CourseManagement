using System.ComponentModel.DataAnnotations;
using CourseManagementSystem.Models;

namespace CourseManagementSystem.ViewModels
{
    public class LessonViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Lesson title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; } = string.Empty;

        public int SectionId { get; set; }

        [Display(Name = "Video Type")]
        public string? VideoType { get; set; }

        [Display(Name = "YouTube URL")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        public string? VideoUrl { get; set; }

        [Display(Name = "Upload Video File")]
        public IFormFile? VideoFile { get; set; }

        public string? ExistingVideoFileName { get; set; }

        [Display(Name = "Duration (minutes)")]
        public int DurationMinutes { get; set; }

        [Display(Name = "Is Free Preview")]
        public bool IsFreePreview { get; set; }

        [Display(Name = "Attachments")]
        public List<IFormFile> AttachmentFiles { get; set; } = new();

        public List<AttachmentVM> ExistingAttachments { get; set; } = new();
    }
}
