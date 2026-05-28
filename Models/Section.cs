using System.ComponentModel.DataAnnotations;

namespace CourseManagementSystem.Models
{
    public class Section
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public int OrderNumber { get; set; }

        //*********** Foreign Key ***********
        public int CourseId { get; set; }

        //********************** Navigation Properties ***********************
        public Course? Course { get; set; }

        public ICollection<Lesson>? Lessons { get; set; }
    }
}
