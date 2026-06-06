using CourseManagementSystem.Models;
using CourseManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var totalUsers = await _context.Users.CountAsync();

            var totalStudents = await (
                from u in _context.Users
                join ur in _context.UserRoles on u.Id equals ur.UserId
                join r in _context.Roles on ur.RoleId equals r.Id
                where r.Name == "Student"
                select u).CountAsync();

            var totalInstructors = await (
                from u in _context.Users
                join ur in _context.UserRoles on u.Id equals ur.UserId
                join r in _context.Roles on ur.RoleId equals r.Id
                where r.Name == "Instructor"
                select u).CountAsync();

            var recentCourses = await _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Instructor)
                .OrderByDescending(c => c.CreatedDate)
                .Take(5)
                .ToListAsync();

            var recentEnrollments = await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Student)
                .OrderByDescending(e => e.EnrolledAt)
                .Take(5)
                .ToListAsync();

            var vm = new AdminDashboardVM
            {
                TotalUsers        = totalUsers,
                TotalStudents     = totalStudents,
                TotalInstructors  = totalInstructors,
                TotalCourses      = await _context.Courses.CountAsync(),
                TotalCategories   = await _context.Categories.CountAsync(),
                TotalEnrollments  = await _context.Enrollments.CountAsync(),
                TotalReviews      = await _context.Reviews.CountAsync(),
                RecentCourses     = recentCourses,
                RecentEnrollments = recentEnrollments
            };

            return View(vm);
        }

        public async Task<IActionResult> Users()
        {
            var users = await _context.Users.ToListAsync();
            var vm = new List<AdminUserVM>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var courseCount     = await _context.Courses.CountAsync(c => c.InstructorId == user.Id);
                var enrollmentCount = await _context.Enrollments.CountAsync(e => e.StudentId == user.Id);

                vm.Add(new AdminUserVM
                {
                    UserId          = user.Id,
                    FullName        = user.FullName,
                    Email           = user.Email ?? string.Empty,
                    Role            = roles.FirstOrDefault() ?? "N/A",
                    CourseCount     = courseCount,
                    EnrollmentCount = enrollmentCount
                });
            }

            return View(vm);
        }

        public async Task<IActionResult> Courses()
        {
            var courses = await _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Instructor)
                .Include(c => c.Enrollments)
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();

            return View(courses);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                course.IsPublished = !course.IsPublished;
                await _context.SaveChangesAsync();
                TempData["Success"] = course.IsPublished ? "Course published." : "Course unpublished.";
            }
            return RedirectToAction(nameof(Courses));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Enrollments)
                .Include(c => c.Reviews)
                .Include(c => c.Wishlists)
                .Include(c => c.Sections)!
                    .ThenInclude(s => s.Lessons)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course != null)
            {
                if (course.Enrollments?.Any() == true)
                    _context.Enrollments.RemoveRange(course.Enrollments);
                if (course.Reviews?.Any() == true)
                    _context.Reviews.RemoveRange(course.Reviews);
                if (course.Wishlists?.Any() == true)
                    _context.Wishlists.RemoveRange(course.Wishlists);

                var cartItems = await _context.CartItems.Where(c => c.CourseId == id).ToListAsync();
                if (cartItems.Any())
                    _context.CartItems.RemoveRange(cartItems);

                if (course.Sections?.Any() == true)
                {
                    foreach (var section in course.Sections)
                    {
                        if (section.Lessons?.Any() == true)
                            _context.Lessons.RemoveRange(section.Lessons);
                    }
                    _context.Sections.RemoveRange(course.Sections);
                }

                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Course and all related data deleted.";
            }
            return RedirectToAction(nameof(Courses));
        }

        public async Task<IActionResult> Enrollments()
        {
            var enrollments = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .OrderByDescending(e => e.EnrolledAt)
                .ToListAsync();

            return View(enrollments);
        }

        public async Task<IActionResult> Reviews()
        {
            var reviews = await _context.Reviews
                .Include(r => r.Student)
                .Include(r => r.Course)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return View(reviews);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                var course = await _context.Courses.FindAsync(review.CourseId);
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();

                if (course != null)
                {
                    var remaining = await _context.Reviews.Where(r => r.CourseId == course.Id).ToListAsync();
                    course.ReviewsCount  = remaining.Count;
                    course.AverageRating = remaining.Any() ? remaining.Average(r => r.Rate) : 0;
                    await _context.SaveChangesAsync();
                }

                TempData["Success"] = "Review deleted.";
            }
            return RedirectToAction(nameof(Reviews));
        }
    }
}