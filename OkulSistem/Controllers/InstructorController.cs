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

        //http://localhost:5115/api/Instructor/id?id=2
        [HttpGet("DeleteStudent")]
        public IActionResult DeleteStudent()
        {
            return View();//kullanıcıdan id alacağımız kısmı getirdik
        }
        [HttpPost("DeleteStudent/{id?}")]
        public async Task<IActionResult> DeleteStudent(string id)
        {
            
            var ogrenci = _context.Students.FirstOrDefault(x => x.StudentID == id); //ogrenci nesnesine silinecek öğrenciyi atadık

            if (ogrenci == null)
            {
                
                TempData["Error"] = "böyle bir öğrenci yok.";
                return RedirectToAction("DeleteStudent");
            }

            
            _context.Students.Remove(ogrenci);//atadıgımız öğrenciyi sildik
            await _context.SaveChangesAsync();//değişiklikleri kaydettik

            
            TempData["Success"] = "ogrenci silindi";
            return RedirectToAction("DeleteStudent");//öğrenci silindikten sonra tekrar başka öğrenic silinmek istenirse DeleteStudent sayfasına geri döndük
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
        public IActionResult KursEkle()
        {

            return View();//bu methodu çağırdığımızda sayfanın dönmesi için
        }

        [HttpPost]
        public async Task<IActionResult> KursEkle(Course kurs)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    _context.Courses.Add(kurs);//veri tabanına ekle yeni öğrenciyi
                    await _context.SaveChangesAsync();
                     TempData["SuccessMessage"] = "Kurs eklendi.";

                    return RedirectToAction("KursEkle");
                }

                
                return View(kurs);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Bir hata oluştu: {ex.Message}";
                return View(kurs);
            }
        }





    }
}
