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
        public IActionResult Create(SectionViewModel vm)
        {
            _sectionRepository.Add(new Section { CourseId = vm.CourseId, Title = vm.Title });
            return RedirectToAction("Index", new { courseId = vm.CourseId });
        }


    }
}
