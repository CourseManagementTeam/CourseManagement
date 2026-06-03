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
                    SectionId = vm.SectionId,
                    VideoUrl = vm.VideoUrl,
                    IsFreePreview = vm.IsFreePreview 
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

        // GET: Lesson/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var lesson = _lessonRepository.GetById(id);

            if (lesson == null)
                return NotFound();

            var vm = new LessonViewModel
            {
                Id = lesson.Id,
                Title = lesson.Title,
                SectionId = lesson.SectionId
            };

            return View(vm);
        }

        // Lesson/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, LessonViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var lesson = _lessonRepository.GetById(id);

            if (lesson == null)
                return NotFound();

            lesson.Title = vm.Title;
            lesson.VideoUrl = vm.VideoUrl;
            lesson.IsFreePreview = vm.IsFreePreview;

            _lessonRepository.Update(lesson);
            _lessonRepository.Save();

            return RedirectToAction(nameof(Index), new { sectionId = lesson.SectionId });
        }

        // GET: Lesson/Delete/5
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var lesson = _lessonRepository.GetById(id);

            if (lesson == null)
                return NotFound();

            return View(lesson); 
        }

        // POST: Lesson/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id, Lesson lessonFromForm)
        {
            var lesson = _lessonRepository.GetById(id);

            if (lesson == null)
                return NotFound();

            int savedSectionId = lesson.SectionId; 

            _lessonRepository.Delete(id);
            _lessonRepository.Save();

            return RedirectToAction(nameof(Index), new { sectionId = savedSectionId });
        }
    }
}
