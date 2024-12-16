using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OkulSistem.Data;
using OkulSistem.Models;
using System.Linq;

namespace OkulSistem.Controllers
{
   /* [Route("api/[controller]")]
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
            var students = await _context.Students.ToListAsync();
            return View(students);
        }
        [HttpGet("StudentByID/{id?}")] 
        public async Task<IActionResult> StudentByID(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "Öğrenci ID'si belirtilmedi.";
                return View(null); 
            }

            var ogrenci = await _context.Students.FirstOrDefaultAsync(x => x.StudentID == id);
            if (ogrenci == null)
            {
                TempData["Error"] = $"Bu ID'ye sahip öğrenci bulunamadı: {id}";
                return View(null); 
            }

            return View(ogrenci); 
        }

        [HttpGet("StudentCourses")]
        public async Task<ActionResult<IEnumerable<StudentsCourse>>> StudentCoursesList()
        {
            return await _context.StudentsCourses.ToListAsync();
        }

        //http://localhost:5115/api/Instructor/id?id=2
        [HttpGet("DeleteStudent")]
        public IActionResult DeleteStudent()
        {
            return View();
        }
        [HttpPost("DeleteStudent/{id?}")]
        public async Task<IActionResult> DeleteStudent(string id)
        {
            // Öğrenci ID'sine göre öğrenci arama
            var ogrenci = _context.Students.FirstOrDefault(x => x.StudentID == id);

            if (ogrenci == null)
            {
                // Öğrenci bulunamadığında hata mesajı
                TempData["Error"] = "Bu ID'ye sahip öğrenci bulunamadı.";
                return RedirectToAction("DeleteStudent");
            }

            // Öğrenciyi silme
            _context.Students.Remove(ogrenci);
            await _context.SaveChangesAsync();

            // Silme işlemi başarılı olduğunda kullanıcıya mesaj gösterme
            TempData["Success"] = "Öğrenci başarıyla silindi.";
            return RedirectToAction("DeleteStudent");
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
