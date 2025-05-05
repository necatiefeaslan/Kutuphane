using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Kutuphane.Models;
using Kutuphane.Data;

namespace Kutuphane.Controllers
{
    public class OduncController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OduncController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Odunc
        public async Task<IActionResult> Index()
        {
            var oduncler = await _context.Oduncler
                .Include(o => o.Kitap)
                .Include(o => o.Ogrenci)
                .ToListAsync();
            return View(oduncler);
        }

        // GET: Odunc/Create
        public IActionResult Create(int? kitapId = null)
        {
            var kitaplar = _context.Kitaplar.ToList();
            var ogrenciler = _context.Ogrenciler.ToList();
            ViewData["KitapId"] = new SelectList(kitaplar, "Id", "KitapAdi", kitapId);
            ViewData["OgrenciId"] = new SelectList(
                ogrenciler.Select(o => new { o.Id, AdSoyad = o.OgrenciAdi + " " + o.OgrenciSoyadi }),
                "Id", "AdSoyad"
            );
            ViewData["SeciliKitapId"] = kitapId;
            return View();
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
                _context.Add(odunc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var kitaplar = _context.Kitaplar.ToList();
            var ogrenciler = _context.Ogrenciler.ToList();
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
            var odunc = await _context.Oduncler.FindAsync(id);
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
            var kitap = await _context.Kitaplar
                .Include(k => k.Kategori)
                .FirstOrDefaultAsync(k => k.Id == id);
            
            return Json(new { 
                stok = kitap?.StokAdedi ?? 0,
                kategori = kitap?.Kategori?.KategoriAdi ?? ""
            });
        }

        [HttpGet]
        public async Task<JsonResult> GetOgrenciBilgi(int id)
        {
            var ogrenci = await _context.Ogrenciler
                .Include(o => o.Sinif)
                .FirstOrDefaultAsync(o => o.Id == id);
            
            var aktifOdunc = await _context.Oduncler
                .CountAsync(o => o.OgrenciId == id && !o.IadeEdildi);
            
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
            var odunc = await _context.Oduncler.FindAsync(id);
            if (odunc != null)
            {
                _context.Oduncler.Remove(odunc);
                await _context.SaveChangesAsync();
            }
            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction(nameof(Index));
        }
    }
} 