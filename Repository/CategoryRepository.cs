using CourseManagementSystem.Models;

namespace CourseManagementSystem.Repository
{
    public class CategoryRepository: Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context)
           : base(context)
        {
        }
    }
}
