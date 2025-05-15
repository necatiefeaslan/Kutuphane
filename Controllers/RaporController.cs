using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Kutuphane.Data;
using Kutuphane.Models;
using System.Linq;

namespace Kutuphane.Controllers
{
    public class RaporController : BaseController
    {
        private readonly ApplicationDbContext _context;
        public RaporController(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        // GET: Rapor/Odunc
        public async Task<IActionResult> Odunc(int? sinifId, int? kitapId, int? ogrenciId, bool? enCokKitapAlan = null, 
            bool? enCokKitapOkuyan = null, bool? iadeGunuEnAzKalan = null, bool? teslimEdilmeyenler = null)
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            var query = _context.Oduncler
                .Include(o => o.Kitap)
                .Include(o => o.Ogrenci)
                .ThenInclude(s => s.Sinif)
                .Where(o => o.UserId == userId);

            // Sınıf filtreleme
            if (sinifId.HasValue && sinifId.Value > 0)
            {
                query = query.Where(o => o.Ogrenci.SinifId == sinifId.Value);
            }

            // Kitap filtreleme
            if (kitapId.HasValue && kitapId.Value > 0)
            {
                query = query.Where(o => o.KitapId == kitapId.Value);
            }

            // Öğrenci filtreleme
            if (ogrenciId.HasValue && ogrenciId.Value > 0)
            {
                query = query.Where(o => o.OgrenciId == ogrenciId.Value);
            }

            // İade edilmemiş kitaplar
            if (teslimEdilmeyenler == true)
            {
                query = query.Where(o => o.IadeEdildi == false);
            }

            var oduncler = await query.ToListAsync();

            // En çok kitap alan öğrenciler
            if (enCokKitapAlan == true)
            {
                var enCokAlanOgrenciler = oduncler
                    .GroupBy(o => o.OgrenciId)
                    .Select(g => new { OgrenciId = g.Key, Adet = g.Count() })
                    .OrderByDescending(g => g.Adet)
                    .Select(g => g.OgrenciId)
                    .ToList();

                if (enCokAlanOgrenciler.Any())
                {
                    int maxAdet = oduncler.GroupBy(o => o.OgrenciId).Max(g => g.Count());
                    oduncler = oduncler.Where(o => oduncler.Count(x => x.OgrenciId == o.OgrenciId) == maxAdet).ToList();
                }
            }

            // En çok kitap okuyan öğrenciler (iade edilmiş kitaplar)
            if (enCokKitapOkuyan == true)
            {
                var enCokOkuyanOgrenciler = oduncler
                    .Where(o => o.IadeEdildi)
                    .GroupBy(o => o.OgrenciId)
                    .Select(g => new { OgrenciId = g.Key, Adet = g.Count() })
                    .OrderByDescending(g => g.Adet)
                    .Select(g => g.OgrenciId)
                    .ToList();

                if (enCokOkuyanOgrenciler.Any())
                {
                    int maxOkumaAdet = oduncler.Where(o => o.IadeEdildi).GroupBy(o => o.OgrenciId).Max(g => g.Count());
                    oduncler = oduncler.Where(o => o.IadeEdildi && oduncler.Count(x => x.OgrenciId == o.OgrenciId && x.IadeEdildi) == maxOkumaAdet).ToList();
                }
            }

            // İade günü en az kalan kitaplar
            if (iadeGunuEnAzKalan == true)
            {
                oduncler = oduncler
                    .Where(o => !o.IadeEdildi)
                    .OrderBy(o => o.IadeTarihi ?? o.OduncAlmaTarihi.AddDays(15))
                    .ToList();
            }

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
                IadeEdildi = o.IadeEdildi,
                SinifAdi = o.Ogrenci?.Sinif?.SinifAdi
            }).ToList();

            // ViewBag'e filtreleme için gerekli seçenekleri ekliyoruz
            ViewBag.Siniflar = new SelectList(_context.Siniflar.Where(s => s.UserId == userId), "Id", "SinifAdi");
            ViewBag.Kitaplar = new SelectList(_context.Kitaplar.Where(k => k.UserId == userId), "Id", "KitapAdi");
            ViewBag.Ogrenciler = new SelectList(
                _context.Ogrenciler
                    .Where(o => o.UserId == userId)
                    .Select(o => new { 
                        Id = o.Id, 
                        TamAd = o.OgrenciAdi + " " + o.OgrenciSoyadi + (o.Aktif ? "" : " (Pasif)") 
                    }), 
                "Id", 
                "TamAd");
            
            // Mevcut filtreleme seçeneklerini ViewBag'e ekliyoruz
            ViewBag.SinifId = sinifId;
            ViewBag.KitapId = kitapId;
            ViewBag.OgrenciId = ogrenciId;
            ViewBag.EnCokKitapAlan = enCokKitapAlan;
            ViewBag.EnCokKitapOkuyan = enCokKitapOkuyan;
            ViewBag.IadeGunuEnAzKalan = iadeGunuEnAzKalan;
            ViewBag.TeslimEdilmeyenler = teslimEdilmeyenler;

            return View(rapor);
        }

        // GET: Rapor/GetOgrencilerBySinif
        [HttpGet]
        public JsonResult GetOgrencilerBySinif(int sinifId)
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            var ogrenciler = _context.Ogrenciler
                .Where(o => o.UserId == userId && (sinifId == 0 || o.SinifId == sinifId))
                .Select(o => new { 
                    Id = o.Id, 
                    Ad = o.OgrenciAdi + " " + o.OgrenciSoyadi + (o.Aktif ? "" : " (Pasif)") 
                })
                .OrderBy(o => o.Ad)
                .ToList();
            
            return Json(ogrenciler);
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
        public string? SinifAdi { get; set; }
    }
} 