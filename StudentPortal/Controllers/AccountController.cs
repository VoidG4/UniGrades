using Microsoft.AspNetCore.Mvc;
using StudentPortal.Models;

namespace StudentPortal.Controllers
{
    public class AccountController : Controller
    {
        private readonly UniversityDbContext _context;

        public AccountController(UniversityDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                if (user.Role == "Student")
                {
                    var student = _context.Students.FirstOrDefault(s => s.UsersUsername == username);
                    if (student != null)
                    {
                        return RedirectToAction("Index", "Student", new { id = student.RegistrationNumber });
                    }
                }
                else if (user.Role == "Professor")
                {
                    var prof = _context.Professors.FirstOrDefault(p => p.UsersUsername == username);
                    if (prof != null)
                    {
                        return RedirectToAction("Index", "Professor", new { id = prof.Afm });
                    }
                }
                else if (user.Role == "Secretary")
                {
                    return RedirectToAction("Index", "Secretary");
                }
            }

            ViewBag.Error = "Λάθος όνομα χρήστη ή κωδικός";
            return View();
        }
    }
}