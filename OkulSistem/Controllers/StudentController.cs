using Microsoft.AspNetCore.Mvc;
using OkulSistem.Data;

namespace OkulSistem.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult BilgiGetir()
        {

            var studentid = HttpContext.Session.GetString("StudentID");
            HttpContext.Session.SetString("StudentID", studentid);

            var student = _context.Students
                .FirstOrDefault(i => i.StudentID == studentid);

            return View(student);
        }
    }
}
