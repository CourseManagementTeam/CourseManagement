using CourseManagementSystem.Models;
using CourseManagementSystem.Repository;

namespace CourseManagementSystem
{
    public interface ILessonRepository : IRepository<Lesson>
    {
        IEnumerable<Lesson> GetLessonsBySectionId(int sectionId);
    }
}
