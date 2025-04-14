using System.ComponentModel.DataAnnotations;

namespace Kutuphane.Models
{
    public class Sinif
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Sınıf adı boş olamaz.")]
        public string SinifAdi { get; set; }
        [Required(ErrorMessage = "Sınıf açıklaması boş olamaz.")]
        public string Aciklama { get; set; }
         public ICollection<Ogrenci>? Ogrenciler { get; set; }
    }
}