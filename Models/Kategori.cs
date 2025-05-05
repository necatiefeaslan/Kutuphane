using System.ComponentModel.DataAnnotations;

namespace Kutuphane.Models
{
    public class Kategori
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategori adı zorunludur.")]
        [Display(Name = "Kategori Adı")]
        public string KategoriAdi { get; set; }

        [Display(Name = "Açıklama")]
        public string? Aciklama { get; set; }

        // Navigation property
        public virtual ICollection<Kitap>? Kitaplar { get; set; }
    }
} 