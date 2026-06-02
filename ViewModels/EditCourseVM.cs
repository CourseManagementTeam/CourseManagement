using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CourseManagementSystem.ViewModels
{
    public class EditCourseVM
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [StringLength(2000)]
        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        public string? Level { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public string? ExistingImage { get; set; }

        public IFormFile? ImageFile { get; set; }

        public IEnumerable<SelectListItem>? Categories { get; set; }
    }
}
