using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TexasChicken.Models
{
    public class MaGiamGia
    {
        [Key]
        [StringLength(50)]
        public string MaCode { get; set; }

        [Required]
        public int MaKM { get; set; }

        [ForeignKey("MaKM")]
        public KhuyenMai KhuyenMai { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int SoLuong { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;
    }
}
