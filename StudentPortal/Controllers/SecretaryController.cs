using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Models;
using System.Linq;

namespace StudentPortal.Controllers
{
    public class SecretaryController : Controller
    {
        private readonly UniversityDbContext _context;

        public SecretaryController(UniversityDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.CurrentPage = "Home";

            ViewBag.TotalStudents = _context.Students.Count();
            ViewBag.TotalProfessors = _context.Professors.Count();
            ViewBag.TotalCourses = _context.Courses.Count();

            return View();
        }

        public IActionResult ManageCourses()
        {
            ViewBag.CurrentPage = "Courses";
            var courses = _context.Courses.Include(c => c.ProfessorsAfmNavigation).ToList();
            return View(courses);
        }

        [HttpGet]
        public IActionResult CreateCourse()
        {
            ViewBag.CurrentPage = "CreateCourse";

            var professorsList = _context.Professors
                .Select(p => new { Afm = p.Afm, FullName = p.Surname + " " + p.Name }).ToList();

            ViewData["ProfessorsAfm"] = new SelectList(professorsList, "Afm", "FullName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateCourse([Bind("CourseTitle,CourseSemester,ProfessorsAfm")] Course course)
        {
            ViewBag.CurrentPage = "CreateCourse";

            if (ModelState.IsValid)
            {
                _context.Add(course);
                _context.SaveChanges();
                return RedirectToAction(nameof(ManageCourses));
            }

            var professorsList = _context.Professors
                .Select(p => new { Afm = p.Afm, FullName = p.Surname + " " + p.Name }).ToList();
            ViewData["ProfessorsAfm"] = new SelectList(professorsList, "Afm", "FullName", course.ProfessorsAfm);
            return View(course);
        }

        [HttpGet]
        public IActionResult EditCourse(int id)
        {
            ViewBag.CurrentPage = "Courses";

            var course = _context.Courses.Find(id);
            if (course == null) return NotFound();

            var professorsList = _context.Professors
                .Select(p => new { Afm = p.Afm, FullName = p.Surname + " " + p.Name }).ToList();

            ViewData["ProfessorsAfm"] = new SelectList(professorsList, "Afm", "FullName", course.ProfessorsAfm);
            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCourse(int id, [Bind("IdCourse,CourseTitle,CourseSemester,ProfessorsAfm")] Course course)
        {
            ViewBag.CurrentPage = "Courses";
            if (id != course.IdCourse) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(course);
                    _context.SaveChanges();
                    TempData["Success"] = "Το μάθημα ενημερώθηκε!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Courses.Any(e => e.IdCourse == id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(ManageCourses));
            }

            var professorsList = _context.Professors
               .Select(p => new { Afm = p.Afm, FullName = p.Surname + " " + p.Name }).ToList();
            ViewData["ProfessorsAfm"] = new SelectList(professorsList, "Afm", "FullName", course.ProfessorsAfm);
            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCourse(int id)
        {
            var course = _context.Courses.Find(id);
            if (course != null)
            {
                try
                {
                    var courseEnrollments = _context.CourseHasStudents
                                                    .Where(x => x.CourseIdCourse == id)
                                                    .ToList();

                    if (courseEnrollments.Any())
                    {
                        _context.CourseHasStudents.RemoveRange(courseEnrollments);
                    }

                    _context.Courses.Remove(course);

                    _context.SaveChanges();

                    TempData["Success"] = "Το μάθημα και όλες οι σχετικές εγγραφές διαγράφηκαν επιτυχώς.";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Σφάλμα κατά τη διαγραφή: " + ex.Message;
                }
            }
            return RedirectToAction(nameof(ManageCourses));
        }

        [HttpGet]
        public IActionResult AssignStudentToCourse()
        {
            ViewBag.CurrentPage = "Assign";

            var studentsList = _context.Students
                .Select(s => new { RegistrationNumber = s.RegistrationNumber, FullName = s.Surname + " " + s.Name + " (" + s.RegistrationNumber + ")" }).ToList();
            ViewData["Students"] = new SelectList(studentsList, "RegistrationNumber", "FullName");

            ViewData["Courses"] = new SelectList(_context.Courses, "IdCourse", "CourseTitle");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AssignStudentToCourse(int studentId, int courseId)
        {
            ViewBag.CurrentPage = "Assign";

            bool exists = _context.CourseHasStudents.Any(x => x.CourseIdCourse == courseId && x.StudentsRegistrationNumber == studentId);

            if (exists)
            {
                TempData["Error"] = "Ο φοιτητής έχει ήδη δηλώσει αυτό το μάθημα!";
            }
            else
            {
                var enrollment = new CourseHasStudent
                {
                    CourseIdCourse = courseId,
                    StudentsRegistrationNumber = studentId,
                    GradeCourseStudent = null
                };
                _context.CourseHasStudents.Add(enrollment);
                _context.SaveChanges();
                TempData["Success"] = "Η δήλωση έγινε επιτυχώς!";
            }
            return RedirectToAction(nameof(AssignStudentToCourse));
        }

        public IActionResult ManageStudents()
        {
            ViewBag.CurrentPage = "Students";
            var students = _context.Students.ToList();
            return View(students);
        }

        [HttpGet]
        public IActionResult CreateStudent()
        {
            ViewBag.CurrentPage = "Students";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateStudent(Student student, string username, string password)
        {
            ViewBag.CurrentPage = "Students";

            if (_context.Users.Any(u => u.Username == username))
            {
                TempData["Error"] = "Το Username χρησιμοποιείται ήδη.";
                return View(student);
            }
            if (_context.Students.Any(s => s.RegistrationNumber == student.RegistrationNumber))
            {
                TempData["Error"] = "Ο Αριθμός Μητρώου υπάρχει ήδη.";
                return View(student);
            }

            var newUser = new User { Username = username, Password = password, Role = "Student" };

            try
            {
                _context.Users.Add(newUser);
                _context.SaveChanges(); 

                student.UsersUsername = username;
                _context.Students.Add(student);
                _context.SaveChanges(); 

                TempData["Success"] = "Ο φοιτητής δημιουργήθηκε!";
                return RedirectToAction(nameof(ManageStudents));
            }
            catch (Exception ex)
            {
                if (_context.Users.Any(u => u.Username == username))
                {
                    _context.Users.Remove(newUser);
                    _context.SaveChanges();
                }
                TempData["Error"] = "Σφάλμα: " + ex.Message;
                return View(student);
            }
        }

        [HttpPost]
        public IActionResult DeleteStudent(int id)
        {
            var student = _context.Students.Find(id);
            if (student != null)
            {
                var username = student.UsersUsername;
                var user = _context.Users.Find(username);

                try
                {
                    var studentGrades = _context.CourseHasStudents
                                                .Where(x => x.StudentsRegistrationNumber == id)
                                                .ToList();

                    if (studentGrades.Any())
                    {
                        _context.CourseHasStudents.RemoveRange(studentGrades);
                    }

                    _context.Students.Remove(student);

                    if (user != null)
                    {
                        _context.Users.Remove(user);
                    }

                    _context.SaveChanges(); 
                    TempData["Success"] = "Ο φοιτητής και όλα τα δεδομένα του διαγράφηκαν.";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Σφάλμα κατά τη διαγραφή: " + ex.Message;
                }
            }
            return RedirectToAction(nameof(ManageStudents));
        }

        public IActionResult ManageProfessors()
        {
            ViewBag.CurrentPage = "Professors";
            var professors = _context.Professors.ToList();
            return View(professors);
        }

        [HttpGet]
        public IActionResult CreateProfessor()
        {
            ViewBag.CurrentPage = "Professors";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateProfessor(Professor professor, string username, string password)
        {
            ViewBag.CurrentPage = "Professors";

            if (_context.Users.Any(u => u.Username == username))
            {
                TempData["Error"] = "Το Username υπάρχει ήδη.";
                return View(professor);
            }

            var newUser = new User
            {
                Username = username,
                Password = password,
                Role = "Professor"
            };

            try
            {

                professor.UsersUsernameNavigation = newUser;

                professor.UsersUsername = username;
                _context.Users.Add(newUser);
                _context.Professors.Add(professor);

                _context.SaveChanges();

                TempData["Success"] = "Ο καθηγητής προστέθηκε!";
                return RedirectToAction(nameof(ManageProfessors));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Σφάλμα: " + ex.InnerException?.Message ?? ex.Message;
                return View(professor);
            }
        }

        [HttpPost]
        public IActionResult DeleteProfessor(int id)
        {
            var prof = _context.Professors.Find(id);
            if (prof != null)
            {
                var username = prof.UsersUsername;
                var user = _context.Users.Find(username);

                try
                {
                    var courses = _context.Courses.Where(c => c.ProfessorsAfm == id).ToList();

                    
                    foreach (var c in courses)
                    {
                        c.ProfessorsAfm = null;
                    }

                    _context.Professors.Remove(prof);

                    if (user != null)
                    {
                        _context.Users.Remove(user);
                    }

                    _context.SaveChanges();
                    TempData["Success"] = "Ο καθηγητής διαγράφηκε και τα μαθήματά του αποδεσμεύτηκαν.";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Σφάλμα κατά τη διαγραφή: " + ex.Message;
                }
            }
            return RedirectToAction(nameof(ManageProfessors));
        }
    }
}