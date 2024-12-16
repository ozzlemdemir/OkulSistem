
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
        public IActionResult GirisYap(Instructor instructor)
        {
            try
            {
                if (instructor == null || string.IsNullOrEmpty(instructor.Email) || string.IsNullOrEmpty(instructor.Password))
                {
                    ViewBag.ErrorMessage = "giris bilgileri eksik ";
                    return View();
                }

                var bilgiler = _context.Instructors//burada girilen bikgilerin veri tabanı ile uyuşup uyuşmadıgı kontrol edilir.
                    .FirstOrDefault(x => x.Email == instructor.Email && x.Password == instructor.Password);

                if (bilgiler != null)
                {
                    HttpContext.Session.SetString("InstructorID", bilgiler.InstructorID);
                    return RedirectToAction("Index","Home");//kullanıcı başarılı giriş yapmışsa home index sayfasına yönlnedirilir
                }

                ViewBag.ErrorMessage = "Hatalı e-posta veya şifre.";
                return View();
            }
            catch (Exception ex)
            {
                
                ViewBag.ErrorMessage = $"Bir hata oluştu: {ex.Message}";//sunu u hatası durumunda çalışır
                return View();
            }
        }

    }
}    
