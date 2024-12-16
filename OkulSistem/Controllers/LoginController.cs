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

namespace OkulSistem.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context)
        {
            _context = context; // Veritabanı bağlamını Dependency Injection ile alıyoruz
        }

        [HttpGet]
        public IActionResult GirisYap()
        {
            return View(); // Login ekranını döndür
        }

        [HttpPost]
        public IActionResult GirisYap(Instructor instructor)
        {
            try
            {
                if (instructor == null || string.IsNullOrEmpty(instructor.Email) || string.IsNullOrEmpty(instructor.Password))
                {
                    ViewBag.ErrorMessage = "Model bağlanmadı veya giriş bilgileri eksik.";
                    return View();
                }

                var bilgiler = _context.Instructors
                    .FirstOrDefault(x => x.Email == instructor.Email && x.Password == instructor.Password);

                if (bilgiler != null)
                {
                    return RedirectToAction("Index","Home");
                }

                ViewBag.ErrorMessage = "Hatalı e-posta veya şifre.";
                return View();
            }
            catch (Exception ex)
            {
                // Log hatayı
                ViewBag.ErrorMessage = $"Bir hata oluştu: {ex.Message}";
                return View();
            }
        }

    }
}    
