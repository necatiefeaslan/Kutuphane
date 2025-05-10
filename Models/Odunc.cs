using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kutuphane.Models
{
    public class Odunc
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Kitap")]
        public int KitapId { get; set; }
        [ForeignKey("KitapId")]
        public virtual Kitap? Kitap { get; set; }

        [Required]
        [Display(Name = "Öğrenci")]
        public int OgrenciId { get; set; }
        [ForeignKey("OgrenciId")]
        public virtual Ogrenci? Ogrenci { get; set; }

        [Required]
        [Display(Name = "Ödünç Alma Tarihi")]
        public DateTime OduncAlmaTarihi { get; set; }

        [Display(Name = "İade Tarihi")]
        public DateTime? IadeTarihi { get; set; }

        [Display(Name = "Durum")]
        public bool IadeEdildi { get; set; } // false: ödünç verildi, true: iade edildi

        public int UserId { get; set; }
    }
} 