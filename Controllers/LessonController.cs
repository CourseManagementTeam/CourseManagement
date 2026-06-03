using Microsoft.AspNetCore.Mvc;

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

    }
}
