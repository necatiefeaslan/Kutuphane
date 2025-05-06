using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kutuphane.Models
{
    public class Kitap
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Kitap adı zorunludur.")]
        [Display(Name = "Kitap Adı")]
        public string? KitapAdi { get; set; }

        [Required(ErrorMessage = "Yazar adı zorunludur.")]
        [Display(Name = "Yazar")]
        public string? Yazar { get; set; }

        [Display(Name = "ISBN")]
        public string? ISBN { get; set; }

        [Display(Name = "Yayın Yılı")]
        public int YayinYili { get; set; }

    
        [Display(Name = "Stok Adedi")]
        public int StokAdedi { get; set; }

        [Display(Name = "Kategori")]
        public int KategoriId { get; set; }

        [ForeignKey("KategoriId")]
        public virtual Kategori? Kategori { get; set; }

        public virtual ICollection<Odunc>? Oduncler { get; set; }
    }
} 