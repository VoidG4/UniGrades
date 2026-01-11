using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Models;
using System.Linq;

namespace StudentPortal.Controllers
{
    public class StudentController : Controller
    {
        private readonly UniversityDbContext _context;

        public StudentController(UniversityDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int id)
        {
            ViewBag.CurrentPage = "Home";

            var student = _context.Students.FirstOrDefault(s => s.RegistrationNumber == id);

            string studentName = student != null ? $"{student.Name} {student.Surname}" : "Φοιτητή";
            ViewBag.StudentName = studentName;

            var allGrades = _context.CourseHasStudents
                                   .Include(c => c.CourseIdCourseNavigation)
                                   .Where(s => s.StudentsRegistrationNumber == id)
                                   .ToList();

            
            var gradedCourses = allGrades.Where(g => g.GradeCourseStudent != null).ToList();

            double average = gradedCourses.Any()
                ? gradedCourses.Average(g => g.GradeCourseStudent.Value)
                : 0;

            int passedCount = gradedCourses.Count(g => g.GradeCourseStudent >= 5);

            
            int totalCourses = allGrades.Count;

            var recentGrades = gradedCourses.Take(5).ToList();

            ViewBag.StudentId = id;
            ViewBag.Average = average.ToString("0.00");
            ViewBag.PassedCount = passedCount;
            ViewBag.TotalCourses = totalCourses; 
            ViewBag.Semester = student?.Semester ?? 1; 

            return View(recentGrades);
        }

        public IActionResult GradesByCourse(int id)
        {
            ViewBag.StudentId = id;
            ViewBag.CurrentPage = "GradesByCourse";

            var student = _context.Students.FirstOrDefault(s => s.RegistrationNumber == id);
            ViewBag.StudentName = student != null ? $"{student.Name} {student.Surname}" : "Φοιτητή";

            var grades = _context.CourseHasStudents
                           .Include(c => c.CourseIdCourseNavigation)
                           .Where(s => s.StudentsRegistrationNumber == id)
                           .OrderBy(g => g.CourseIdCourseNavigation.CourseTitle)
                           .ToList();

            return View(grades);
        }

        public IActionResult GradesBySemester(int id)
        {
            ViewBag.StudentId = id;
            ViewBag.CurrentPage = "GradesBySemester";

            var student = _context.Students.FirstOrDefault(s => s.RegistrationNumber == id);
            ViewBag.StudentName = student != null ? $"{student.Name} {student.Surname}" : "Φοιτητή";

            var grades = _context.CourseHasStudents
                           .Include(c => c.CourseIdCourseNavigation)
                           .Where(s => s.StudentsRegistrationNumber == id)
                           .OrderBy(g => g.CourseIdCourseNavigation.CourseSemester)
                           .ToList();

            return View(grades);
        }

        public IActionResult TotalGrades(int id)
        {
            ViewBag.StudentId = id;
            ViewBag.CurrentPage = "TotalGrades";

            var student = _context.Students.FirstOrDefault(s => s.RegistrationNumber == id);
            ViewBag.StudentName = student != null ? $"{student.Name} {student.Surname}" : "Φοιτητή";

            var allGrades = _context.CourseHasStudents
                           .Include(c => c.CourseIdCourseNavigation)
                           .Where(s => s.StudentsRegistrationNumber == id)
                           .OrderByDescending(g => g.GradeCourseStudent)
                           .ToList();

            return View(allGrades);
        }
    }
}