using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OkulSistem.Data;
using OkulSistem.Models;
using System.Linq;

namespace OkulSistem.Controllers
{
   public class InstructorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InstructorController(ApplicationDbContext context)
        {
            _context = context;
        }

        //http://localhost:5115/api/Instructor/StudentList
        [HttpGet("StudentList")]
        public async Task<ActionResult<IEnumerable<Student>>> GetS()
        {
            var students = await _context.Students.ToListAsync();//tüm öğrencilerin listesini students nesnesi üzerinden listeliyoruz
            return View(students);//GetS viewimiz ile görselleştiriyoruz
        }
        [HttpGet("CoursesList")]
        public async Task<ActionResult<IEnumerable<Student>>> KursListele()
        {
            var kurslar = await _context.Courses.ToListAsync();//tüm kurslar kurslar nesnesi üzerinden listeliyoruz
            return View(kurslar);
        }
        [HttpGet("StudentByID/{id?}")] 
        public async Task<IActionResult> StudentByID(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "lutfen id giriniz";
                return View(null); 
            }
            //burada kullanıcın girdiği değere göre veri tabanında uyuşan verinin olup olmadıgını kontrol ediyoruz
            var ogrenci = await _context.Students.FirstOrDefaultAsync(x => x.StudentID == id);
            if (ogrenci == null)
            {
                TempData["Error"] = $" {id} id'sine ait öğrenci yok";
                return View(null); 
            }

            return View(ogrenci); //ogrenci varsa view şeklinde görselleştiriyoruz
        }

        [HttpGet("StudentCourses")]
        public async Task<IActionResult> StudentCoursesList()
        {
            var ogrenciKurslar = await _context.StudentsCourses
         .Include(sc => sc.Student)   
         .Include(sc => sc.Course)  
         .ToListAsync();  

            return View(ogrenciKurslar);
        }


        
        [HttpGet("DeleteStudent")]
        public IActionResult DeleteStudent()
        {
            return View();//kullanıcıdan id alacağımız kısmı getirdik
        }
        [HttpPost("DeleteStudent/{id?}")]
        public IActionResult DeleteStudent(string id)
        {
            var ogrenci = _context.Students.FirstOrDefault(x => x.StudentID == id);

            if (ogrenci == null)
            {
                TempData["Error"] = "Böyle bir öğrenci yok.";
                return RedirectToAction("DeleteStudent");
            }

            // Onay ekranına öğrenci bilgilerini gönder
            TempData["StudentID"] = id;
            TempData["StudentName"] = $"{ogrenci.FirstName} {ogrenci.LastName}"; // Öğrenci adı ve soyadı
            return RedirectToAction("ConfirmDeleteStudent");
        }

        [HttpGet("ConfirmDeleteStudent")]
        public IActionResult ConfirmDeleteStudent()
        {
            return View(); // Onay ekranını döndür
        }

        [HttpPost("ConfirmDeleteStudent")]
        public async Task<IActionResult> ConfirmDeleteStudentPost(string id)
        {
            var ogrenci = _context.Students.FirstOrDefault(x => x.StudentID == id);

            if (ogrenci == null)
            {
                TempData["Error"] = "Böyle bir öğrenci yok.";
                return RedirectToAction("DeleteStudent");
            }

            _context.Students.Remove(ogrenci);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Öğrenci başarıyla silindi.";
            return RedirectToAction("DeleteStudent");
        }

        

        [HttpGet]
        public IActionResult InstructorDetails()
        {

            var instructorId = HttpContext.Session.GetString("InstructorID");
            HttpContext.Session.SetString("InstructorID", instructorId);

            var instructor = _context.Instructors
                .FirstOrDefault(i => i.InstructorID == instructorId);

            return View(instructor);
        }
        [HttpGet]
        public IActionResult OgrenciEkle()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> OgrenciEkle(Student student)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    if (string.IsNullOrWhiteSpace(student.StudentID))
                    {
                        TempData["ErrorMessage"] = "Öğrenci ID'si boş bırakılamaz.";
                        return View(student);
                    }

                    var existingStudent = await _context.Students
            .FirstOrDefaultAsync(s => s.StudentID == student.StudentID);

                    if (existingStudent != null)
                    {
                        TempData["ErrorMessage"] = "Bu ID'ye sahip bir öğrenci zaten var.";
                        return View(student);
                    }
                    student.Role = Request.Form["Role"];
                    _context.Students.Add(student);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Öğrenci başarıyla eklendi.";
                    return RedirectToAction("GetS"); // Tüm öğrencileri listeleme sayfasına yönlendir
                }

                return View(student);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Öğrenci eklenirken bir hata oluştu.";
                return View(student);
            }
        }


        [HttpGet]
        public async Task<IActionResult> DersIstekleri()
        {
            var currentInstructorID = HttpContext.Session.GetString("InstructorID");

            
            var pendingRequests = await _context.InstructorCourses
                .Where(ic => ic.InstructorID == currentInstructorID && ic.Onay == false)
                .Include(ic => ic.Course)
                .Include(ic => ic.Student) 
                .ToListAsync();

            return View(pendingRequests);
        }
        [HttpPost]
        public async Task<IActionResult> DersOnayla(int selectionID)
        {
           
            var courseRequest = await _context.InstructorCourses
                .Include(ic => ic.Student)
                .FirstOrDefaultAsync(ic => ic.InstructorCourseID == selectionID);

            var student = await _context.Students
           .FirstOrDefaultAsync(s => s.StudentID == courseRequest.StudentID);

            if (courseRequest != null)
            {
                courseRequest.Onay = true; 

               
                var studentCourse = new StudentsCourse
                {
                    StudentID = courseRequest.StudentID,
                    CourseID = courseRequest.CourseID,
                    StudentName=student.FirstName,
                    StudentLastName=student.LastName
                   
                };
                _context.StudentsCourses.Add(studentCourse);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("DersIstekleri");
        }
        [HttpPost]
        public async Task<IActionResult> DersReddet(int selectionID)
        {
            var courseRequest = await _context.InstructorCourses
                .FirstOrDefaultAsync(ic => ic.InstructorCourseID == selectionID);

            if (courseRequest != null)
            {
                _context.InstructorCourses.Remove(courseRequest);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("DersIstekleri");
        }
        [HttpGet]
        public async Task<IActionResult> DersimiAlanOgrenciler()
        {
            var currentInstructorID = HttpContext.Session.GetString("InstructorID");

           
            var studentsInCourses = await _context.StudentsCourses
                .Join(_context.Courses,
                    sc => sc.CourseID,
                    c => c.CourseID,
                    (sc, c) => new { sc, c }) 
                .Where(joined => joined.c.InstructorID.ToString() == currentInstructorID) 
                .Select(joined => new
                {
                    StudentName = joined.sc.Student.FirstName,
                    CourseName = joined.c.CourseName, 
                    StudentLastName=joined.sc.Student.LastName,
                    CourseCredit = joined.c.Credits 
                })
                .ToListAsync();

            return View(studentsInCourses);
        

    }





    }
}
