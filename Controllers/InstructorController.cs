using CourseManagementSystem.Models;
using CourseManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CourseManagementSystem.Controllers
{
    [Authorize(Roles = "Instructor")]
    public class InstructorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InstructorController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var courses = await _context.Courses
                .Where(c => c.InstructorId == instructorId)
                .Include(c => c.Enrollments)
                .Include(c => c.Reviews)
                .ToListAsync();

            var totalStudents = courses.SelectMany(c => c.Enrollments ?? []).Select(e => e.StudentId).Distinct().Count();
            var totalReviews  = courses.Sum(c => c.ReviewsCount);
            var avgRating     = courses.Any(c => c.ReviewsCount > 0)
                ? courses.Where(c => c.ReviewsCount > 0).Average(c => c.AverageRating)
                : 0;

            var vm = new InstructorDashboardVM
            {
                TotalCourses  = courses.Count,
                TotalStudents = totalStudents,
                TotalReviews  = totalReviews,
                AverageRating = Math.Round(avgRating, 1),
                Courses       = courses
            };

            return View(vm);
        }

        public async Task<IActionResult> MyStudents()
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var enrollments = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Where(e => e.Course!.InstructorId == instructorId)
                .OrderByDescending(e => e.EnrolledAt)
                .ToListAsync();

            var vm = enrollments.Select(e => new InstructorStudentVM
            {
                StudentId          = e.StudentId,
                FullName           = e.Student?.FullName ?? "Unknown",
                Email              = e.Student?.Email ?? string.Empty,
                CourseName         = e.Course?.Title ?? "Unknown",
                EnrolledAt         = e.EnrolledAt,
                ProgressPercentage = e.ProgressPercentage
            }).ToList();

            return View(vm);
        }
    }
}