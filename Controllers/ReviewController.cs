using CourseManagementSystem.Models;
using CourseManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementSystem.Controllers
{
    [Authorize(Roles = "Student")]
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public ReviewController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int courseId)
        {
            var course = await context.Courses
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null)
            {
                return NotFound();
            }

            var userId = userManager.GetUserId(User);

            bool enrolled = await context.Enrollments
                .AnyAsync(e =>
                    e.CourseId == courseId &&
                    e.StudentId == userId);

            if (!enrolled)
            {
                return Forbid();
            }

            bool reviewExists = await context.Reviews
                .AnyAsync(r =>
                    r.CourseId == courseId &&
                    r.StudentId == userId);

            if (reviewExists)
            {
                TempData["Warning"] =
                    "You already reviewed this course.";

                return RedirectToAction(
                    "Details",
                    "Course",
                    new { id = courseId });
            }

            ReviewVM vm = new()
            {
                CourseId = courseId
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReviewVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var userId = userManager.GetUserId(User);

            bool enrolled = await context.Enrollments
                .AnyAsync(e =>
                    e.CourseId == vm.CourseId &&
                    e.StudentId == userId);

            if (!enrolled)
            {
                return Forbid();
            }

            bool reviewExists = await context.Reviews
                .AnyAsync(r =>
                    r.CourseId == vm.CourseId &&
                    r.StudentId == userId);

            if (reviewExists)
            {
                TempData["Warning"] =
                    "You already reviewed this course.";

                return RedirectToAction(
                    "Details",
                    "Course",
                    new { id = vm.CourseId });
            }

            Review review = new()
            {
                CourseId  = vm.CourseId,
                StudentId = userId,
                Comment   = vm.Comment,
                Rate      = vm.Rating,
                CreatedAt = DateTime.Now
            };

            context.Reviews.Add(review);
            await context.SaveChangesAsync();

            var course = await context.Courses.FindAsync(vm.CourseId);
            if (course != null)
            {
                var allRates = await context.Reviews
                    .Where(r => r.CourseId == vm.CourseId)
                    .Select(r => r.Rate)
                    .ToListAsync();

                course.ReviewsCount  = allRates.Count;
                course.AverageRating = allRates.Any() ? Math.Round(allRates.Average(), 1) : 0;
                await context.SaveChangesAsync();
            }

            TempData["Success"] = "Your review has been submitted!";

            return RedirectToAction("Details", "Course", new { id = vm.CourseId });
        }

        public async Task<IActionResult> MyReviews()
        {
            var userId = userManager.GetUserId(User);

            var reviews = await context.Reviews
                .Include(r => r.Course)
                .Where(r => r.StudentId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return View(reviews);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var userId = userManager.GetUserId(User);

            var review = await context.Reviews
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null)
            {
                return NotFound();
            }

            if (review.StudentId != userId)
            {
                return Forbid();
            }

            context.Reviews.Remove(review);

            await context.SaveChangesAsync();

            TempData["Success"] =
                "Review deleted successfully.";

            return RedirectToAction(nameof(MyReviews));
        }
    }
}
