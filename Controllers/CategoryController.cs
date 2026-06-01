using CourseManagementSystem.Models;
using CourseManagementSystem.Repository;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagementSystem.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public IActionResult Index()
        {
            var categories = _categoryRepository.GetAll();
            return View(categories);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
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
        public IActionResult Edit(int id)
        {
            var category = _categoryRepository.GetById(id);

            if (category == null)
                return NotFound();

            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
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
        public IActionResult Delete(int id)
        {
            var category = _categoryRepository.GetById(id);

            if (category == null)
                return NotFound();

            return View(category);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _categoryRepository.Delete(id);
            _categoryRepository.Save();

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Details(int id)
        {
            var category = _categoryRepository.GetById(id);

            if (category == null)
                return NotFound();

            return View(category);
        }
    }
}
