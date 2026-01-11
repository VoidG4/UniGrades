using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Models;
using System.Linq;

namespace StudentPortal.Controllers
{
    public class ProfessorController : Controller
    {
        private readonly UniversityDbContext _context;

        public ProfessorController(UniversityDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int id)
        {
            ViewBag.CurrentPage = "Home";

            var professor = _context.Professors.FirstOrDefault(p => p.Afm == id);
            if (professor == null) return NotFound();

            string profName = $"{professor.Name} {professor.Surname}";
            ViewBag.ProfessorName = profName;
            ViewBag.ProfessorId = id;

            var myCourses = _context.Courses.Where(c => c.ProfessorsAfm == id).ToList();
            var myCourseIds = myCourses.Select(c => c.IdCourse).ToList();

            var allMyStudentRecords = _context.CourseHasStudents
                                            .Where(chs => myCourseIds.Contains(chs.CourseIdCourse))
                                            .Include(c => c.CourseIdCourseNavigation)
                                            .Include(s => s.StudentsRegistrationNumberNavigation)
                                            .ToList();

            ViewBag.CoursesCount = myCourses.Count;
            ViewBag.UniqueStudents = allMyStudentRecords.Select(x => x.StudentsRegistrationNumber).Distinct().Count();
            ViewBag.TotalGraded = allMyStudentRecords.Count(x => x.GradeCourseStudent != null);
            ViewBag.PendingGrades = allMyStudentRecords.Count(x => x.GradeCourseStudent == null);

            var recentGrades = allMyStudentRecords.Where(x => x.GradeCourseStudent != null).Take(5).ToList();

            return View(recentGrades);
        }

        public IActionResult MyCourses(int id, string mode = "view")
        {
            ViewBag.CurrentPage = mode == "enter" ? "MyCourses_Enter" : "MyCourses_View";
            ViewBag.CurrentMode = mode; 

            ViewBag.ProfessorId = id;
            var professor = _context.Professors.FirstOrDefault(p => p.Afm == id);
            ViewBag.ProfessorName = professor != null ? $"{professor.Name} {professor.Surname}" : "Καθηγητής";

            if (mode == "enter")
            {
                ViewBag.PageTitle = "Καταχώρηση - Επιλογή Μαθήματος";
                ViewBag.PageDesc = "Επιλέξτε μάθημα για να περάσετε βαθμούς";
                ViewBag.ButtonText = "Βαθμολόγηση";
                ViewBag.ButtonIcon = "bi-pencil-square";
                ViewBag.ButtonColor = "btn-outline-success"; 
            }
            else
            {
                ViewBag.PageTitle = "Προβολή - Επιλογή Μαθήματος";
                ViewBag.PageDesc = "Επιλέξτε μάθημα για να δείτε το ιστορικό";
                ViewBag.ButtonText = "Προβολή";
                ViewBag.ButtonIcon = "bi-eye";
                ViewBag.ButtonColor = "btn-outline-primary"; 
            }

            var myCourses = _context.Courses.Where(c => c.ProfessorsAfm == id).ToList();

            return View(myCourses);
        }

        public IActionResult CourseDetails(int courseId, int professorId, string mode = "view")
        {
            ViewBag.CurrentPage = mode == "enter" ? "MyCourses_Enter" : "MyCourses_View";
            ViewBag.CurrentMode = mode;
            ViewBag.ProfessorId = professorId;

            var course = _context.Courses.FirstOrDefault(c => c.IdCourse == courseId);
            if (course == null) return NotFound();

            ViewBag.CourseTitle = course.CourseTitle;
            ViewBag.CourseId = courseId;

            var professor = _context.Professors.FirstOrDefault(p => p.Afm == professorId);
            ViewBag.ProfessorName = professor != null ? $"{professor.Name} {professor.Surname}" : "Καθηγητής";

            var query = _context.CourseHasStudents
                                .Include(s => s.StudentsRegistrationNumberNavigation)
                                .Where(c => c.CourseIdCourse == courseId);

            if (mode == "view")
            {
                query = query.Where(s => s.GradeCourseStudent != null);
                ViewBag.TableTitle = "Βαθμολογημένοι Φοιτητές";
                ViewBag.NoDataMessage = "Δεν υπάρχουν καταχωρημένες βαθμολογίες ακόμα.";
            }
            else 
            {
                query = query.Where(s => s.GradeCourseStudent == null);
                ViewBag.TableTitle = "Εκκρεμείς Βαθμολογίες";
                ViewBag.NoDataMessage = "Μπράβο! Δεν υπάρχουν εκκρεμότητες. Έχουν βαθμολογηθεί όλοι.";
            }

            var studentsList = query.OrderBy(s => s.StudentsRegistrationNumberNavigation.Surname).ToList();

            return View(studentsList);
        }

        [HttpPost]
        public IActionResult UpdateGrade(int courseId, int studentId, int professorId, int? grade, string mode) 
        {
            var record = _context.CourseHasStudents
                             .FirstOrDefault(r => r.CourseIdCourse == courseId && r.StudentsRegistrationNumber == studentId);

            if (record != null)
            {
                record.GradeCourseStudent = grade;
                _context.SaveChanges();
            }

            return RedirectToAction("CourseDetails", new
            {
                courseId = courseId,
                professorId = professorId,
                mode = mode 
            });
        }
    }
}