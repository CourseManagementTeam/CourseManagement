using CourseManagementSystem.Models;
using CourseManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementSystem.Controllers
{
    [Authorize(Roles = "Student")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public DashboardController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = userManager.GetUserId(User);

            var enrollments = await context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.StudentId == userId)
                .OrderByDescending(e => e.EnrolledAt)
                .ToListAsync();

            var wishlistCourses = await context.Wishlists
                .Include(w => w.Course)
                .Where(w => w.StudentId == userId)
                .OrderByDescending(w => w.AddedAt)
                .Select(w => w.Course)
                .ToListAsync();

            var recentReviews = await context.Reviews
                .Include(r => r.Course)
                .Where(r => r.StudentId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .Take(5)
                .ToListAsync();

            StudentDashboardVM vm = new()
            {
                TotalCourses = enrollments.Count,

                CompletedCourses = enrollments.Count(e => e.IsCompleted),

                WishlistCount = wishlistCourses.Count,

                ReviewsCount = recentReviews.Count,

                Enrollments = enrollments,

                WishlistCourses = wishlistCourses,

                RecentReviews = recentReviews
            };

            return View(vm);
        }
    }
}
