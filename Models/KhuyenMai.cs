using System;
using System.ComponentModel.DataAnnotations;

namespace TexasChicken.Models
{
    public class KhuyenMai
    {
        [Key]
        public int MaKM { get; set; }

        [Required]
        [StringLength(255)]
        public string TenKM { get; set; }

        [Required]
        [StringLength(50)]
        public string LoaiKM { get; set; } // 'PhanTram' hoặc 'TienMat'

        [Required]
        [Range(0, double.MaxValue)]
        public decimal GiaTri { get; set; }

        [Required]
        public DateTime NgayBatDau { get; set; }

        [Required]
        public DateTime NgayKetThuc { get; set; }

        public bool TrangThai { get; set; } = true;
    }
}
