using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Kutuphane.Models;
using Kutuphane.Data;

namespace Kutuphane.Controllers
{
    public class OduncController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public OduncController(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        // GET: Odunc
        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            var oduncler = await _context.Oduncler
                .Include(o => o.Kitap)
                .Include(o => o.Ogrenci)
                .Where(o => o.UserId == userId && o.Aktif)
                .ToListAsync();
            return View(oduncler);
        }

        // GET: Odunc/Create
        public IActionResult Create(int? kitapId = null)
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            var kitaplar = _context.Kitaplar.Where(k => k.UserId == userId).ToList();
            var ogrenciler = _context.Ogrenciler.Where(o => o.UserId == userId).ToList();
            ViewData["KitapId"] = new SelectList(kitaplar, "Id", "KitapAdi", kitapId);
            ViewData["OgrenciId"] = new SelectList(
                ogrenciler.Select(o => new { o.Id, AdSoyad = o.OgrenciAdi + " " + o.OgrenciSoyadi }),
                "Id", "AdSoyad"
            );
            ViewData["SeciliKitapId"] = kitapId;
            return View();
        }

        // GET: Odunc/AraKitaplar
        [HttpGet]
        public JsonResult AraKitaplar(string term)
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            var kitaplar = _context.Kitaplar
                .Where(k => k.UserId == userId && k.KitapAdi.Contains(term) && k.Aktif)
                .Select(k => new { id = k.Id, text = k.KitapAdi })
                .ToList();
            
            return Json(kitaplar);
        }

        // GET: Odunc/AraOgrenciler
        [HttpGet]
        public JsonResult AraOgrenciler(string term)
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            var ogrenciler = _context.Ogrenciler
                .Where(o => o.UserId == userId && o.Aktif && 
                    (o.OgrenciAdi.Contains(term) || o.OgrenciSoyadi.Contains(term)))
                .Select(o => new { 
                    id = o.Id, 
                    text = o.OgrenciAdi + " " + o.OgrenciSoyadi 
                })
                .ToList();
            
            return Json(ogrenciler);
        }

        // POST: Odunc/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("KitapId,OgrenciId,IadeTarihi")] Odunc odunc)
        {
            if (ModelState.IsValid)
            {
                odunc.OduncAlmaTarihi = DateTime.Now;
                odunc.IadeEdildi = false;
                odunc.Aktif = true;
                odunc.UserId = int.Parse(HttpContext.Session.GetString("UserId"));
                _context.Add(odunc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            var kitaplar = _context.Kitaplar.Where(k => k.UserId == userId).ToList();
            var ogrenciler = _context.Ogrenciler.Where(o => o.UserId == userId).ToList();
            ViewData["KitapId"] = new SelectList(kitaplar, "Id", "KitapAdi", odunc.KitapId);
            ViewData["OgrenciId"] = new SelectList(
                ogrenciler.Select(o => new { o.Id, AdSoyad = o.OgrenciAdi + " " + o.OgrenciSoyadi }),
                "Id", "AdSoyad", odunc.OgrenciId
            );
            return View(odunc);
        }

        // POST: Odunc/IadeAl/5
        [HttpPost]
        public async Task<IActionResult> IadeAl(int id)
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            var odunc = await _context.Oduncler.FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);
            if (odunc != null && !odunc.IadeEdildi)
            {
                odunc.IadeEdildi = true;
                odunc.IadeTarihi = DateTime.Now;
                _context.Update(odunc);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<JsonResult> GetKitapBilgi(int id)
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            var kitap = await _context.Kitaplar
                .Include(k => k.Kategori)
                .FirstOrDefaultAsync(k => k.Id == id && k.UserId == userId);
            
            return Json(new { 
                stok = kitap?.StokAdedi ?? 0,
                kategori = kitap?.Kategori?.KategoriAdi ?? ""
            });
        }

        [HttpGet]
        public async Task<JsonResult> GetOgrenciBilgi(int id)
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            var ogrenci = await _context.Ogrenciler
                .Include(o => o.Sinif)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);
            
            var aktifOdunc = await _context.Oduncler
                .CountAsync(o => o.OgrenciId == id && !o.IadeEdildi && o.UserId == userId);
            
            return Json(new { 
                sinif = ogrenci?.Sinif?.SinifAdi ?? "",
                aktifOdunc = aktifOdunc
            });
        }

        // POST: Odunc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string? returnUrl)
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            var odunc = await _context.Oduncler.FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);
            if (odunc != null)
            {
                // Soft delete - ödünç kaydını silmek yerine aktif alanını false yap
                odunc.Aktif = false;
                _context.Update(odunc);
                await _context.SaveChangesAsync();
            }
            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction(nameof(Index));
        }
    }
} 