using System.ComponentModel.DataAnnotations;

namespace Kutuphane.Models
{
    public class Sinif
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Sınıf adı zorunludur.")]
        [Display(Name = "Sınıf Adı")]
        public string? SinifAdi { get; set; }

        [Display(Name = "Açıklama")]
        public string? Aciklama { get; set; }

        public virtual ICollection<Ogrenci>? Ogrenciler { get; set; }
        public int UserId { get; set; }
    }
}