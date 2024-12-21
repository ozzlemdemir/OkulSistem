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

        [HttpGet("OgrenciGuncelle/{id?}")]
        public IActionResult OgrenciGuncelle(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return View();
            }

            // Öğrenciyi bulmak için veritabanı sorgusu
            var guncelogrenci = _context.Students.FirstOrDefault(x => x.StudentID == id);

            if (guncelogrenci == null)
            {
                TempData["ErrorMessage"] = "Öğrenci bulunamadı.";
                return View();
            }

            return View(guncelogrenci); 
        }

        [HttpPost("OgrenciGuncelle")]
        public async Task<IActionResult> OgrenciGuncelle(Student student)
        {
            try
            {
                
                if (student == null || string.IsNullOrEmpty(student.StudentID))
                {
                    TempData["ErrorMessage"] = "Güncelleme için geçerli bir öğrenci ID'si giriniz.";
                    return View(student);
                }

               
                var guncelogrenci = _context.Students.FirstOrDefault(s => s.StudentID == student.StudentID);

                if (guncelogrenci == null)
                {
                    TempData["ErrorMessage"] = "Güncellemek istediğiniz öğrenci bulunamadı.";
                    return View(student);
                }


                if (!string.IsNullOrEmpty(student.FirstName))
                {
                    guncelogrenci.FirstName = student.FirstName;
                }

                if (!string.IsNullOrEmpty(student.LastName))
                {
                    guncelogrenci.LastName = student.LastName;
                }

                if (!string.IsNullOrEmpty(student.Email))
                {
                    guncelogrenci.Email = student.Email;
                }

                if (!string.IsNullOrEmpty(student.Department))
                {
                    guncelogrenci.Department = student.Department;
                }

                if (!string.IsNullOrEmpty(student.Password))
                {
                    guncelogrenci.Password = student.Password;
                }

                _context.Update(guncelogrenci);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Öğrenci bilgileri başarıyla güncellendi.";
                return RedirectToAction("OgrenciGuncelle", new { id = student.StudentID });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Bir hata oluştu: {ex.Message}";
                return View(student);
            }
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
            
            return View();//bu methodu çağırdığımızda sayfanın dönmesi için
        }

        [HttpPost]
        public async Task<IActionResult> OgrenciEkle(Student student)
        {
            try
            {
                
                if (ModelState.IsValid)
                {
                    string selectedRole = Request.Form["Role"];
                    student.Role = selectedRole;
                    _context.Students.Add(student);//veri tabanına ekle yeni öğrenciyi
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Öğrenci eklendi.";
                    return RedirectToAction("OgrenciEkle");
                }

               
                return View(student);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Bir hata oluştu: {ex.Message}";
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
                .FirstOrDefaultAsync(ic => ic.InstructorCourseID == selectionID);

            if (courseRequest != null)
            {
                courseRequest.Onay = true; 

               
                var studentCourse = new StudentsCourse
                {
                    StudentID = courseRequest.StudentID,
                    CourseID = courseRequest.CourseID
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
                    CourseCredit = joined.c.Credits 
                })
                .ToListAsync();

            return View(studentsInCourses);
        

    }





    }
}
