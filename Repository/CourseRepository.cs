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
        public IEnumerable<Course> GetCoursesByInstructor(string instructorId)
        {
            return _context.Courses
                .Include(c => c.Category)
                .Where(c => c.InstructorId == instructorId)
                .ToList();
        }

        public IEnumerable<Course> SearchCourses(string? search, int? categoryId)
        {
            var query = _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Instructor)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c =>
                   EF.Functions.Like(c.Title, $"%{search}%") ||
                    c.Description.Contains(search));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(c => c.CategoryId == categoryId.Value);
            }

            return query.ToList();
        }
    }
}
