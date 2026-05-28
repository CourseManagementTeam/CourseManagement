using CourseManagementSystem.Models;

namespace CourseManagementSystem.Repository
{
    public interface IReviewRepository : IRepository<Review>
    {
        IEnumerable<Review> GetReviewsByCourseId(int courseId);
    }
}
