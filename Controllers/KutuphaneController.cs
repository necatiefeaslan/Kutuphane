using Microsoft.AspNetCore.Mvc;
using Kutuphane.Data;
using Kutuphane.Models;
using System.Linq;

namespace Kutuphane.Controllers
{
    public class KutuphaneController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KutuphaneController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Sinif
        public IActionResult Index()
        {
            var Siniflar = _context.Siniflar.ToList();
            return View(Siniflar);
        }

        // GET: Sinif/Details/5
        public IActionResult Details(int id)
        {
            var Sinif = _context.Siniflar.FirstOrDefault(k => k.Id == id);
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
                _context.Siniflar.Add(Sinif);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(Sinif);
        }

        // GET: Sinif/Edit/5
        public IActionResult Edit(int id)
        {
            var Sinif = _context.Siniflar.FirstOrDefault(k => k.Id == id);
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
            var guncellenecekSinif = _context.Siniflar.FirstOrDefault(k => k.Id == id);
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
            var Sinif = _context.Siniflar.FirstOrDefault(k => k.Id == id);
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
            var Sinif = _context.Siniflar.FirstOrDefault(k => k.Id == id);
            if (Sinif != null)
            {
                _context.Siniflar.Remove(Sinif);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
