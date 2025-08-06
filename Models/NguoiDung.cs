using System;
using System.ComponentModel.DataAnnotations;

namespace TexasChicken.Models
{
    public class NguoiDung
    {
        [Key]
        public int MaND { get; set; }

        [Required(ErrorMessage = "Đây là thông tin bắt buộc")]
        [MinLength(3, ErrorMessage = "Họ phải có ít nhất 3 ký tự")]
        public string Ho { get; set; }

        [Required(ErrorMessage = "Đây là thông tin bắt buộc")]
        [MinLength(3, ErrorMessage = "Tên phải có ít nhất 3 ký tự")]
        public string Ten { get; set; }

        [Required(ErrorMessage = "Đây là thông tin bắt buộc")]
        [RegularExpression(@"^[\w\.-]+@gmail\.com$", ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Đây là thông tin bắt buộc")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$",
        ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự, gồm chữ hoa, chữ thường và số")]
        public string MatKhau { get; set; }

        [Required(ErrorMessage = "Đây là thông tin bắt buộc")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại không hợp lệ. Ví dụ: 0123456789")]
        public string SoDienThoai { get; set; }

        [Required]
        [RegularExpression("Admin|NhanVien", ErrorMessage = "Vai trò chỉ được là Admin hoặc NhanVien")]
        public string VaiTro { get; set; } // Admin, NhanVien

        public DateTime NgayTao { get; set; } = DateTime.Now;
    }
}