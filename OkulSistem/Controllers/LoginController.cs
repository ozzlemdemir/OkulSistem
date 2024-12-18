
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Web;
using OkulSistem.Models;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using OkulSistem.Data;
using Microsoft.EntityFrameworkCore;
using OkulSistem.Controllers;

namespace OkulSistem.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GirisYap()
        {
            return View(); //default olarak GirisYap sayfası ayarlandı
        }
        [HttpPost]
        public IActionResult GirisYap(string email, string password, string role)
        {
           try
              {
                  if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
                     {
                        ViewBag.ErrorMessage = "Giriş bilgilerini eksik girdiniz!";
                        return View();
                      }

                 if (role == "Instructor")
                      {
                     var instructor = _context.Instructors
                      .FirstOrDefault(x => x.Email == email && x.Password == password && x.Role == role);

                       if (instructor != null)
                       {
                       HttpContext.Session.SetString("InstructorID", instructor.InstructorID);
                       return RedirectToAction("Index", "Home");
                        }
                  }
                   else if (role == "Student")
                      {
                     var student = _context.Students
                      .FirstOrDefault(x => x.Email == email && x.Password == password && x.Role == role);

                      if (student != null)
                       {
                        HttpContext.Session.SetString("StudentID", student.StudentID);
                        return RedirectToAction("OgrenciMenu", "Menu");
                       }
                    }

                  ViewBag.ErrorMessage = "Hatalı e-posta, şifre veya rol girdiniz!";
                 return View();
                }
             catch (Exception ex)
                  {
                   ViewBag.ErrorMessage = $"Bir hata oluştu: {ex.Message}";
                    return View();
                 }
}

    }
}    
