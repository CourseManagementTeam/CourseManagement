using CourseManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementSystem.Repository
{
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        public ReviewRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        public IEnumerable<Review> GetReviewsByCourseId(int courseId)
        {
            return _context.Reviews
                .Where(r => r.CourseId == courseId)
                .Include(r => r.Student)
                .ToList();
        }
    }
}
