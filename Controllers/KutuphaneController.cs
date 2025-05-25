using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kutuphane.Data;
using Kutuphane.Models;
using System.Linq;

namespace Kutuphane.Controllers
{
    public class KutuphaneController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public KutuphaneController(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        // GET: Sinif
        public IActionResult Index()
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            var Siniflar = _context.Siniflar.Where(s => s.UserId == userId && s.Aktif == true).ToList();
            return View(Siniflar);
        }

        // GET: Sinif/Details/5
        public IActionResult Details(int id)
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            var Sinif = _context.Siniflar.FirstOrDefault(k => k.Id == id && k.UserId == userId);
            if (Sinif == null)
            {
                return NotFound();
            }
            return View(Sinif);
        }

        // GET: Sinif/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Sinif/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Sinif Sinif)
        {
            if (ModelState.IsValid)
            {
                Sinif.UserId = int.Parse(HttpContext.Session.GetString("UserId"));
                _context.Siniflar.Add(Sinif);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(Sinif);
        }

        // GET: Sinif/Edit/5
        public IActionResult Edit(int id)
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            var Sinif = _context.Siniflar.FirstOrDefault(k => k.Id == id && k.UserId == userId);
            if (Sinif == null)
            {
                return NotFound();
            }
            return View(Sinif);
        }

        // POST: Sinif/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Sinif Sinif)
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            var guncellenecekSinif = _context.Siniflar.FirstOrDefault(k => k.Id == id && k.UserId == userId);
            if (guncellenecekSinif != null)
            {
                guncellenecekSinif.SinifAdi = Sinif.SinifAdi;
                guncellenecekSinif.Aciklama = Sinif.Aciklama;
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Sinif/Delete/5
        public IActionResult Delete(int id)
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            var Sinif = _context.Siniflar.FirstOrDefault(k => k.Id == id && k.UserId == userId);
            if (Sinif == null)
            {
                return NotFound();
            }
            return View(Sinif);
        }

        // POST: Sinif/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            var Sinif = _context.Siniflar.FirstOrDefault(k => k.Id == id && k.UserId == userId);
            if (Sinif != null)
            {
                // Soft delete - sınıfı gerçekten silmek yerine Aktif alanını false yap
                Sinif.Aktif = false;
                _context.Update(Sinif);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
