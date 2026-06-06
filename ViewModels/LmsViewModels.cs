namespace CourseManagementSystem.ViewModels
{
    public class AttachmentVM
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public long FileSize { get; set; }

        public string FileSizeDisplay => FileSize switch
        {
            < 1024 => $"{FileSize} B",
            < 1024 * 1024 => $"{FileSize / 1024} KB",
            _ => $"{FileSize / (1024 * 1024)} MB"
        };

        public string FileIcon => FileType.ToLower() switch
        {
            "pdf"  => "bi-file-earmark-pdf-fill text-danger",
            "docx" => "bi-file-earmark-word-fill text-primary",
            "pptx" => "bi-file-earmark-ppt-fill text-warning",
            "zip"  => "bi-file-zip-fill text-secondary",
            _      => "bi-file-earmark-fill"
        };
    }

    public class LessonVM
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int OrderNumber { get; set; }
        public int DurationMinutes { get; set; }
        public bool IsFreePreview { get; set; }
        public string? VideoUrl { get; set; }
        public string? VideoType { get; set; }
        public string? UploadedVideoFileName { get; set; }
        public List<AttachmentVM> Attachments { get; set; } = new();

        public bool HasVideo => !string.IsNullOrEmpty(VideoUrl) || !string.IsNullOrEmpty(UploadedVideoFileName);

        public string? EmbedUrl
        {
            get
            {
                if (string.IsNullOrEmpty(VideoUrl)) return null;
                if (VideoUrl.Contains("youtube.com/watch?v="))
                    return VideoUrl.Replace("youtube.com/watch?v=", "youtube.com/embed/").Split('&')[0];
                if (VideoUrl.Contains("youtu.be/"))
                    return "https://www.youtube.com/embed/" + VideoUrl.Split('/').Last().Split('?')[0];
                return VideoUrl;
            }
        }
    }

    public class SectionVM
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int OrderNumber { get; set; }
        public List<LessonVM> Lessons { get; set; } = new();

        public int TotalDurationMinutes => Lessons.Sum(l => l.DurationMinutes);
        public int LessonCount => Lessons.Count;
    }

    public class ReviewDisplayVM
    {
        public int Id { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentInitial => StudentName.Length > 0 ? StudentName.Substring(0, 1).ToUpper() : "?";
        public int Rate { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class InstructorVM
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? ProfileImage { get; set; }
        public int TotalCourses { get; set; }
        public int TotalStudents { get; set; }
    }

    public class CourseDetailsFullVM
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? WhatYouWillLearn { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public string? Level { get; set; }
        public int TotalHours { get; set; }
        public double AverageRating { get; set; }
        public int ReviewsCount { get; set; }
        public int StudentCount { get; set; }
        public string? CategoryName { get; set; }
        public int CategoryId { get; set; }

        public bool IsEnrolled { get; set; }
        public bool IsInWishlist { get; set; }
        public bool IsInCart { get; set; }
        public bool HasReviewed { get; set; }

        public InstructorVM Instructor { get; set; } = new();
        public List<SectionVM> Sections { get; set; } = new();
        public List<ReviewDisplayVM> Reviews { get; set; } = new();
        public List<CourseListVM> RelatedCourses { get; set; } = new();

        public List<string> LearnItems =>
            (WhatYouWillLearn ?? string.Empty)
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())
            .Where(s => s.Length > 0)
            .ToList();

        public int TotalLessons => Sections.Sum(s => s.LessonCount);
        public int TotalDurationMinutes => Sections.Sum(s => s.TotalDurationMinutes);
    }
}
