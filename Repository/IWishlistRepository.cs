using CourseManagementSystem.Models;

namespace CourseManagementSystem.Repository
{
    public interface IWishlistRepository
    {
        IEnumerable<Wishlist> GetWishlistByStudentId(string studentId);

        void Add(Wishlist wishlist);

        void Delete(string studentId, int courseId);

        void Save();
    }
}
