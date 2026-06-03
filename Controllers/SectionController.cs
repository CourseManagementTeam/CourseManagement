using CourseManagementSystem.Models;
using CourseManagementSystem.Repository;
using CourseManagementSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagementSystem.Controllers
{
    public class SectionController : Controller
    {
        private readonly ISectionRepository _sectionRepository;

        public SectionController(ISectionRepository sectionRepository)
        {
            _sectionRepository = sectionRepository;
        }

        // GET: Section/Index?courseId=1
        [HttpGet]
        public IActionResult Index(int courseId)
        {
            var sections = _sectionRepository.GetSectionsByCourseId(courseId);
            return View(sections);
        }
        [HttpGet]
        public IActionResult Create(int courseId)
        {
            ViewBag.CourseId = courseId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SectionViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            _sectionRepository.Add(new Section
            {
                CourseId = vm.CourseId,
                Title = vm.Title
            });

            _sectionRepository.Save();

            return RedirectToAction("Index", new { courseId = vm.CourseId });
        }

        // GET: Section/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var section = _sectionRepository.GetById(id);

            if (section == null)
                return NotFound();

            var vm = new SectionViewModel
            {
                CourseId = section.CourseId,
                Title = section.Title
            };

            return View(vm);
        }

        // POST: Section/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, SectionViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var section = _sectionRepository.GetById(id);

            if (section == null)
                return NotFound();

            section.Title = vm.Title;

            _sectionRepository.Update(section);
            _sectionRepository.Save();

            return RedirectToAction("Index", new { courseId = section.CourseId });
        }

        
        // GET: Section/Delete/5
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var section = _sectionRepository.GetById(id);

            if (section == null)
                return NotFound();

            return View(section);
        }

        // POST: Section/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id, Section section)
        {
            var sec = _sectionRepository.GetById(id);

            if (sec == null)
                return NotFound();

            int courseId = sec.CourseId;

            _sectionRepository.Delete(sec.Id);

            return RedirectToAction("Index", new { courseId });
        }
    }
}
