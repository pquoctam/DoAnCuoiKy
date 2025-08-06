using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TexasChicken.Models;
using System.Collections.Generic;
using System.Linq;

namespace TexasChicken.Controllers
{
    public class GioHangController : Controller
    {
        private readonly TexasContext _context;


        public GioHangController(TexasContext context)
        {
            _context = context;
        }

        // GET: /GioHang
        public IActionResult Index()
        {
            int? maKH = HttpContext.Session.GetInt32("MaKH");
            if (maKH == null)
            {
                TempData["ShowLoginModal"] = "true";
                return Redirect(Request.Headers["Referer"].ToString());
            }

            var khachHang = _context.KhachHang.FirstOrDefault(k => k.MaKH == maKH);
            if (khachHang == null)
                return NotFound();

            var cart = _context.Cart
    .Include(c => c.CartItem)
        .ThenInclude(ci => ci.MaMonNavigation)
    .Include(c => c.CartItem)
        .ThenInclude(ci => ci.CartItemTopping)
            .ThenInclude(cit => cit.MaToppingNavigation)
    .FirstOrDefault(c => c.MaKH == maKH);


            if (cart == null || !cart.CartItem.Any())
            {
                ViewBag.ThongBao = "Giỏ hàng của bạn đang trống.";
                return View(new Cart());
            }

            return View(cart);
        }
        public IActionResult Them(int maMon, int quantity, List<int> topping)
        {
            var maKH = HttpContext.Session.GetInt32("MaKH");
            if (maKH == null)
                return RedirectToAction("DangNhap", "TaiKhoan");

            // Tìm hoặc tạo Cart
            var cart = _context.Cart.FirstOrDefault(c => c.MaKH == maKH);
            if (cart == null)
            {
                cart = new Cart
                {
                    MaKH = maKH.Value,
                    CreatedAt = DateTime.Now 
                };
                _context.Cart.Add(cart);
                _context.SaveChanges();
            }

            // Tìm CartItem có cùng món và các topping giống nhau
            var existingCartItems = _context.CartItem
                .Include(ci => ci.CartItemTopping)
                .Where(ci => ci.CartId == cart.CartId && ci.MaMon == maMon)
                .ToList();

            CartItem foundItem = null;

            foreach (var item in existingCartItems)
            {
                var existingToppingIds = item.CartItemTopping.Select(t => t.MaTopping).OrderBy(t => t);
                var newToppingIds = (topping ?? new List<int>()).OrderBy(t => t);

                if (existingToppingIds.SequenceEqual(newToppingIds))
                {
                    foundItem = item;
                    break;
                }
            }

            if (foundItem != null)
            {
                // Nếu tồn tại, chỉ tăng số lượng
                foundItem.SoLuong += quantity;
                _context.SaveChanges();
            }
            else
            {
                // Nếu chưa có, thêm mới
                var cartItem = new CartItem
                {
                    CartId = cart.CartId,
                    MaMon = maMon,
                    SoLuong = quantity
                };
                _context.CartItem.Add(cartItem);
                _context.SaveChanges();

                if (topping != null && topping.Any())
                {
                    foreach (var t in topping)
                    {
                        _context.CartItemTopping.Add(new CartItemTopping
                        {
                            CartItemId = cartItem.CartItemId,
                            MaTopping = t
                        });
                    }
                    _context.SaveChanges();
                }
            }

            // Cập nhật số lượng giỏ hàng (tổng số lượng món)
            int soLuong = _context.CartItem
                .Where(c => c.CartId == cart.CartId)
                .Sum(c => c.SoLuong);
            HttpContext.Session.SetInt32("CartCount", soLuong);

            return Redirect(Request.Headers["Referer"].ToString());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult XoaCartItem([FromBody] XoaCartItemRequest data)
        {
            int? maKH = HttpContext.Session.GetInt32("MaKH");
            if (maKH == null)
                return Unauthorized();

            var cartItem = _context.CartItem
                .Include(ci => ci.CartItemTopping)
                .FirstOrDefault(c => c.CartItemId == data.CartItemId);

            if (cartItem != null)
            {
                _context.CartItemTopping.RemoveRange(cartItem.CartItemTopping);
                _context.CartItem.Remove(cartItem);
                _context.SaveChanges();

                // Cập nhật lại số lượng session
                var cart = _context.Cart
                    .Include(c => c.CartItem)
                    .FirstOrDefault(c => c.MaKH == maKH);

                if (cart != null)
                {
                    int soLuong = cart.CartItem.Sum(ci => ci.SoLuong);
                    HttpContext.Session.SetInt32("CartCount", soLuong);
                }

                return Ok();
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult CapNhatSoLuong([FromBody] CapNhatSoLuongRequest data)
        {
            var maKH = HttpContext.Session.GetInt32("MaKH");
            if (maKH == null) return Unauthorized();

            var cartItem = _context.CartItem.FirstOrDefault(ci => ci.CartItemId == data.CartItemId);
            if (cartItem != null && data.SoLuongMoi > 0)
            {
                cartItem.SoLuong = data.SoLuongMoi;
                _context.SaveChanges();

                // Cập nhật session CartCount
                var cart = _context.Cart.Include(c => c.CartItem).FirstOrDefault(c => c.MaKH == maKH);
                if (cart != null)
                {
                    int soLuong = cart.CartItem.Sum(ci => ci.SoLuong);
                    HttpContext.Session.SetInt32("CartCount", soLuong);
                }

                return Ok();
            }

            return NotFound();
        }

        public class CapNhatSoLuongRequest
        {
            public int CartItemId { get; set; }
            public int SoLuongMoi { get; set; }
        }


        public class XoaCartItemRequest
        {
            public int CartItemId { get; set; }
        }
        public IActionResult DatHang()
        {
            var maKH = HttpContext.Session.GetInt32("MaKH");
            if (maKH == null)
                return RedirectToAction("DangNhap", "TaiKhoan");

            var cart = _context.Cart
                .Include(c => c.CartItem)
                    .ThenInclude(i => i.MaMonNavigation)
                .Include(c => c.CartItem)
                    .ThenInclude(i => i.CartItemTopping)
                        .ThenInclude(tp => tp.MaToppingNavigation)
                .FirstOrDefault(c => c.MaKH == maKH);

            if (cart == null || !cart.CartItem.Any())
                return RedirectToAction("Index", "ThucDon");

            // Lấy thông tin khách hàng
            var khachHang = _context.KhachHang.FirstOrDefault(k => k.MaKH == maKH);
            if (khachHang != null)
            {
                ViewBag.HoTen = $"{khachHang.Ho} {khachHang.Ten}";
                ViewBag.SoDienThoai = khachHang.SoDienThoai;
            }

            // Lấy danh sách voucher hợp lệ
            var vouchers = _context.MaGiamGia
                .Include(m => m.KhuyenMai)
                .Where(m =>
                    m.SoLuong > 0 &&
                    m.KhuyenMai.TrangThai == true &&
                    m.KhuyenMai.NgayBatDau <= DateTime.Now &&
                    m.KhuyenMai.NgayKetThuc >= DateTime.Now
                ).ToList();

            ViewBag.VoucherList = vouchers;

            return View("DatHang", cart);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThanhToan(DonHangViewModel model, string MaCoupon)
        {
            var maKH = HttpContext.Session.GetInt32("MaKH");
            if (maKH == null)
                return RedirectToAction("DangNhap", "TaiKhoan");

            var cart = _context.Cart
                .Include(c => c.CartItem)
                    .ThenInclude(i => i.MaMonNavigation)
                .Include(c => c.CartItem)
                    .ThenInclude(i => i.CartItemTopping)
                        .ThenInclude(tp => tp.MaToppingNavigation)
                .FirstOrDefault(c => c.MaKH == maKH);

            if (cart == null || !cart.CartItem.Any())
            {
                TempData["Error"] = "Giỏ hàng trống!";
                return RedirectToAction("Index");
            }

            // Tính tổng tiền trước khuyến mãi
            decimal tongTien = cart.CartItem.Sum(item =>
                ((item.MaMonNavigation?.GiaBan ?? 0) +
                (item.CartItemTopping?.Sum(t => t.MaToppingNavigation?.GiaTopping ?? 0) ?? 0)) * item.SoLuong
            );

            // Xử lý mã coupon
            decimal giamGia = 0;
            int? maKM = null;

            if (!string.IsNullOrEmpty(MaCoupon))
            {
                var coupon = _context.MaGiamGia
                    .Include(m => m.KhuyenMai)
                    .FirstOrDefault(m => m.MaCode == MaCoupon && m.SoLuong > 0);

                if (coupon != null)
                {
                    var km = coupon.KhuyenMai;
                    if (km.TrangThai && km.NgayBatDau <= DateTime.Now && km.NgayKetThuc >= DateTime.Now)
                    {
                        if (km.LoaiKM == "PhanTram")
                            giamGia = tongTien * ((decimal)km.GiaTri / 100);
                        else if (km.LoaiKM == "TienMat")
                            giamGia = km.GiaTri;

                        // Giới hạn giảm không vượt quá tổng tiền
                        giamGia = Math.Min(giamGia, tongTien);
                        maKM = km.MaKM;

                        // Trừ số lượng mã còn lại
                        coupon.SoLuong -= 1;
                        _context.MaGiamGia.Update(coupon);
                    }
                }
            }

            // Tạo đơn hàng
            var donHang = new DonHang
            {
                MaKH = maKH.Value,
                HoTen = model.HoTen,
                SoDienThoai = model.SoDienThoai,
                DiaChiGiaoHang = model.DiaChiGiaoHang,
                GhiChu = model.GhiChu,
                PhuongThucGiaoHang = model.PhuongThucGiaoHang,
                NgayDat = DateTime.Now,
                TongTien = tongTien - giamGia,
                MaKM = maKM,
                GiamGia = giamGia
            };

            _context.DonHang.Add(donHang);
            _context.SaveChanges();

            // Lưu chi tiết đơn hàng
            // Lưu chi tiết đơn hàng
            foreach (var item in cart.CartItem)
            {
                var giaMon = item.MaMonNavigation?.GiaBan ?? 0;
                var giaTopping = item.CartItemTopping?.Sum(t => t.MaToppingNavigation?.GiaTopping ?? 0) ?? 0;

                var chiTiet = new ChiTietDonHang
                {
                    MaDonHang = donHang.MaDonHang,
                    MaMon = item.MaMon,
                    SoLuong = item.SoLuong,
                    DonGia = giaMon,
                    TongTien = (giaMon + giaTopping) * item.SoLuong,
                    ChiTietDonHangTopping = item.CartItemTopping?.Select(t => new ChiTietDonHangTopping
                    {
                        MaTopping = t.MaTopping
                    }).ToList()
                };

                _context.ChiTietDonHang.Add(chiTiet);
            }
            _context.SaveChanges();

            // XÓA GIỎ HÀNG
            var cartItems = _context.CartItem
                .Include(ci => ci.CartItemTopping)
                .Where(ci => ci.CartId == cart.CartId)
                .ToList();

            foreach (var item in cartItems)
            {
                _context.CartItemTopping.RemoveRange(item.CartItemTopping);
            }
            _context.CartItem.RemoveRange(cartItems);
            _context.Cart.Remove(cart);
            _context.SaveChanges();

            // Xoá session đếm giỏ hàng
            HttpContext.Session.Remove("CartCount");

            TempData["DatHangThanhCong"] = "Đặt hàng thành công!";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult ApDungCoupon(string MaCoupon)
        {
            var maKH = HttpContext.Session.GetInt32("MaKH");
            if (maKH == null)
                return RedirectToAction("DangNhap", "TaiKhoan");

            var coupon = _context.MaGiamGia
                .Include(m => m.KhuyenMai)
                .FirstOrDefault(m => m.MaCode == MaCoupon && m.SoLuong > 0);

            if (coupon == null || coupon.KhuyenMai == null)
            {
                TempData["MaCouponError"] = "Mã không hợp lệ hoặc đã hết hạn.";
                HttpContext.Session.Remove("MaCoupon");
                HttpContext.Session.Remove("MaKM");
                HttpContext.Session.Remove("GiamGia");
                HttpContext.Session.Remove("LoaiKM");
                HttpContext.Session.Remove("GiaTriKM");
            }
            else
            {
                var km = coupon.KhuyenMai;

                if (!km.TrangThai || km.NgayBatDau > DateTime.Now || km.NgayKetThuc < DateTime.Now)
                {
                    TempData["MaCouponError"] = "Mã không hợp lệ hoặc đã hết hạn.";
                    HttpContext.Session.Remove("MaCoupon");
                    HttpContext.Session.Remove("MaKM");
                    HttpContext.Session.Remove("GiamGia");
                    HttpContext.Session.Remove("LoaiKM");
                    HttpContext.Session.Remove("GiaTriKM");
                }
                else
                {
                    // Lưu thông tin mã giảm giá vào session
                    HttpContext.Session.SetString("MaCoupon", MaCoupon);
                    HttpContext.Session.SetInt32("MaKM", km.MaKM);

                    // Lưu loại và giá trị khuyến mãi dạng chuỗi
                    HttpContext.Session.SetString("LoaiKM", km.LoaiKM); // "PhanTram" hoặc "TienMat"
                    HttpContext.Session.SetString("GiaTriKM", km.GiaTri.ToString());

                    TempData["MaCouponSuccess"] = km.LoaiKM == "PhanTram"
                        ? $"Áp dụng thành công: giảm {km.GiaTri:0.#}%"
                        : $"Áp dụng thành công: giảm {((decimal)km.GiaTri):N0} đ";
                }
            }

            return RedirectToAction("DatHang");
        }
    }
}
