using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Kutuphane.Models;
using Kutuphane.Data;

namespace Kutuphane.Controllers
{
    public class KitapController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KitapController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Kitap
        public async Task<IActionResult> Index()
        {
            var kitaplar = await _context.Kitaplar
                .Include(k => k.Kategori)
                .ToListAsync();
            return View(kitaplar);
        }

        // GET: Kitap/Create
        public IActionResult Create()
        {
            ViewData["KategoriId"] = new SelectList(_context.Kategoriler, "Id", "KategoriAdi");
            return View();
        }

        // POST: Kitap/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("KitapAdi,Yazar,ISBN,YayinYili,StokAdedi,KategoriId")] Kitap kitap)
        {
            if (ModelState.IsValid)
            {
                _context.Add(kitap);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["KategoriId"] = new SelectList(_context.Kategoriler, "Id", "KategoriAdi", kitap.KategoriId);
            return View(kitap);
        }

        // GET: Kitap/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kitap = await _context.Kitaplar.FindAsync(id);
            if (kitap == null)
            {
                return NotFound();
            }
            ViewData["KategoriId"] = new SelectList(_context.Kategoriler, "Id", "KategoriAdi", kitap.KategoriId);
            return View(kitap);
        }

        // POST: Kitap/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,KitapAdi,Yazar,ISBN,YayinYili,StokAdedi,KategoriId")] Kitap kitap)
        {
            if (id != kitap.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(kitap);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KitapExists(kitap.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["KategoriId"] = new SelectList(_context.Kategoriler, "Id", "KategoriAdi", kitap.KategoriId);
            return View(kitap);
        }

        // GET: Kitap/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kitap = await _context.Kitaplar
                .Include(k => k.Kategori)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (kitap == null)
            {
                return NotFound();
            }

            return View(kitap);
        }

        // POST: Kitap/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var kitap = await _context.Kitaplar.FindAsync(id);
            if (kitap != null)
            {
                _context.Kitaplar.Remove(kitap);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Kitap/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var kitap = await _context.Kitaplar.Include(k => k.Kategori).FirstOrDefaultAsync(k => k.Id == id);
            if (kitap == null)
            {
                return NotFound();
            }
            return View(kitap);
        }

        private bool KitapExists(int id)
        {
            return _context.Kitaplar.Any(e => e.Id == id);
        }
    }
} 