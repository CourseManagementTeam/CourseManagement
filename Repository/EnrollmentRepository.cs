using CourseManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementSystem.Repository
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly ApplicationDbContext _context;

        public EnrollmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Enrollment> GetAll()
        {
            return _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .ToList();
        }

        public Enrollment? GetEnrollment(string studentId, int courseId)
        {
            return _context.Enrollments
                .FirstOrDefault(e => e.StudentId == studentId
                                  && e.CourseId == courseId);
        }

        public void Add(Enrollment enrollment)
        {
            _context.Enrollments.Add(enrollment);
        }

        public void Delete(string studentId, int courseId)
        {
            var enrollment = GetEnrollment(studentId, courseId);

            if (enrollment != null)
            {
                _context.Enrollments.Remove(enrollment);
            }
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
