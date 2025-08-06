using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TexasChicken.Models
{
    public class NhaHang
    {
        [Key]
        public int MaNH { get; set; }

        [Required]
        public string TenNH { get; set; }

        public string DiaChi { get; set; }

        [ForeignKey("QuanHuyen")]
        public int MaQH { get; set; }

        public QuanHuyen QuanHuyen { get; set; }

        public string SoDienThoai { get; set; }

        public string Email { get; set; }

        public string GioHoatDong { get; set; }

        // Nếu bạn muốn sau này có thể thêm Tọa độ GPS:
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
