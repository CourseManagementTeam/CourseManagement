using CourseManagementSystem.Models;

namespace CourseManagementSystem.Repository
{
    public interface IEnrollmentRepository
    : IRepository<Enrollment>
    {
    }
    //public interface IEnrollmentRepository
    //{
    //    IEnumerable<Enrollment> GetAll();

    //    Enrollment? GetEnrollment(string studentId, int courseId);

    //    void Add(Enrollment enrollment);

    //    void Delete(string studentId, int courseId);

    //    void Save();
    //}
}
