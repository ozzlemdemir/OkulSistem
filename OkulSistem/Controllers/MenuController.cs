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
            
            var studentEmail = User.Identity.Name; 
            var student = _context.Instructors.FirstOrDefault(x => x.Email == studentEmail);

            if (student == null)
            {
                return RedirectToAction("GirisYap", "Login"); 
            }

            return View();
        }
    }
}
