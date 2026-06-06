using CourseManagementSystem.Models;
using CourseManagementSystem.Repository;
using CourseManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CourseManagementSystem.Controllers
{
    public class SectionController : Controller
    {
        private readonly ISectionRepository _sectionRepository;
        private readonly ApplicationDbContext _context;

        public SectionController(ISectionRepository sectionRepository, ApplicationDbContext context)
        {
            _sectionRepository = sectionRepository;
            _context = context;
        }

        private bool OwnsParentCourse(int courseId)
        {
            if (User.IsInRole("Admin")) return true;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return _context.Courses.Any(c => c.Id == courseId && c.InstructorId == userId);
        }

        // GET: Section/Index?courseId=1
        [HttpGet]
        [Authorize(Roles = "Instructor,Admin")]
        public IActionResult Index(int courseId)
        {
            if (!OwnsParentCourse(courseId)) return Forbid();

            var sections = _context.Sections
                .Where(s => s.CourseId == courseId)
                .OrderBy(s => s.OrderNumber)
                .Include(s => s.Lessons!.OrderBy(l => l.OrderNumber))
                .ToList();

            var course = _context.Courses.Find(courseId);
            ViewBag.CourseId    = courseId;
            ViewBag.CourseTitle = course?.Title ?? "Course";

            return View(sections);
        }

        [HttpGet]
        [Authorize(Roles = "Instructor,Admin")]
        public IActionResult Create(int courseId)
        {
            if (!OwnsParentCourse(courseId)) return Forbid();
            ViewBag.CourseId = courseId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Instructor,Admin")]
        public IActionResult Create(SectionViewModel vm)
        {
            if (!OwnsParentCourse(vm.CourseId)) return Forbid();

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
        [Authorize(Roles = "Instructor,Admin")]
        public IActionResult Edit(int id)
        {
            var section = _sectionRepository.GetById(id);
            if (section == null) return NotFound();
            if (!OwnsParentCourse(section.CourseId)) return Forbid();

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
        [Authorize(Roles = "Instructor,Admin")]
        public IActionResult Edit(int id, SectionViewModel vm)
        {
            var section = _sectionRepository.GetById(id);
            if (section == null) return NotFound();
            if (!OwnsParentCourse(section.CourseId)) return Forbid();

            if (!ModelState.IsValid)
                return View(vm);

            section.Title = vm.Title;
            _sectionRepository.Update(section);
            _sectionRepository.Save();

            return RedirectToAction("Index", new { courseId = section.CourseId });
        }

        // GET: Section/Delete/5
        [HttpGet]
        [Authorize(Roles = "Instructor,Admin")]
        public IActionResult Delete(int id)
        {
            var section = _sectionRepository.GetById(id);
            if (section == null) return NotFound();
            if (!OwnsParentCourse(section.CourseId)) return Forbid();

            return View(section);
        }

        // POST: Section/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Instructor,Admin")]
        public IActionResult Delete(int id, Section section)
        {
            var sec = _sectionRepository.GetById(id);
            if (sec == null) return NotFound();
            if (!OwnsParentCourse(sec.CourseId)) return Forbid();

            int courseId = sec.CourseId;
            _sectionRepository.Delete(sec.Id);

            return RedirectToAction("Index", new { courseId });
        }

        // GET: Section/Content?courseId=1
        [HttpGet]
        [Authorize]
        public IActionResult Content(int courseId)
        {
            var sections = _sectionRepository.GetSectionsByCourseId(courseId);

            var firstSection = sections.FirstOrDefault();
            ViewBag.CourseTitle = firstSection?.Course?.Title ?? "Course Curriculum";
            ViewBag.CourseId = courseId;

            return View(sections);
        }
    }
}