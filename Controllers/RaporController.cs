using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kutuphane.Data;
using Kutuphane.Models;

namespace Kutuphane.Controllers
{
    public class RaporController : Controller
    {
        private readonly ApplicationDbContext _context;
        public RaporController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Rapor/Odunc
        public async Task<IActionResult> Odunc()
        {
            var oduncler = await _context.Oduncler
                .Include(o => o.Kitap)
                .Include(o => o.Ogrenci)
                .ToListAsync();

            // Her ödünç için kalan gün ve gecikme hesaplaması
            var rapor = oduncler.Select(o => new OduncRaporViewModel
            {
                KitapAdi = o.Kitap?.KitapAdi,
                Ogrenci = o.Ogrenci != null ? o.Ogrenci.OgrenciAdi + " " + o.Ogrenci.OgrenciSoyadi : "",
                VerilisTarihi = o.OduncAlmaTarihi,
                SonTeslimTarihi = o.OduncAlmaTarihi.AddDays(15), // 15 gün ödünç süresi varsayalım
                KalanGun = (o.OduncAlmaTarihi.AddDays(15) - DateTime.Now).Days
            }).ToList();

            return View(rapor);
        }
    }

    public class OduncRaporViewModel
    {
        public string? KitapAdi { get; set; }
        public string? Ogrenci { get; set; }
        public DateTime VerilisTarihi { get; set; }
        public DateTime SonTeslimTarihi { get; set; }
        public int KalanGun { get; set; }
    }
} 