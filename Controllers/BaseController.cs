using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Kutuphane.Data;
using System.Linq;

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
            var userId = HttpContext.Session.GetString("UserId");
            if (!string.IsNullOrEmpty(userId))
            {
                var user = _context.Users.FirstOrDefault(u => u.UserId.ToString() == userId);
                if (user != null)
                {
                    ViewBag.FullName = user.FirstName + " " + user.LastName;
                }
            }
            base.OnActionExecuting(context);

            // Skip authentication for User controller actions
            if (context.Controller is UserController)
            {
                return;
            }

            // Check if user is logged in
            if (HttpContext.Session.GetString("UserId") == null)
            {
                context.Result = RedirectToAction("Login", "User");
            }
        }
    }
} 