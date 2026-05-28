using CourseManagementSystem.Models;

namespace CourseManagementSystem.Repository
{
    public class LessonRepository : Repository<Lesson>, ILessonRepository
    {
        public LessonRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        public IEnumerable<Lesson> GetLessonsBySectionId(int sectionId)
        {
            return _context.Lessons
                .Where(l => l.SectionId == sectionId)
                .ToList();
        }
    }
}
