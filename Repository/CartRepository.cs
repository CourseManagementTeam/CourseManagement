using CourseManagementSystem.Models;

namespace CourseManagementSystem.Repository
{
    public class CartRepository : Repository<CartItem>, ICartRepository
    {
        public CartRepository(ApplicationDbContext context)
            : base(context)
        {
        }
    }
}
