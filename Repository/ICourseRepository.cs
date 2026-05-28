using CourseManagementSystem.Models;

namespace CourseManagementSystem.Repository
{
    public interface ICourseRepository : IRepository<Course>
    {
        IEnumerable<Course> GetCoursesWithCategoryAndInstructor();

        Course? GetCourseDetails(int id);
    }
    
}
