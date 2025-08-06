using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TexasChicken.Models
{
    public class ChiTietDonHang
    {
        [Key]
        public int MaChiTiet { get; set; }
        public int MaDonHang { get; set; }
        public int MaMon { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; } // Giá gốc của món
        public decimal TongTien { get; set; } // Tổng tiền = (DonGia + topping) * SoLuong
        public string GhiChuTopping { get; set; }

        [ForeignKey("MaDonHang")]
        public virtual DonHang DonHang { get; set; }

        [ForeignKey("MaMon")]
        public virtual ThucDon MonAn { get; set; }
        public virtual ICollection<ChiTietDonHangTopping> ChiTietDonHangTopping { get; set; }
    }
}
