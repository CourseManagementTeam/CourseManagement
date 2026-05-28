using CourseManagementSystem.Models;

namespace CourseManagementSystem.Repository
{
    public interface ISectionRepository : IRepository<Section>
    {
        IEnumerable<Section> GetSectionsByCourseId(int courseId);
    }
}
