using CourseManagementSystem.Models;
using CourseManagementSystem.Repository;
using CourseManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace CourseManagementSystem.Controllers
{
    [Authorize]
    public class CourseController : Controller
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ICartRepository _cartRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly ApplicationDbContext _context;

        public CourseController(
            ApplicationDbContext context,
    ICourseRepository courseRepository,
    ICategoryRepository categoryRepository,
    ICartRepository cartRepository,
    IEnrollmentRepository enrollmentRepository,
    IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _courseRepository = courseRepository;
            _categoryRepository = categoryRepository;
            _cartRepository = cartRepository;
            _enrollmentRepository = enrollmentRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        // 1. شاشة عرض الكورسات العامة (Search & Filter)
        [AllowAnonymous]
        public IActionResult Index(string? search, int? categoryId)
        {
            var courses = _courseRepository.SearchCourses(search, categoryId);

            var courseListVM = courses.Select(c => new CourseListVM
            {
                Id = c.Id,
                Title = c.Title,
                Price = c.Price,
                ImageUrl = c.ImageUrl,
                CategoryName = c.Category?.Name ?? "General",
                InstructorName = c.Instructor?.FullName ?? "Unknown",
                AverageRating = c.AverageRating
            }).ToList();

            ViewBag.Categories = new SelectList(_categoryRepository.GetAll(), "Id", "Name", categoryId);
            ViewBag.Search = search;

            return View(courseListVM);
        }

        // 2. كورسات الـ Instructor الحالي فقط (مطلوبة بالتكليف)
        [Authorize(Roles = "Instructor")]
        public IActionResult InstructorCourses()
        {
            // بمجرد تفعيل اللوجن، السطر القادم سيسحب الـ ID الحقيقي للمستخدم الحالي
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var courses = _courseRepository.GetCoursesByInstructor(instructorId);
            return View(courses);
        }

        // 3. تفاصيل الكورس بالتصميم الفخم
        [AllowAnonymous]
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
                InstructorName = course.Instructor?.FullName ?? "Expert Instructor",
                AverageRating = course.AverageRating,
                ReviewsCount = course.ReviewsCount,
                SectionsCount = course.Sections?.Count ?? 0,
                LessonsCount = course.Sections?.Sum(s => s.Lessons?.Count ?? 0) ?? 0
            };

            return View(vm);
        }

        // 4. إنشاء كورس جديد (GET)
        [Authorize(Roles = "Instructor")]
        [HttpGet]
        public IActionResult Create()
        {
            var vm = new CreateCourseVM
            {
                Categories = _categoryRepository.GetAll().Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
            };

            return View(vm);
        }

        // 5. إنشاء كورس جديد (POST)
        [Authorize(Roles = "Instructor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateCourseVM vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Categories = _categoryRepository.GetAll().Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                });
                return View(vm);
            }

            string? fileName = null;
            if (vm.ImageFile != null)
            {
                fileName = UploadImage(vm.ImageFile);
            }

            var instructorId =
  User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            Course course = new Course
            {
                Title = vm.Title,
                Description = vm.Description,
                Price = vm.Price,
                Level = vm.Level,
                CategoryId = vm.CategoryId,
                ImageUrl = fileName,
                InstructorId = instructorId
            };

            _courseRepository.Add(course);
            _courseRepository.Save();

            TempData["Success"] = "Course created successfully!";
            return RedirectToAction(nameof(Index));
        }

        // 6. تعديل كورس (GET)
        [Authorize(Roles = "Instructor")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var course = _courseRepository.GetById(id);
            if (course == null) return NotFound();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (course.InstructorId != userId)
            {
                return Forbid();
            }

            var vm = new EditCourseVM
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                Price = course.Price,
                Level = course.Level,
                CategoryId = course.CategoryId,
                ExistingImage = course.ImageUrl,
                Categories = _categoryRepository.GetAll().Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                    Selected = c.Id == course.CategoryId
                })
            };

            return View(vm);
        }

        // 7. تعديل كورس (POST)
        [Authorize(Roles = "Instructor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EditCourseVM vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Categories = _categoryRepository.GetAll().Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                });
                return View(vm);
            }

            var course = _courseRepository.GetById(vm.Id);
            if (course == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (course.InstructorId != userId)
            {
                return Forbid();
            }

            if (vm.ImageFile != null)
            {
                // مسح الصورة القديمة إذا وُجدت لمنع تراكم الملفات
                DeleteImage(course.ImageUrl);
                course.ImageUrl = UploadImage(vm.ImageFile);
            }

            course.Title = vm.Title;
            course.Description = vm.Description;
            course.Price = vm.Price;
            course.Level = vm.Level;
            course.CategoryId = vm.CategoryId;

            _courseRepository.Update(course);
            _courseRepository.Save();

            TempData["Success"] = "Course updated successfully!";
            return RedirectToAction(nameof(InstructorCourses));
        }

        // 8. حذف كورس (GET)
        [Authorize(Roles = "Instructor")]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var course = _courseRepository.GetCourseDetails(id);
            if (course == null) return NotFound();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (course.InstructorId != userId)
            {
                return Forbid();
            }
            return View(course);
        }

        // 9. تأكيد الحذف (POST)
        [Authorize(Roles = "Instructor")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var course = _courseRepository.GetById(id);

            if (course == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (course.InstructorId != userId)
            {
                return Forbid();
            }

            DeleteImage(course.ImageUrl);

            _courseRepository.Delete(id);
            _courseRepository.Save();

            TempData["Success"] = "Course deleted successfully!";

            return RedirectToAction(nameof(InstructorCourses));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToWishlist(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "Please login first";
                return RedirectToAction("Login", "Account");
            }

            bool exists = _context.Wishlists.Any(w =>
                w.StudentId == userId &&
                w.CourseId == id);

            if (!exists)
            {
                _context.Wishlists.Add(new Wishlist
                {
                    StudentId = userId,
                    CourseId = id,
                    AddedAt = DateTime.Now
                });

                _context.SaveChanges();
            }

            TempData["Success"] = "Course added to wishlist";

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BuyNow(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "Please login first";
                return RedirectToAction("Login", "Account");
            }

            bool exists = _context.Enrollments.Any(e =>
                e.StudentId == userId &&
                e.CourseId == id);

            if (!exists)
            {
                _context.Enrollments.Add(new Enrollment
                {
                    StudentId = userId,
                    CourseId = id,
                    EnrolledAt = DateTime.Now
                });

                _context.SaveChanges();
            }

            TempData["Success"] = "Course purchased successfully";

            return RedirectToAction(nameof(Details), new { id });
        }

      
        // --- ميثودز مساعدة لرفع وحذف الصور من السيرفر ---
        private string UploadImage(IFormFile file)
        {
           
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError("", "Invalid file type.");
            }

           
            if (file.Length > 5 * 1024 * 1024)
            {
                ModelState.AddModelError("", "Invalid file type.");
            }
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "courses");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            return fileName;
        }

        private void DeleteImage(string? fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return;
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "courses", fileName);
            if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
        }


    }
}

//using CourseManagementSystem.Models;
//using CourseManagementSystem.Repository;
//using CourseManagementSystem.ViewModels;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;

//namespace CourseManagementSystem.Controllers
//{
//    public class CourseController : Controller
//    {
//        private readonly ICourseRepository _courseRepository;
//        private readonly ICategoryRepository _categoryRepository;
//        private readonly IWebHostEnvironment _webHostEnvironment;

//        public CourseController(
//            ICourseRepository courseRepository,
//            ICategoryRepository categoryRepository,
//            IWebHostEnvironment webHostEnvironment)
//        {
//            _courseRepository = courseRepository;
//            _categoryRepository = categoryRepository;
//            _webHostEnvironment = webHostEnvironment;
//        }
//        public IActionResult Index(string? search, int? categoryId)
//        {
//            var courses = _courseRepository.SearchCourses(search, categoryId);

//            ViewBag.Categories = new SelectList(
//                _categoryRepository.GetAll(),
//                "Id",
//                "Name");

//            return View(courses);
//        }
//        public IActionResult Details(int id)
//        {
//            var course = _courseRepository.GetCourseDetails(id);

//            if (course == null)
//                return NotFound();

//            var vm = new CourseDetailsVM
//            {
//                Id = course.Id,
//                Title = course.Title,
//                Description = course.Description,
//                Price = course.Price,
//                ImageUrl = course.ImageUrl,
//                CategoryName = course.Category?.Name,
//                InstructorName = course.Instructor?.FullName,
//                AverageRating = course.AverageRating,
//                ReviewsCount = course.ReviewsCount,
//                SectionsCount = course.Sections?.Count ?? 0,
//                LessonsCount = course.Sections?
//                    .Sum(s => s.Lessons?.Count ?? 0) ?? 0
//            };

//            return View(vm);
//        }
//        [Authorize]
//        [HttpGet]
//        public IActionResult Create()
//        {
//            var vm = new CreateCourseVM
//            {
//                Categories = _categoryRepository.GetAll()
//                .Select(c => new SelectListItem
//                {
//                    Value = c.Id.ToString(),
//                    Text = c.Name
//                })
//            };

//            return View(vm);
//        }
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public IActionResult Create(CreateCourseVM vm)
//        {
//            if (!ModelState.IsValid)
//            {
//                vm.Categories = _categoryRepository.GetAll()
//                    .Select(c => new SelectListItem
//                    {
//                        Value = c.Id.ToString(),
//                        Text = c.Name
//                    });

//                return View(vm);
//            }

//            string? fileName = null;

//            if (vm.ImageFile != null)
//            {
//                string uploadsFolder = Path.Combine(
//                    _webHostEnvironment.WebRootPath,
//                    "images",
//                    "courses");

//                fileName = Guid.NewGuid().ToString()
//                           + Path.GetExtension(vm.ImageFile.FileName);

//                string filePath = Path.Combine(uploadsFolder, fileName);

//                using (var stream = new FileStream(filePath, FileMode.Create))
//                {
//                    vm.ImageFile.CopyTo(stream);
//                }
//            }

//            Course course = new Course
//            {
//                Title = vm.Title,
//                Description = vm.Description,
//                Price = vm.Price,
//                Level = vm.Level,
//                CategoryId = vm.CategoryId,
//                ImageUrl = fileName,

//                // مؤقتًا لحد ما Login يخلص
//                InstructorId = "TEMP"
//            };

//            _courseRepository.Add(course);
//            _courseRepository.Save();

//            return RedirectToAction(nameof(Index));
//        }
//    }
//}
