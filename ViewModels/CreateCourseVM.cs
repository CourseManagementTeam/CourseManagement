using CourseManagementSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CourseManagementSystem.ViewModels
{
    public class CreateCourseVM
    {
      
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

        [DataType(DataType.Upload)]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png", ".webp" }, ErrorMessage = "Only .jpg, .jpeg, .png, and .webp images are allowed!")]
        [MaxFileSize(5 * 1024 * 1024, ErrorMessage = "Maximum allowed file size is 5 MB!")]
        public IFormFile? ImageFile { get; set; }

            public IEnumerable<SelectListItem>? Categories { get; set; }
        }
    }

