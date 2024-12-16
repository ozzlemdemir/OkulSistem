using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OkulSistem.Data;
using OkulSistem.Models;
using System.Linq;

namespace OkulSistem.Controllers
{
   /*[Route("api/[controller]")]
    [ApiController]*/

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
                .Select(sc => new StudentsCourse
                {
                    SelectionID = sc.SelectionID,
                    StudentID = sc.Student.StudentID,
                    CourseID = sc.Course.CourseID,
                    StudentName= sc.Student.FirstName,
                    StudentLastName = sc.Student.LastName,



                })
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


        [HttpPut("OgrenciGuncelleme")]
        public async Task<ActionResult<Student>> OgrenciGuncelle(Student student, string id)
        {
            var guncelogrenci = _context.Students.FirstOrDefault(x => x.StudentID == id);
            if (guncelogrenci == null)
            {
                return NotFound("Öğrenci bulunamadı.");
            }

            // Null olmayan özellikleri güncelle
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

            await _context.SaveChangesAsync(); // Değişiklikleri kaydet
            return Ok(guncelogrenci); // Güncellenmiş öğrenci döndür
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
        [HttpPost("OgrenciEkle")]
        public async Task<ActionResult> OgrenciEkle(Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return Ok(student);
        }



    }
}
