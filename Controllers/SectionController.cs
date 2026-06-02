using CourseManagementSystem.Repository;
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
        public IActionResult Index(int courseId)
        {
            var sections = _sectionRepository.GetSectionsByCourseId(courseId);
            return View(sections);
        }

       

    }
}
