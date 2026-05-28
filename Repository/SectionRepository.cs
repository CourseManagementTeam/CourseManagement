using CourseManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementSystem.Repository
{
    public class SectionRepository : Repository<Section>, ISectionRepository
    {
        public SectionRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        public IEnumerable<Section> GetSectionsByCourseId(int courseId)
        {
            return _context.Sections
                .Where(s => s.CourseId == courseId)
                .Include(s => s.Lessons)
                .ToList();
        }
    }
}
