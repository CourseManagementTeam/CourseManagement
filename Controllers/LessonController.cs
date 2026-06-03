using CourseManagementSystem.Models;
using CourseManagementSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementSystem.Controllers
{
    public class LessonController : Controller
    {
        private readonly ILessonRepository _lessonRepository;

        public LessonController(ILessonRepository lessonRepository)
        {
            _lessonRepository = lessonRepository;
        }

        // GET: Lesson/Index?sectionId=1
        [HttpGet]
        public IActionResult Index(int sectionId)
        {
            ViewBag.SectionId = sectionId;

            var lessons = _lessonRepository.GetLessonsBySectionId(sectionId);
            return View(lessons);
        }

        // GET: Lesson/Create?sectionId=1
        [HttpGet]
        public IActionResult Create(int sectionId)
        {
            var vm = new LessonViewModel
            {
                SectionId = sectionId
            };
            return View(vm);
        }

        // POST: Lesson/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(LessonViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            try
            {
                var newLesson = new Lesson
                {
                    Title = vm.Title,
                    SectionId = vm.SectionId
                };

                _lessonRepository.Add(newLesson);
                _lessonRepository.Save();

                return RedirectToAction(nameof(Index), new { sectionId = vm.SectionId });
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Unable to save. The parent section could not be found.");
                return View(vm);
            }
        }
    }
}
