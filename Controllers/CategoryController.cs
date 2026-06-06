using CourseManagementSystem.Models;
using CourseManagementSystem.Repository;
using CourseManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementSystem.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ApplicationDbContext _context;

        public CategoryController(ICategoryRepository categoryRepository, ApplicationDbContext context)
        {
            _categoryRepository = categoryRepository;
            _context = context;
        }

        public IActionResult Index()
        {
            var categories = _categoryRepository.GetAll();
            return View(categories);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _categoryRepository.Add(category);
                _categoryRepository.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            var category = _categoryRepository.GetById(id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _categoryRepository.Update(category);
                _categoryRepository.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var category = _categoryRepository.GetById(id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteConfirmed(int id)
        {
            _categoryRepository.Delete(id);
            _categoryRepository.Save();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null) return NotFound();

            var courses = await _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Enrollments)
                .Where(c => c.CategoryId == id && c.IsPublished)
                .ToListAsync();

            var totalStudents = courses
                .SelectMany(c => c.Enrollments ?? [])
                .Select(e => e.StudentId)
                .Distinct()
                .Count();

            var ratedCourses = courses.Where(c => c.ReviewsCount > 0).ToList();
            var avgRating = ratedCourses.Any()
                ? ratedCourses.Average(c => c.AverageRating)
                : 0;

            var vm = new CategoryDetailsVM
            {
                Id            = category.Id,
                Name          = category.Name,
                Description   = category.Description,
                TotalCourses  = courses.Count,
                TotalStudents = totalStudents,
                AverageRating = Math.Round(avgRating, 1),
                Courses       = courses.Select(c => new CourseListVM
                {
                    Id             = c.Id,
                    Title          = c.Title,
                    Price          = c.Price,
                    ImageUrl       = c.ImageUrl,
                    CategoryName   = category.Name,
                    InstructorName = c.Instructor?.FullName ?? "Unknown",
                    AverageRating  = c.AverageRating
                }).ToList()
            };

            return View(vm);
        }
    }
}
