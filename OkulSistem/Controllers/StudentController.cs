using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OkulSistem.Data;
using OkulSistem.Models;

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
        [HttpGet("Courses/{studentId?}")]
        public IActionResult GetStudentCourses(string studentId)
        {
            if (string.IsNullOrEmpty(studentId))
            {
                studentId = HttpContext.Session.GetString("StudentID");
            }


            var studentCourses = _context.StudentsCourses
                .Where(sc => sc.StudentID == studentId)
                .Include(sc => sc.Course) // İlişkili Course tablosunu dahil ediyoruz
                .ToList();
            return View(studentCourses); // Kurs listesini View'a gönderiyoruz
        }
        [HttpGet]
        public async Task<IActionResult> DersSec()
        {
            var studentId = HttpContext.Session.GetString("StudentID");
            var secilenKurslar = await _context.StudentsCourses
                                            .Where(sc => sc.StudentID == studentId)
                                            .Select(sc => sc.CourseID)
                                            .ToListAsync();
            var secilebilenKurs = await _context.Courses.Where(c=>!secilenKurslar.Contains(c.CourseID)) // Zaten seçilmiş dersleri hariç tut
                                              .ToListAsync();
            return View(secilebilenKurs);
        }
        [HttpPost]
        public async Task<IActionResult> DersSec(string courseId)
        {
            var studentId = HttpContext.Session.GetString("StudentID");
            var existingSelection=await _context.StudentsCourses.FirstOrDefaultAsync(sc => sc.StudentID == studentId && sc.CourseID == courseId);
            if(existingSelection != null)
            {
                TempData["Hata"] = "Bu dersi Zaten seçmişsiniz";
                return RedirectToAction("DersSec");
            }
            var studentcourse = new StudentsCourse
            {
                
                StudentID=studentId,
                CourseID=courseId
            };
            _context.StudentsCourses.Add(studentcourse);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Ders Seçimi Yapıldı!";
            return RedirectToAction("DersSec");
        }
    }
}
