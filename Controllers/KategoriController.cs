using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kutuphane.Models;
using Kutuphane.Data;

namespace Kutuphane.Controllers
{
    public class KategoriController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KategoriController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Kategori
        public async Task<IActionResult> Index()
        {
            return View(await _context.Kategoriler.ToListAsync());
        }

        // GET: Kategori/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Kategori/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("KategoriAdi,Aciklama")] Kategori kategori)
        {
            if (ModelState.IsValid)
            {
                _context.Add(kategori);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(kategori);
        }

        // GET: Kategori/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kategori = await _context.Kategoriler.FindAsync(id);
            if (kategori == null)
            {
                return NotFound();
            }
            return View(kategori);
        }

        // POST: Kategori/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,KategoriAdi,Aciklama")] Kategori kategori)
        {
            if (id != kategori.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(kategori);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KategoriExists(kategori.Id))
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
            return View(kategori);
        }

        // GET: Kategori/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kategori = await _context.Kategoriler
                .FirstOrDefaultAsync(m => m.Id == id);
            if (kategori == null)
            {
                return NotFound();
            }

            return View(kategori);
        }

        // POST: Kategori/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var kategori = await _context.Kategoriler.FindAsync(id);
            if (kategori != null)
            {
                _context.Kategoriler.Remove(kategori);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Kategori/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var kategori = await _context.Kategoriler.FirstOrDefaultAsync(k => k.Id == id);
            if (kategori == null)
            {
                return NotFound();
            }
            return View(kategori);
        }

        private bool KategoriExists(int id)
        {
            return _context.Kategoriler.Any(e => e.Id == id);
        }
    }
} 