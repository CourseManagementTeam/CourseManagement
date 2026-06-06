using CourseManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementSystem.Controllers
{
    [Authorize(Roles = "Student")]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var items = await _context.CartItems
                .Include(c => c.Course)
                    .ThenInclude(c => c.Instructor)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            return View(items);
        }

        public async Task<IActionResult> Add(int courseId)
        {
            var userId = _userManager.GetUserId(User);

            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);
            if (course == null)
                return NotFound();

            bool exists = await _context.CartItems
                .AnyAsync(c => c.UserId == userId && c.CourseId == courseId);

            if (!exists)
            {
                _context.CartItems.Add(new CartItem { UserId = userId!, CourseId = courseId });
                await _context.SaveChangesAsync();
                TempData["Success"] = "Course added to cart.";
            }
            else
            {
                TempData["Warning"] = "Course is already in your cart.";
            }

            return RedirectToAction("Details", "Course", new { id = courseId });
        }

        public async Task<IActionResult> Remove(int courseId)
        {
            var userId = _userManager.GetUserId(User);

            var item = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.CourseId == courseId);

            if (item != null)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Course removed from cart.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout()
        {
            var userId = _userManager.GetUserId(User);

            var cartItems = await _context.CartItems
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["Warning"] = "Your cart is empty.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var item in cartItems)
            {
                bool alreadyEnrolled = await _context.Enrollments
                    .AnyAsync(e => e.StudentId == userId && e.CourseId == item.CourseId);

                if (!alreadyEnrolled)
                {
                    _context.Enrollments.Add(new Enrollment
                    {
                        StudentId = userId!,
                        CourseId = item.CourseId,
                        EnrolledAt = DateTime.Now,
                        ProgressPercentage = 0,
                        IsCompleted = false
                    });
                }
            }

            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Successfully enrolled in {cartItems.Count} course(s)!";
            return RedirectToAction("MyCourses", "Enrollment");
        }
    }
}