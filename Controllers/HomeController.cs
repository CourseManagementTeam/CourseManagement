using CourseManagementSystem.Models; // تأكد من الـ namespace الصحيح للموديلز بتاعتك
using CourseManagementSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Learnly.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        public HomeController(ApplicationDbContext db) { _db = db; }

        public async Task<IActionResult> Index()
        {
            var vm = new HomeViewModel
            {
                // 1. ترتيب حسب عدد التسجيلات (مع عمل Include للـ Enrollments)
                FeaturedCourses = await _db.Courses
                    .Include(c => c.Category)
                    .Include(c => c.Instructor)
                    .Include(c => c.Enrollments)
                    .OrderByDescending(c => c.Enrollments.Count)
                    .Take(8).ToListAsync(),

                // 2. استخدام الخاصية المحسوبة الجاهزة عندك AverageRating لزيادة الأداء
                TrendingCourses = await _db.Courses
                    .Include(c => c.Category)
                    .Include(c => c.Instructor)
                    .OrderByDescending(c => c.AverageRating)
                    .Take(6).ToListAsync(),

                // 3. تعديل CreatedAt إلى CreatedDate وتصفية الكورسات المنشورة فقط
                NewCourses = await _db.Courses
                    .Include(c => c.Category)
                    .Include(c => c.Instructor)
                    .Where(c => c.IsPublished)
                    .OrderByDescending(c => c.CreatedDate)
                    .Take(4).ToListAsync(),

                // 4. عمل Include للكورسات عشان الـ Count يقرأ صح في الـ View
                Categories = await _db.Categories
                    .Include(c => c.Courses)
                    .Take(8).ToListAsync(),

                // 5. تعديل Rating إلى Rate و User إلى Student
                TopReviews = await _db.Reviews
                    .Include(r => r.Student)
                    .Include(r => r.Course)
                    .Where(r => r.Rate >= 5)
                    .OrderByDescending(r => r.CreatedAt)
                    .Take(3).ToListAsync(),

                TotalCourses = await _db.Courses.CountAsync(),
                TotalStudents = await _db.Users.CountAsync(), // يفضل مستقبلاً فلترتها برول الطالب
                TotalInstructors = await _db.Users.CountAsync(), // يفضل مستقبلاً فلترتها برول المدرس
                TotalCategories = await _db.Categories.CountAsync(),
            };

            return View(vm);
        }
    }
}