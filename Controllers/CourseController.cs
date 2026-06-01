using CourseManagementSystem.Models;
using CourseManagementSystem.Repository;
using CourseManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CourseManagementSystem.Controllers
{
    public class CourseController : Controller
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CourseController(
            ICourseRepository courseRepository,
            ICategoryRepository categoryRepository,
            IWebHostEnvironment webHostEnvironment)
        {
            _courseRepository = courseRepository;
            _categoryRepository = categoryRepository;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index(string? search, int? categoryId)
        {
            var courses = _courseRepository.SearchCourses(search, categoryId);

            ViewBag.Categories = new SelectList(
                _categoryRepository.GetAll(),
                "Id",
                "Name");

            return View(courses);
        }
        public IActionResult Details(int id)
        {
            var course = _courseRepository.GetCourseDetails(id);

            if (course == null)
                return NotFound();

            var vm = new CourseDetailsVM
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                Price = course.Price,
                ImageUrl = course.ImageUrl,
                CategoryName = course.Category?.Name,
                InstructorName = course.Instructor?.FullName,
                AverageRating = course.AverageRating,
                ReviewsCount = course.ReviewsCount,
                SectionsCount = course.Sections?.Count ?? 0,
                LessonsCount = course.Sections?
                    .Sum(s => s.Lessons?.Count ?? 0) ?? 0
            };

            return View(vm);
        }
        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            var vm = new CreateCourseVM
            {
                Categories = _categoryRepository.GetAll()
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
            };

            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateCourseVM vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Categories = _categoryRepository.GetAll()
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    });

                return View(vm);
            }

            string? fileName = null;

            if (vm.ImageFile != null)
            {
                string uploadsFolder = Path.Combine(
                    _webHostEnvironment.WebRootPath,
                    "images",
                    "courses");

                fileName = Guid.NewGuid().ToString()
                           + Path.GetExtension(vm.ImageFile.FileName);

                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    vm.ImageFile.CopyTo(stream);
                }
            }

            Course course = new Course
            {
                Title = vm.Title,
                Description = vm.Description,
                Price = vm.Price,
                Level = vm.Level,
                CategoryId = vm.CategoryId,
                ImageUrl = fileName,

                // مؤقتًا لحد ما Login يخلص
                InstructorId = "TEMP"
            };

            _courseRepository.Add(course);
            _courseRepository.Save();

            return RedirectToAction(nameof(Index));
        }
    }
}
