using System.ComponentModel.DataAnnotations;

namespace CourseManagementSystem.Models
{
    public class Attachment
    {
        public int Id { get; set; }

        [Required]
        public string FileName { get; set; } = string.Empty;

        [Required]
        public string StoredFileName { get; set; } = string.Empty;

        [Required]
        public string FileType { get; set; } = string.Empty;

        public long FileSize { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public int LessonId { get; set; }

        public Lesson? Lesson { get; set; }
    }
}
