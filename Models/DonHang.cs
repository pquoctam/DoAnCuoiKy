using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TexasChicken.Models
{
    public class DonHang
    {
        [Key]
        public int MaDonHang { get; set; }

        public int MaKH { get; set; }
        public DateTime NgayDat { get; set; }

        public string HoTen { get; set; }
        public string SoDienThoai { get; set; }
        public string PhuongThucGiaoHang { get; set; }
        public string? TrangThai { get; set; }
        public decimal TongTien { get; set; }
        public string DiaChiGiaoHang { get; set; }
        public string? GhiChu { get; set; }

        public int? MaKM { get; set; }
        public decimal? GiamGia { get; set; }

        [ForeignKey("MaKM")]
        public KhuyenMai KhuyenMai { get; set; }

        // Quan hệ
        [ForeignKey("MaKH")]
        public virtual KhachHang KhachHang { get; set; }
        public virtual ICollection<ChiTietDonHang> ChiTietDonHang { get; set; }
    }
}
