using CourseManagementSystem.Models;

namespace CourseManagementSystem.Repository
{
    public interface ICourseRepository : IRepository<Course>
    {
        IEnumerable<Course> GetCoursesWithCategoryAndInstructor();
        IEnumerable<Course> GetCoursesByInstructor(string instructorId);
        IEnumerable<Course> SearchCourses(string? search, int? categoryId);

        Course? GetCourseDetails(int id);
    }
    
}
