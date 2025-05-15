using Microsoft.AspNetCore.Mvc;
using Kutuphane.Data; // Adjust namespace based on your project structure
using Kutuphane.Models; // Adjust namespace based on your models
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Kutuphane.Controllers
{
    public class OgrenciController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public OgrenciController(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        // GET: Ogrenci
        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            var ogrenciler = await _context.Ogrenciler
                .Include(o => o.Sinif)
                .Where(o => o.UserId == userId && o.Aktif)
                .ToListAsync();
            return View(ogrenciler);
        }

        public async Task<IActionResult> Ekle()
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            ViewBag.Siniflar = new SelectList(
                await _context.Siniflar.Where(s => s.UserId == userId).ToListAsync(),
                "Id", "SinifAdi"
            );
            return View();
        }
        // POST: Ogrenci/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ekle(Ogrenci ogrenci)
        {
            if (ModelState.IsValid)
            {
                ogrenci.UserId = int.Parse(HttpContext.Session.GetString("UserId"));
                _context.Ogrenciler.Add(ogrenci);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            ViewBag.Siniflar = new SelectList(
                await _context.Siniflar.Where(s => s.UserId == userId).ToListAsync(),
                "Id", "SinifAdi"
            );
            return View(ogrenci);
        }


        [HttpGet]
        public async Task<IActionResult> Guncelle(int id)
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            var ogrenci = await _context.Ogrenciler.FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);
            if (ogrenci == null) return NotFound();

            ViewBag.Siniflar = new SelectList(
                await _context.Siniflar.Where(s => s.UserId == userId).ToListAsync(),
                "Id", "SinifAdi"
            );
            return View(ogrenci);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Guncelle(Ogrenci ogrenci)
        {
            if (!ModelState.IsValid)
            {
                var userId = int.Parse(HttpContext.Session.GetString("UserId"));
                ViewBag.Siniflar = new SelectList(
                    await _context.Siniflar.Where(s => s.UserId == userId).ToListAsync(),
                    "Id", "SinifAdi"
                );
                return View(ogrenci);
            }
            var userId2 = int.Parse(HttpContext.Session.GetString("UserId"));
            var existingOgrenci = await _context.Ogrenciler.AsNoTracking().FirstOrDefaultAsync(o => o.Id == ogrenci.Id && o.UserId == userId2);
            if (existingOgrenci == null) return NotFound();

            ogrenci.UserId = userId2;
            _context.Update(ogrenci);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> Sil(int id)
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            var ogrenci = await _context.Ogrenciler.FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);
            if (ogrenci == null) return NotFound();

            return View(ogrenci);
        }

        [HttpPost, ActionName("Sil")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SilOnayla(int id)
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            var ogrenci = await _context.Ogrenciler.FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);
            if (ogrenci == null) return NotFound();

            ogrenci.Aktif = false;
            _context.Ogrenciler.Update(ogrenci);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }

   
    
}