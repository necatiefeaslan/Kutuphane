using Microsoft.EntityFrameworkCore;
using Kutuphane.Models;

namespace Kutuphane.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Sinif> Siniflar { get; set; }
        public DbSet<Ogrenci> Ogrenciler { get; set; }
        public DbSet<Kitap> Kitaplar { get; set; }
        public DbSet<Kategori> Kategoriler { get; set; }
        public DbSet<Odunc> Oduncler { get; set; }
        public DbSet<User> Users { get; set; }
    }
}