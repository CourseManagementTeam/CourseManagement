namespace CourseManagementSystem.ViewModels
{
    public class AdminUserVM
    {
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int CourseCount { get; set; }
        public int EnrollmentCount { get; set; }
    }
}