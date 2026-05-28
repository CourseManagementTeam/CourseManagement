using System.ComponentModel.DataAnnotations;

namespace CourseManagementSystem.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        //********************** Navigation Property ***********************
        public ICollection<Course>? Courses { get; set; }
    }
 
}

