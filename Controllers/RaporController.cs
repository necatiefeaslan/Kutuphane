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
                Id = o.Id,
                KitapAdi = o.Kitap?.KitapAdi,
                Ogrenci = o.Ogrenci != null ? o.Ogrenci.OgrenciAdi + " " + o.Ogrenci.OgrenciSoyadi : "",
                VerilisTarihi = o.OduncAlmaTarihi,
                SonTeslimTarihi = o.IadeTarihi ?? o.OduncAlmaTarihi.AddDays(15),
                KalanGun = o.IadeTarihi.HasValue
                    ? (int)Math.Ceiling((o.IadeTarihi.Value.Date - DateTime.Now.Date).TotalDays)
                    : (int)Math.Ceiling((o.OduncAlmaTarihi.AddDays(15).Date - DateTime.Now.Date).TotalDays),
                IadeEdildi = o.IadeEdildi
            }).ToList();

            return View(rapor);
        }
    }

    public class OduncRaporViewModel
    {
        public int Id { get; set; }
        public string? KitapAdi { get; set; }
        public string? Ogrenci { get; set; }
        public DateTime VerilisTarihi { get; set; }
        public DateTime? SonTeslimTarihi { get; set; }
        public int KalanGun { get; set; }
        public bool IadeEdildi { get; set; }
    }
} 