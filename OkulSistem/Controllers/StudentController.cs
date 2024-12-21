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
        public async Task<IActionResult> OnayBekleyen()
        {
            var currentStudentID= HttpContext.Session.GetString("StudentID");
            var pendingCourses = await _context.InstructorCourses
                                          .Where(i => i.StudentID == currentStudentID && i.Onay == false)
                                          .Include(i=> i.Course)
                                          .ToListAsync();
            return View(pendingCourses);
        }
       
        [HttpGet]
        public async Task<IActionResult> DersSec()
        {
            var currentStudentID = HttpContext.Session.GetString("StudentID");

           
            var alinanDersler = await _context.StudentsCourses
                .Where(sc => sc.StudentID == currentStudentID)
                .Select(sc => sc.CourseID)
                .ToListAsync();

           
            var availableCourses = await _context.Courses
                .Where(c => !alinanDersler.Contains(c.CourseID))
                .ToListAsync();

            return View(availableCourses);  
        } 
        [HttpPost]
        public async Task<IActionResult> DersSec(string courseId)
        {
            var currentStudentID = HttpContext.Session.GetString("StudentID");
            var alinanders= await _context.StudentsCourses
                                         .AnyAsync(sc => sc.StudentID == currentStudentID && sc.CourseID == courseId);
            if (alinanders)
            {
                TempData["ErrorMessage"] = "Bu dersi zaten aldınız!";
                return RedirectToAction("DersSec");
            }
            var course = await _context.Courses.Include(c => c.Instructor)
                               .FirstOrDefaultAsync(c => c.CourseID == courseId);
            if (course != null && course.Instructor!=null)
            {
                var courseRequest = new InstructorCourse
                {
                    StudentID = currentStudentID,
                    CourseID = courseId,
                    Onay = false,
                    InstructorID = course.InstructorID,
                };

                _context.InstructorCourses.Add(courseRequest);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Onaybekleyen");
        
    }
    }
}
