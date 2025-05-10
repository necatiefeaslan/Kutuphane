using System.ComponentModel.DataAnnotations;

namespace Kutuphane.Models
{
    public class Ogrenci{
        public int Id { get; set; }
        public string OgrenciAdi { get; set; } = string.Empty;
        public string OgrenciSoyadi { get; set; } = string.Empty;
        public string OkulNumarasi { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Lütfen bir kategori seçin.")]
        public int SinifId { get; set; } 
        public Sinif? Sinif { get; set; } 

        public DateTime EklenmeTarihi { get; set; } = DateTime.Now;
        public int UserId { get; set; }
    }
}