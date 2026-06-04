using CourseManagementSystem.Models;

namespace CourseManagementSystem.Repository
{
    public interface IWishlistRepository : IRepository<Wishlist>
    {
      
        Task<bool> IsInWishlistAsync(string studentId, int courseId);
    }
}