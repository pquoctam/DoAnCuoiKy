using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TexasChicken.Models
{
    public class KhachHang
    {
        [Key]
        public int MaKH { get; set; }

        [Required(ErrorMessage = "Đây là thông tin bắt buộc")]
        [MinLength(3, ErrorMessage = "Họ phải có ít nhất 3 ký tự")]
        public string Ho { get; set; }

        [Required(ErrorMessage = "Đây là thông tin bắt buộc")]
        [MinLength(3, ErrorMessage = "Tên phải có ít nhất 3 ký tự")]
        public string Ten { get; set; }

        [Required(ErrorMessage = "Đây là thông tin bắt buộc")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại không hợp lệ. Ví dụ: 0123456789")]
        public string SoDienThoai { get; set; }

        [Required(ErrorMessage = "Đây là thông tin bắt buộc")]
        [RegularExpression(@"^[\w\.-]+@gmail\.com$", ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Đây là thông tin bắt buộc")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$",
        ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự, gồm chữ hoa, chữ thường và số")]
        public string MatKhau { get; set; }

        [Required(ErrorMessage = "Đây là thông tin bắt buộc")]
        [Compare("MatKhau", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        [NotMapped]
        public string XacNhanMatKhau { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? NgaySinh { get; set; }

        public DateTime NgayTao { get; set; }

        public DateTime? NgayCapNhat { get; set; }
        public bool TrangThai { get; set; }
    }
}