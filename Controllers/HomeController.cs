using CourseManagementSystem.Models; // تأكد من الـ namespace الصحيح للموديلز بتاعتك
using CourseManagementSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CourseManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new HomeViewModel
            {
                FeaturedCourses = await _db.Courses
         .Include(c => c.Category)
         .Include(c => c.Instructor)
         .Include(c => c.Enrollments)
         .OrderByDescending(c => c.Enrollments.Count)
         .Take(8).ToListAsync(),

                TrendingCourses = await _db.Courses
         .Include(c => c.Category)
         .Include(c => c.Instructor)
         .OrderByDescending(c => c.AverageRating)
         .Take(6).ToListAsync(),

                NewCourses = await _db.Courses
         .Include(c => c.Category)
         .Include(c => c.Instructor)
         .Where(c => c.IsPublished)
         .OrderByDescending(c => c.CreatedDate)
         .Take(4).ToListAsync(),

                Categories = await _db.Categories
         .Include(c => c.Courses)
         .Take(8).ToListAsync(),

                TopReviews = await _db.Reviews
         .Include(r => r.Student)
         .Include(r => r.Course)
         .Where(r => r.Rate >= 5)
         .OrderByDescending(r => r.CreatedAt)
         .Take(3).ToListAsync(),

                // ⭐ الجديد هنا
                MostPopularCourses = await _db.Courses
         .Include(c => c.Category)
         .Include(c => c.Instructor)
         .Include(c => c.Enrollments)
        .OrderByDescending(c =>
    c.Enrollments.Count * 2 +
    c.Reviews.Count +
    c.AverageRating)
         .Take(6)
         .ToListAsync(),

                TotalCourses = await _db.Courses.CountAsync(),
                TotalStudents = await _db.Users.CountAsync(),
                TotalInstructors = await _db.Users.CountAsync(),
                TotalCategories = await _db.Categories.CountAsync(),
            };

            return View(vm);
        }
    }
}