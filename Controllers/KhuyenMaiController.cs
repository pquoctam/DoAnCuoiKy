using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TexasChicken.Models;

public class KhuyenMaiController : Controller
{
    private readonly TexasContext _context;

    public KhuyenMaiController(TexasContext context)
    {
        _context = context;
    }

    public IActionResult Index(int page = 1)
    {
        int pageSize = 10;
        int totalItems = _context.KhuyenMai.Count();

        var ds = _context.KhuyenMai
                         .OrderByDescending(k => k.MaKM)
                         .Skip((page - 1) * pageSize)
                         .Take(pageSize)
                         .ToList();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        return View("~/Views/Admin/KhuyenMai/Index.cshtml", ds);
    }

    [HttpGet]
    public IActionResult Them()
    {
        return View("~/Views/Admin/KhuyenMai/Them.cshtml");
    }

    [HttpPost]
    public IActionResult Them(KhuyenMai km)
    {
        // Kiểm tra loại khuyến mãi
        if (km.LoaiKM == "PhanTram")
        {
            // Giá trị phải là số nguyên và từ 1 đến 100
            if (km.GiaTri % 1 != 0)
            {
                ModelState.AddModelError("GiaTri", "Giá trị phần trăm phải là số nguyên.");
            }
            else if (km.GiaTri < 1 || km.GiaTri > 100)
            {
                ModelState.AddModelError("GiaTri", "Phần trăm phải nằm trong khoảng 1 đến 100.");
            }
        }
        else if (km.LoaiKM == "TienMat")
        {
            if (km.GiaTri < 1000)
            {
                ModelState.AddModelError("GiaTri", "Số tiền khuyến mãi phải từ 1.000 VNĐ trở lên.");
            }
        }
        else
        {
            ModelState.AddModelError("LoaiKM", "Vui lòng chọn loại khuyến mãi.");
        }

        // Kiểm tra ngày
        if (km.NgayKetThuc <= km.NgayBatDau)
        {
            ModelState.AddModelError("NgayKetThuc", "Ngày kết thúc phải sau ngày bắt đầu.");
        }

        if (!ModelState.IsValid)
        {
            return View("~/Views/Admin/KhuyenMai/Them.cshtml", km);
        }

        km.TrangThai = true;
        _context.KhuyenMai.Add(km);
        _context.SaveChanges();
        TempData["msg"] = "Thêm khuyến mãi thành công!";
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Sua(int id)
    {
        var km = _context.KhuyenMai.FirstOrDefault(k => k.MaKM == id);
        if (km == null) return NotFound();
        return View("~/Views/Admin/KhuyenMai/Sua.cshtml", km);
    }

    [HttpPost]
    public IActionResult Sua(KhuyenMai km)
    {
        // Kiểm tra loại khuyến mãi là phần trăm
        if (km.LoaiKM == "PhanTram")
        {
            // Giá trị phải là số nguyên và trong khoảng 1-100
            if (km.GiaTri % 1 != 0)
            {
                ModelState.AddModelError("GiaTri", "Phần trăm khuyến mãi phải là số nguyên.");
            }
            else if (km.GiaTri < 1 || km.GiaTri > 100)
            {
                ModelState.AddModelError("GiaTri", "Phần trăm khuyến mãi phải từ 1 đến 100.");
            }
        }
        else if (km.LoaiKM == "TienMat")
        {
            if (km.GiaTri < 1000)
            {
                ModelState.AddModelError("GiaTri", "Tiền mặt khuyến mãi phải lớn hơn hoặc bằng 1.000 VNĐ.");
            }
        }

        // Kiểm tra ngày bắt đầu và ngày kết thúc
        if (km.NgayKetThuc <= km.NgayBatDau)
        {
            ModelState.AddModelError("NgayKetThuc", "Ngày kết thúc phải sau ngày bắt đầu.");
        }

        if (!ModelState.IsValid)
        {
            return View("~/Views/Admin/KhuyenMai/Sua.cshtml", km);
        }

        var existing = _context.KhuyenMai.Find(km.MaKM);
        if (existing == null) return NotFound();

        existing.TenKM = km.TenKM;
        existing.LoaiKM = km.LoaiKM;
        existing.GiaTri = km.GiaTri;
        existing.NgayBatDau = km.NgayBatDau;
        existing.NgayKetThuc = km.NgayKetThuc;
        existing.TrangThai = km.TrangThai;

        _context.SaveChanges();
        TempData["msg"] = "✅ Đã cập nhật khuyến mãi!";
        return RedirectToAction("Index");
    }

    public IActionResult Xoa(int id)
    {
        var km = _context.KhuyenMai.Find(id);
        if (km != null)
        {
            _context.KhuyenMai.Remove(km);
            _context.SaveChanges();
            TempData["msg"] = "❌ Đã xóa khuyến mãi!";
        }
        return RedirectToAction("Index");
    }
}
