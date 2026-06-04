using CourseManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
namespace CourseManagementSystem.Repository
{
    public class WishlistRepository : Repository<Wishlist>, IWishlistRepository
    {
        public WishlistRepository(ApplicationDbContext context) : base(context) { }

        public async Task<bool> IsInWishlistAsync(string studentId, int courseId)
        {
            return await _context.Wishlists.AnyAsync(w => w.StudentId == studentId && w.CourseId == courseId);
        }
    }
}
//        private readonly ApplicationDbContext _context;

//        public WishlistRepository(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        public IEnumerable<Wishlist> GetWishlistByStudentId(string studentId)
//        {
//            return _context.Wishlists
//                .Where(w => w.StudentId == studentId)
//                .Include(w => w.Course)
//                .ToList();
//        }

//        public void Add(Wishlist wishlist)
//        {
//            _context.Wishlists.Add(wishlist);
//        }

//        public void Delete(string studentId, int courseId)
//        {
//            var wishlist = _context.Wishlists
//                .FirstOrDefault(w => w.StudentId == studentId
//                                  && w.CourseId == courseId);

//            if (wishlist != null)
//            {
//                _context.Wishlists.Remove(wishlist);
//            }
//        }

//        public void Save()
//        {
//            _context.SaveChanges();
//        }

//    }
//}
