using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Kutuphane.Models
{
    public class Sinif
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Sınıf adı zorunludur.")]
        [Display(Name = "Sınıf Adı")]
        [RegularExpression(@"^([1-9]|1[0-2])(-[A-Z])?$", ErrorMessage = "Sınıf adı 1-12 arasında olmalı ve opsiyonel olarak şube içermelidir (Örn: 9 veya 9-A).")]
        [SinifAdiDogrulama(ErrorMessage = "Sınıf adı 1-12 arasında olmalıdır.")]
        public string? SinifAdi { get; set; }

        [Display(Name = "Açıklama")]
        public string? Aciklama { get; set; }

        [Display(Name = "Aktif")]
        public bool Aktif { get; set; } = true;

        public virtual ICollection<Ogrenci>? Ogrenciler { get; set; }
        public int UserId { get; set; }
    }

    public class SinifAdiDogrulamaAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
                return ValidationResult.Success;

            string sinifAdi = value.ToString()!;
            
            // Sadece rakam içeren kısmı al
            string sinifNumarasi = sinifAdi;
            
            // Eğer şube içeriyorsa (örn: 9-A), rakam kısmını al
            if (sinifAdi.Contains('-'))
            {
                sinifNumarasi = sinifAdi.Split('-')[0];
            }
            
            // Sayıya dönüştürülebiliyor mu ve 1-12 arasında mı kontrol et
            if (int.TryParse(sinifNumarasi, out int sayi) && sayi >= 1 && sayi <= 12)
            {
                return ValidationResult.Success;
            }
            
            return new ValidationResult("Sınıf adı 1-12 arasında bir sayı olmalıdır.");
        }
    }
}