using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Collections.Specialized.BitVector32;

namespace CourseManagementSystem.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }
        //public string? VideoUrl { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        //********************** Calculated Properties ***********************
        public double AverageRating { get; set; } = 0;

        public int ReviewsCount { get; set; } = 0;

        public int TotalHours { get; set; }

        public bool IsPublished { get; set; } = false;

        public string? Level { get; set; }

        //******** Foreign Keys ****************
        public string InstructorId { get; set; }

        public int CategoryId { get; set; }

        //********************** Navigation Properties ***********************
        public ApplicationUser? Instructor { get; set; }

        public Category? Category { get; set; }
        public ICollection<Enrollment>? Enrollments { get; set; }

        public ICollection<Review>? Reviews { get; set; }

        //**
        public ICollection<Section>? Sections { get; set; }
        public ICollection<Wishlist>? Wishlists { get; set; }

    }
}
