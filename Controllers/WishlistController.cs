using CourseManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementSystem.Controllers
{

    [Authorize(Roles = "Student")]
    public class WishlistController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext context;

        public WishlistController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.context = context;
        }

        public async Task<IActionResult> Add(int courseId)
        {
            var userId = userManager.GetUserId(User);

            var course = await context.Courses
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null)
            {
                return NotFound();
            }

            bool exists = await context.Wishlists
                .AnyAsync(w =>
                    w.CourseId == courseId &&
                    w.StudentId == userId);

            if (exists)
            {
                TempData["Warning"] =
                    "Course already exists in wishlist.";

                return RedirectToAction(
                    "Details",
                    "Courses",
                    new { id = courseId });
            }

            Wishlist wishlist = new()
            {
                CourseId = courseId,
                StudentId = userId,
                AddedAt = DateTime.UtcNow
            };

            context.Wishlists.Add(wishlist);

            await context.SaveChangesAsync();

            TempData["Success"] =
                "Course added to wishlist.";

            return RedirectToAction(
                "Details",
                "Course",
                new { id = courseId });
        }

        public async Task<IActionResult> MyWishlist()
        {
            var userId = userManager.GetUserId(User);

            var wishlist = await context.Wishlists
                .Include(w => w.Course)
                .Where(w => w.StudentId == userId)
                .OrderByDescending(w => w.AddedAt)
                .ToListAsync();

            return View(wishlist);
        }

        public async Task<IActionResult> Remove(int courseId)
        {
            var userId = userManager.GetUserId(User);

            var wishlist = await context.Wishlists
                .FirstOrDefaultAsync(w =>
                    w.CourseId == courseId &&
                    w.StudentId == userId);

            if (wishlist == null)
            {
                return NotFound();
            }

            context.Wishlists.Remove(wishlist);

            await context.SaveChangesAsync();

            TempData["Success"] =
                "Course removed from wishlist.";

            return RedirectToAction(nameof(MyWishlist));
        }
    }
}
