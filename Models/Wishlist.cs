namespace CourseManagementSystem.Models
{
    public class Wishlist
    {
        // ************** Composite Key *******************
        public string StudentId { get; set; }

        public int CourseId { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.Now;

        //********************** Navigation Properties ***********************
        public ApplicationUser? Student { get; set; }

        public Course? Course { get; set; }
    }
}
