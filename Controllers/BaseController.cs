using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Kutuphane.Data;
using Kutuphane.Models;
using System.Security.Cryptography;
using System.Text;

namespace Kutuphane.Controllers
{
    public class BaseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BaseController(ApplicationDbContext context)
        {
            _context = context;
            // Admin kullanıcısı yoksa oluştur
            CreateAdminUserIfNotExists().Wait();
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Kullanıcı giriş yapmamışsa login sayfasına yönlendir
            if (HttpContext.Session.GetString("UserId") == null)
            {
                // AuthUser cookie'si varsa, kullanıcıyı otomatik giriş yap
                if (Request.Cookies.ContainsKey("AuthUser"))
                {
                    try
                    {
                        var encryptedUserId = Request.Cookies["AuthUser"];
                        var userId = Encoding.UTF8.GetString(Convert.FromBase64String(encryptedUserId));
                        var user = _context.Users.Find(int.Parse(userId));
                        if (user != null)
                        {
                            HttpContext.Session.SetString("UserId", user.UserId.ToString());
                            HttpContext.Session.SetString("Email", user.Email);
                            HttpContext.Session.SetString("Role", user.Role);
                        }
                        else
                        {
                            // Geçersiz cookie, sil
                            Response.Cookies.Delete("AuthUser");
                            context.Result = new RedirectToActionResult("Login", "User", null);
                            return;
                        }
                    }
                    catch
                    {
                        // Hata oluşursa cookie'yi sil ve login sayfasına yönlendir
                        Response.Cookies.Delete("AuthUser");
                        context.Result = new RedirectToActionResult("Login", "User", null);
                        return;
                    }
                }
                else
                {
                    context.Result = new RedirectToActionResult("Login", "User", null);
                    return;
                }
            }

            // Kullanıcı bilgilerini ViewBag'e ekle
            try
            {
                var userId = int.Parse(HttpContext.Session.GetString("UserId"));
                var user = _context.Users.Find(userId);
                ViewBag.FullName = $"{user.FirstName} {user.LastName}";
                ViewBag.UserRole = user.Role;
            }
            catch
            {
                // Kullanıcı bulunamazsa, oturumu sonlandır
                HttpContext.Session.Clear();
                Response.Cookies.Delete("AuthUser");
                context.Result = new RedirectToActionResult("Login", "User", null);
                return;
            }

            base.OnActionExecuting(context);
        }

        // Admin kullanıcısı oluşturma metodu
        private async Task CreateAdminUserIfNotExists()
        {
            // Admin kullanıcısı zaten var mı kontrol et
            if (!await _context.Users.AnyAsync(u => u.Role == "Admin"))
            {
                // Admin kullanıcısı yok, oluştur
                var adminUser = new User
                {
                    Username = "admin",
                    Email = "admin@example.com",
                    FirstName = "Admin",
                    LastName = "User",
                    Role = "Admin",
                    Password = HashPassword("admin123") // varsayılan şifre
                };

                _context.Users.Add(adminUser);
                await _context.SaveChangesAsync();
            }
        }

        // Şifre hashleme yardımcı metodu
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
} 