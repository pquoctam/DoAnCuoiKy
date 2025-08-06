using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TexasChicken.Models
{
    public class ThucDon
    {
        [Key]
        public int MaMon { get; set; }

        [Required(ErrorMessage = "Tên món không được để trống.")]
        [Display(Name = "Tên Món")]
        public string TenMon { get; set; }

        [Display(Name = "Giá Gốc")]
        public decimal GiaGoc { get; set; }

        [Required(ErrorMessage = "Giá bán không được để trống.")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá bán phải lớn hơn hoặc bằng 0.")]
        [Display(Name = "Giá Bán")]
        public decimal GiaBan { get; set; }

        [Display(Name = "Mô Tả")]
        public string? MoTa { get; set; }

        [Display(Name = "Hình Ảnh")]
        public string HinhAnh { get; set; }

        [ForeignKey("LoaiMon")]
        [Required(ErrorMessage = "Vui lòng chọn loại món.")]
        public int MaLoai { get; set; }

        [Display(Name = "Loại Món")]
        public LoaiMon LoaiMon { get; set; }

        public bool TrangThai { get; set; } = true;

        public DateTime NgayTao { get; set; } = DateTime.UtcNow.AddHours(7);

        public DateTime? NgayCapNhat { get; set; } = DateTime.UtcNow.AddHours(7);

        public ICollection<MonAn_Topping> MonAn_Topping { get; set; }
    }

}