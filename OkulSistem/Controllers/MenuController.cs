using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using OkulSistem.Data;


namespace OkulSistem.Controllers
{
    public class MenuController : Controller
    {
        private readonly ApplicationDbContext _context;
        public MenuController(ApplicationDbContext context)
        {
            _context = context; 
        }
        public IActionResult OgrenciMenu ()
        {
            // Giriş yapan kullanıcıyı al
            var studentEmail = User.Identity.Name; // Kullanıcı adı, giriş yapan kullanıcının email'ini temsil eder
            var student = _context.Instructors.FirstOrDefault(x => x.Email == studentEmail);

            if (student == null)
            {
                return RedirectToAction("GirisYap", "Login"); // Giriş yapılmamışsa login sayfasına yönlendir
            }

            
            ViewBag.Instructor = student;  // Giriş yapan instructor'ı view'a gönder

            return View();
        }
    }
}
