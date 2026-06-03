using CourseManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementSystem.Controllers
{
    public class EnrollmentController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext context;

        public EnrollmentController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.context = context;
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Enroll(int courseId)
        {
            var course = await context.Courses
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null)
            {
                return NotFound();
            }

            var userId = userManager.GetUserId(User);

            bool alreadyEnrolled =
                await context.Enrollments
                .AnyAsync(e =>
                    e.CourseId == courseId &&
                    e.StudentId == userId);

            if (alreadyEnrolled)
            {
                TempData["Warning"] =
                    "You are already enrolled in this course.";

                return RedirectToAction(
                    "Details",
                    "Course",
                    new { id = courseId });
            }

            Enrollment enrollment = new()
            {
                CourseId = courseId,
                StudentId = userId,
                ProgressPercentage = 0,
                IsCompleted = false
            };

            context.Enrollments.Add(enrollment);

            await context.SaveChangesAsync();

            TempData["Success"] =
                "Course enrolled successfully.";

            return RedirectToAction(nameof(MyCourses));
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> MyCourses()
        {
            var userId = userManager.GetUserId(User);

            var enrollments = await context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.StudentId == userId)
                .ToListAsync();

            return View(enrollments);
        }
    }
}
