using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using OkulSistem.Data;

namespace OkulSistem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context; // Veritabanı bağlamını Dependency Injection ile alıyoruz
        }

         public IActionResult Index()
         {
            // Giriş yapan kullanıcıyı al
            var instructorEmail = User.Identity.Name; // Kullanıcı adı, giriş yapan kullanıcının email'ini temsil eder
            var instructor = _context.Instructors.FirstOrDefault(x => x.Email == instructorEmail);

          if (instructor == null)
          {
            return RedirectToAction("GirisYap", "Login"); // Giriş yapılmamışsa login sayfasına yönlendir
          }

           // Öğrenciler ve dersler gibi bilgileri gönderebiliriz (isteğe bağlı)
           ViewBag.Instructor = instructor;  // Giriş yapan instructor'ı view'a gönder

             return View();
         }
        public IActionResult Logout()
        {
            // Kullanıcı oturumunu sonlandır
            HttpContext.SignOutAsync();

            // Kullanıcıyı login sayfasına yönlendir
            return RedirectToAction("GirisYap", "Login");
        }
    }
}
