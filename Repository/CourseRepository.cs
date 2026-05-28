using CourseManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementSystem.Repository
{
    public class CourseRepository : Repository<Course>, ICourseRepository
    {
        public CourseRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        public IEnumerable<Course> GetCoursesWithCategoryAndInstructor()
        {
            return _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Instructor)
                .ToList();
        }

        public Course? GetCourseDetails(int id)
        {
            return _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Instructor)
                .Include(c => c.Sections)
                    .ThenInclude(s => s.Lessons)
                .Include(c => c.Reviews)
                .FirstOrDefault(c => c.Id == id);
        }
    
    }
}
