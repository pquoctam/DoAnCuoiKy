using System.ComponentModel.DataAnnotations;

namespace TexasChicken.Models
{
    public class Topping
    {
        [Key]
        public int MaTopping { get; set; }

        [Required]
        [StringLength(100)]
        public string TenTopping { get; set; }

        [Range(0, double.MaxValue)]
        public decimal GiaTopping { get; set; }

        [StringLength(50)]
        public string Loai { get; set; }

        [StringLength(255)]
        public string HinhAnh { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.UtcNow.AddHours(7);

        public DateTime? NgayCapNhat { get; set; }

        public bool TrangThai { get; set; } = true;

        public ICollection<MonAn_Topping> MonAn_Topping { get; set; }
    }
}
