using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TexasChicken.Models;
namespace TexasChicken.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly TexasContext _context;

        public TaiKhoanController(TexasContext context)
        {
            _context = context;
        }

        // ==========================
        // ĐĂNG NHẬP CHUNG
        // ==========================
        [HttpPost]
        public IActionResult DangNhap(string Email, string MatKhau)
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(MatKhau))
            {
                TempData["LoiDangNhap"] = "Vui lòng nhập đầy đủ thông tin!";
                TempData["ShowLoginModal"] = "true";
                return RedirectToAction("Index", "Home");
            }

            // 1. Kiểm tra trong bảng NguoiDung (Admin, Nhân viên)
            var nguoiDung = _context.NguoiDung
                .FirstOrDefault(nd => nd.Email == Email && nd.MatKhau == MatKhau);

            if (nguoiDung != null)
            {
                HttpContext.Session.SetString("HoTen", $"{nguoiDung.Ho} {nguoiDung.Ten}");
                HttpContext.Session.SetString("VaiTro", nguoiDung.VaiTro);
                HttpContext.Session.SetInt32("MaND", nguoiDung.MaND);

                if (nguoiDung.VaiTro == "Admin")
                    return RedirectToAction("Index", "Admin");

                if (nguoiDung.VaiTro == "NhanVien")
                    return RedirectToAction("Index", "NhanVien");

                TempData["LoiDangNhap"] = "Bạn không có quyền đăng nhập.";
                TempData["ShowLoginModal"] = "true";
                return RedirectToAction("Index", "Home");
            }

            // 2. Kiểm tra trong bảng KhachHang
            var kh = _context.KhachHang
                .FirstOrDefault(k => (k.Email == Email || k.SoDienThoai == Email) && k.MatKhau == MatKhau);

            if (kh != null)
            {
                HttpContext.Session.SetString("HoTen", $"{kh.Ho} {kh.Ten}");
                HttpContext.Session.SetInt32("MaKH", kh.MaKH);
                HttpContext.Session.SetString("VaiTro", "KhachHang");

                return RedirectToAction("Index", "Home");
            }

            // 3. Không tìm thấy tài khoản nào
            TempData["LoiDangNhap"] = "Sai tài khoản hoặc mật khẩu!";
            TempData["ShowLoginModal"] = "true";
            return RedirectToAction("Index", "Home");
        }

        // ==========================
        // ĐĂNG KÝ KHÁCH HÀNG
        // ==========================
        [HttpGet]
        public IActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DangKy(KhachHang kh)
        {
            if (ModelState.IsValid)
            {
                var exists = _context.KhachHang.Any(x =>
                    x.Email == kh.Email || x.SoDienThoai == kh.SoDienThoai);

                if (exists)
                {
                    ViewBag.ThongBao = "Email hoặc số điện thoại đã tồn tại!";
                    return View(kh);
                }

                kh.NgayTao = DateTime.Now;
                kh.TrangThai = true;
                _context.KhachHang.Add(kh);
                _context.SaveChanges();

                HttpContext.Session.SetString("HoTen", $"{kh.Ho} {kh.Ten}");
                HttpContext.Session.SetInt32("MaKH", kh.MaKH);
                HttpContext.Session.SetString("VaiTro", "KhachHang");

                return RedirectToAction("Index", "Home");
            }

            return View(kh);
        }

        // ==========================
        // ĐĂNG XUẤT
        // ==========================
        public IActionResult DangXuat()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult QuenMatKhau()
        {
            return View();
        }

        [HttpPost]
        public IActionResult QuenMatKhau(string email)
        {
            var kh = _context.KhachHang.FirstOrDefault(k => k.Email == email);
            if (kh == null)
            {
                TempData["LoiQuenMatKhau"] = "Email không tồn tại!";
                return View();
            }

            // 1. Tạo mã OTP 6 chữ số
            var random = new Random();
            var otp = random.Next(100000, 999999).ToString();

            // 2. Lưu OTP và Email vào TempData
            TempData["OTP"] = otp;
            TempData["EmailOTP"] = email;
            TempData["OTPTime"] = DateTime.Now.ToString();

            // 3. Gửi email (bạn sẽ cấu hình SMTP sau)
            Console.WriteLine($"[DEBUG] Mã OTP gửi đến {email}: {otp}");

            return RedirectToAction("XacThucOTP");
        }

        [HttpGet]
        public IActionResult XacThucOTP()
        {
            return View();
        }

        [HttpPost]
        public IActionResult XacThucOTP(string otp)
        {
            var otpSaved = TempData["OTP"]?.ToString();
            var email = TempData["EmailOTP"]?.ToString();
            var time = TempData["OTPTime"]?.ToString();

            if (otpSaved == null || email == null || time == null)
            {
                TempData["LoiOTP"] = "Thông tin không hợp lệ.";
                return RedirectToAction("QuenMatKhau");
            }

            var sentTime = DateTime.Parse(time);
            if ((DateTime.Now - sentTime).TotalMinutes > 5)
            {
                TempData["LoiOTP"] = "Mã OTP đã hết hạn (quá 5 phút)";
                return RedirectToAction("QuenMatKhau");
            }

            if (otp != otpSaved)
            {
                TempData["LoiOTP"] = "Mã OTP không chính xác!";
                return RedirectToAction("XacThucOTP");
            }

            TempData["EmailReset"] = email;
            return RedirectToAction("DatLaiMatKhau");
        }
        [HttpGet]
        public IActionResult DatLaiMatKhau()
        {
            if (TempData["EmailReset"] == null)
                return RedirectToAction("QuenMatKhau");

            ViewBag.Email = TempData["EmailReset"];
            return View();
        }
        [HttpPost]
        public IActionResult DatLaiMatKhau(string email, string matKhauMoi)
        {
            var kh = _context.KhachHang.FirstOrDefault(k => k.Email == email);
            if (kh != null)
            {
                kh.MatKhau = matKhauMoi; // Bạn có thể băm mật khẩu ở đây
                _context.SaveChanges();

                TempData["ThongBao"] = "Mật khẩu đã được cập nhật!";
            }

            return RedirectToAction("Index", "Home");
        }
    }
}