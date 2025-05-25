using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kutuphane.Data;
using Kutuphane.Models;
using System.Security.Cryptography;
using System.Text;

namespace Kutuphane.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: User/Register
        public IActionResult Register()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: User/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                // Check if username already exists
                if (await _context.Users.AnyAsync(u => u.Username == user.Username))
                {
                    ModelState.AddModelError("Username", "Bu kullanıcı adı zaten kullanılıyor.");
                    return View(user);
                }

                // Hash password
                user.Password = HashPassword(user.Password);

                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login");
            }
            return View(user);
        }

        // GET: User/Login
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: User/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string Email, string login_password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == Email);
            
            if (user != null && VerifyPassword(login_password, user.Password))
            {
                // Session'a kullanıcı bilgilerini kaydet
                HttpContext.Session.SetString("UserId", user.UserId.ToString());
                HttpContext.Session.SetString("Email", user.Email);
                HttpContext.Session.SetString("Role", user.Role);

                // Kalıcı cookie ayarla (her zaman hatırla)
                CookieOptions cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(30),
                    HttpOnly = true,
                    IsEssential = true,
                    SameSite = SameSiteMode.Lax
                };

                // Kullanıcı kimliğini şifreleyerek cookie'ye kaydet
                string encryptedUserId = Convert.ToBase64String(Encoding.UTF8.GetBytes(user.UserId.ToString()));
                Response.Cookies.Append("AuthUser", encryptedUserId, cookieOptions);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Geçersiz e-posta veya şifre");
            return View();
        }

        // GET: User/Logout
        public IActionResult Logout()
        {
            // Session'ı temizle
            HttpContext.Session.Clear();
            
            // Cookie'yi sil
            Response.Cookies.Delete("AuthUser");
            
            return RedirectToAction("Login");
        }

        // GET: User/Profile
        public async Task<IActionResult> Profile()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            var user = await _context.Users.FindAsync(int.Parse(userId));
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(User user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _context.Users.FindAsync(user.UserId);
                if (existingUser == null)
                {
                    return NotFound();
                }

                // Update user properties
                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                existingUser.Email = user.Email;

                // If password is provided, update it
                if (!string.IsNullOrEmpty(user.Password))
                {
                    existingUser.Password = HashPassword(user.Password);
                }

                try
                {
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Profil başarıyla güncellendi.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Profile));
            }
            return View(user);
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string inputPassword, string hashedPassword)
        {
            return HashPassword(inputPassword) == hashedPassword;
        }

        // GET: User/CreateUser
        public IActionResult CreateUser()
        {
            // Kullanıcı giriş yapmamışsa login sayfasına yönlendir
            if (HttpContext.Session.GetString("UserId") == null)
                return RedirectToAction("Login");
            
            // Kullanıcı admin değilse anasayfaya yönlendir
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("Index", "Home");
            
            return View();
        }

        // POST: User/CreateUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(User user)
        {
            // Kullanıcı giriş yapmamışsa login sayfasına yönlendir
            if (HttpContext.Session.GetString("UserId") == null)
                return RedirectToAction("Login");

            // Kullanıcı admin değilse anasayfaya yönlendir
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                // Check if username already exists
                if (await _context.Users.AnyAsync(u => u.Username == user.Username))
                {
                    ModelState.AddModelError("Username", "Bu kullanıcı adı zaten kullanılıyor.");
                    return View(user);
                }

                // Hash password
                user.Password = HashPassword(user.Password);

                _context.Add(user);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Yeni kullanıcı başarıyla oluşturuldu.";
                return RedirectToAction("Profile");
            }
            return View(user);
        }
    }
} 