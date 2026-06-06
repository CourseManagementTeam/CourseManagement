using System.ComponentModel.DataAnnotations;

namespace CourseManagementSystem.Models
{
    public class Lesson
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string? VideoUrl { get; set; }

        public int DurationMinutes { get; set; }

        public bool IsFreePreview { get; set; }

        public int OrderNumber { get; set; }

        //*********** Foreign Key ***********
        public int SectionId { get; set; }

        //********************** Navigation Properties ***********************
        public Section? Section { get; set; }

        public string? VideoType { get; set; }

        public string? UploadedVideoFileName { get; set; }

        public ICollection<LessonProgress>? LessonProgresses { get; set; }

        public ICollection<Attachment>? Attachments { get; set; }
    }
}
