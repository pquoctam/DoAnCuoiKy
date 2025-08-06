using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TexasChicken.Models;
using System.Linq;

public class DonHangController : Controller
{
    private readonly TexasContext _context;

    public DonHangController(TexasContext context)
    {
        _context = context;
    }

    // 1. Danh sách đơn hàng
    public IActionResult Index(int page = 1)
    {
        int pageSize = 10;
        int total = _context.DonHang.Count();

        var ds = _context.DonHang
                         .OrderByDescending(d => d.NgayDat)
                         .Skip((page - 1) * pageSize)
                         .Take(pageSize)
                         .ToList();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling((double)total / pageSize);

        return View("~/Views/Admin/DonHang/Index.cshtml", ds);
    }

    // 2. Xem chi tiết đơn hàng
    public IActionResult ChiTiet(int id)
    {
        var don = _context.DonHang
                          .Include(d => d.KhachHang)
                          .Include(d => d.ChiTietDonHang)
                              .ThenInclude(c => c.MonAn)
                          .Include(d => d.ChiTietDonHang)
                              .ThenInclude(c => c.ChiTietDonHangTopping)
                                  .ThenInclude(t => t.Topping)
                          .FirstOrDefault(d => d.MaDonHang == id);

        if (don == null)
        {
            return NotFound();
        }

        return View("~/Views/Admin/DonHang/ChiTiet.cshtml", don);
    }

    // 3. Cập nhật trạng thái đơn hàng (VD: Đã giao, Đã hủy, Đang xử lý)
    // POST: Cập nhật trạng thái
    [HttpPost]
    public IActionResult CapNhatTrangThai(int id, string trangThai)
    {
        var don = _context.DonHang.Find(id);
        if (don != null && !string.IsNullOrEmpty(trangThai))
        {
            don.TrangThai = trangThai;
            _context.SaveChanges();
        }
        return RedirectToAction("Index");
    }

    // POST: Xoá đơn hàng
    [HttpPost]
    public IActionResult Xoa(int id)
    {
        var don = _context.DonHang
                         .Include(d => d.ChiTietDonHang)
                             .ThenInclude(c => c.ChiTietDonHangTopping)
                         .FirstOrDefault(d => d.MaDonHang == id);

        if (don != null)
        {
            // Xoá topping -> chi tiết -> đơn hàng
            foreach (var ct in don.ChiTietDonHang)
            {
                _context.ChiTietDonHangTopping.RemoveRange(ct.ChiTietDonHangTopping);
            }

            _context.ChiTietDonHang.RemoveRange(don.ChiTietDonHang);
            _context.DonHang.Remove(don);
            _context.SaveChanges();
        }

        return RedirectToAction("Index");
    }
}
