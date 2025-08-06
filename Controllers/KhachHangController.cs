using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TexasChicken.Models;
using System;
using System.Linq;

public class KhachHangController : Controller
{
    private readonly TexasContext _context;

    public KhachHangController(TexasContext context)
    {
        _context = context;
    }

    // 1. Hiển thị danh sách có phân trang
    public IActionResult Index(int page = 1)
    {
        int pageSize = 10;
        int totalItems = _context.KhachHang.Count(); // Không lọc theo TrangThai để xem cả bị khóa

        var ds = _context.KhachHang
                         .OrderBy(kh => kh.MaKH)
                         .Skip((page - 1) * pageSize)
                         .Take(pageSize)
                         .ToList();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        return View("~/Views/Admin/KhachHang/Index.cshtml", ds);
    }

    // 2. Hiển thị form chỉnh sửa
    [HttpGet]
    public IActionResult Sua(int id)
    {
        var kh = _context.KhachHang.FirstOrDefault(k => k.MaKH == id);
        if (kh == null) return NotFound();

        return View("~/Views/Admin/KhachHang/Sua.cshtml", kh);
    }

    // 3. Xử lý chỉnh sửa
    [HttpPost]
    public async Task<IActionResult> Sua(KhachHang model)
    {
        ModelState.Remove("XacNhanMatKhau");
        if (!ModelState.IsValid)
        {
            return View("~/Views/Admin/KhachHang/Sua.cshtml", model);
        }

        var existing = await _context.KhachHang.FindAsync(model.MaKH);
        if (existing == null) return NotFound();

        existing.Ho = model.Ho;
        existing.Ten = model.Ten;
        existing.Email = model.Email;
        existing.SoDienThoai = model.SoDienThoai;
        existing.NgaySinh = model.NgaySinh;
        existing.TrangThai = model.TrangThai;
        existing.NgayCapNhat = DateTime.UtcNow.AddHours(7);

        await _context.SaveChangesAsync();
        TempData["msg"] = "✅ Đã cập nhật thông tin khách hàng!";
        return RedirectToAction("Index");
    }

    // 4. Xoá mềm (khóa tài khoản)
    public IActionResult Xoa(int id)
    {
        var kh = _context.KhachHang.Find(id);
        if (kh != null)
        {
            kh.TrangThai = false;
            kh.NgayCapNhat = DateTime.UtcNow.AddHours(7);
            _context.SaveChanges();
            TempData["msg"] = "❌ Tài khoản đã bị khóa!";
        }
        return RedirectToAction("Index");
    }
}
