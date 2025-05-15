using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Kutuphane.Data;
using System.Linq;
using System.Text;

namespace Kutuphane.Controllers
{
    public class BaseController : Controller
    {
        protected readonly ApplicationDbContext _context;

        public BaseController(ApplicationDbContext context)
        {
            _context = context;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Skip authentication for User controller actions (except Logout)
            if (context.Controller is UserController && 
                context.ActionDescriptor.DisplayName != "Kutuphane.Controllers.UserController.Logout")
            {
                base.OnActionExecuting(context);
                return;
            }

            var userId = HttpContext.Session.GetString("UserId");
            
            // Eğer session yoksa, cookie kontrolü yap
            if (string.IsNullOrEmpty(userId))
            {
                // Cookie'de kullanıcı bilgisi var mı kontrol et
                if (Request.Cookies.TryGetValue("AuthUser", out string encryptedUserId))
                {
                    try
                    {
                        // Cookie'den kullanıcı ID'sini al ve çöz
                        string decodedUserId = Encoding.UTF8.GetString(Convert.FromBase64String(encryptedUserId));
                        
                        if (int.TryParse(decodedUserId, out int cookieUserId))
                        {
                            // Kullanıcıyı veritabanından kontrol et
                            var user = _context.Users.FirstOrDefault(u => u.UserId == cookieUserId);
                            
                            if (user != null)
                            {
                                // Kullanıcı geçerliyse, session'ı yeniden oluştur
                                HttpContext.Session.SetString("UserId", user.UserId.ToString());
                                HttpContext.Session.SetString("Email", user.Email);
                                HttpContext.Session.SetString("Role", user.Role);
                                
                                // userId değişkenini güncelle
                                userId = user.UserId.ToString();
                            }
                        }
                    }
                    catch
                    {
                        // Cookie çözülemezse sil
                        Response.Cookies.Delete("AuthUser");
                    }
                }
            }

            // Kullanıcı bilgilerini ViewBag'e ekle
            if (!string.IsNullOrEmpty(userId))
            {
                var user = _context.Users.FirstOrDefault(u => u.UserId.ToString() == userId);
                if (user != null)
                {
                    ViewBag.FullName = user.FirstName + " " + user.LastName;
                }
            }
            
            base.OnActionExecuting(context);

            // Hala giriş yapılmadıysa, Login sayfasına yönlendir
            if (string.IsNullOrEmpty(userId))
            {
                context.Result = RedirectToAction("Login", "User");
            }
        }
    }
} 