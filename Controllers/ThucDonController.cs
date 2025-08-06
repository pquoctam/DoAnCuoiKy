using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TexasChicken.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq;
using System.Text;

namespace TexasChicken.Controllers;

public class ThucDonController : Controller
{
    private readonly TexasContext _context;

    private string RemoveDiacritics(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new System.Text.StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }

    public ThucDonController(TexasContext context)
    {
        _context = context;
    }

    public IActionResult Index(int? maLoai, string keyword)
    {
        var monan = _context.ThucDon
                            .Where(m => m.TrangThai == true)
                            .AsQueryable();

        string tieuDe = "THỰC ĐƠN";

        if (maLoai.HasValue)
        {
            monan = monan.Where(m => m.MaLoai == maLoai.Value);

            var loaiMon = _context.LoaiMon.FirstOrDefault(l => l.MaLoai == maLoai.Value);
            if (loaiMon != null)
            {
                tieuDe = loaiMon.TenLoai.ToUpper();
            }
        }

        // ✅ Thêm tìm kiếm
        if (!string.IsNullOrEmpty(keyword))
        {
            string keywordNoDiacritics = RemoveDiacritics(keyword.ToLower());

            monan = monan
                .AsEnumerable() // ✅ Chuyển sang LINQ in-memory
                .Where(m => RemoveDiacritics(m.TenMon.ToLower()).Contains(keywordNoDiacritics))
                .AsQueryable(); // ✅ Nếu muốn giữ kiểu IQueryable cho thống nhất

            ViewBag.TimKiem = keyword;
        }


        ViewBag.TieuDe = tieuDe;

        var danhmuc = _context.LoaiMon
                              .Where(l => l.TrangThai == true)
                              .ToList();
        ViewBag.DanhMuc = danhmuc;

        return View(monan.ToList());
    }

    public IActionResult ChiTiet(int id)
    {
        var monAn = _context.ThucDon.FirstOrDefault(m => m.MaMon == id);
        if (monAn == null)
        {
            return NotFound();
        }

        // ✅ Chỉ lấy loại món đang hiển thị
        ViewBag.DanhMuc = _context.LoaiMon
                                  .Where(l => l.TrangThai == true)
                                  .ToList();

        ViewBag.MaLoaiDangXem = monAn.MaLoai;

        var toppings = from t in _context.Topping
                       join mt in _context.MonAn_Topping on t.MaTopping equals mt.MaTopping
                       where mt.MaMon == id
                       select t;

        ViewBag.Toppings = toppings.ToList();

        return View(monAn);
    }

    public IActionResult QuanLy(int page = 1)
    {
        int pageSize = 10;

        var query = _context.ThucDon
            .Include(m => m.LoaiMon); // Lấy tất cả, không lọc theo TrangThai

        int totalItems = query.Count();

        var list = query
            .OrderByDescending(m => m.NgayTao)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
        ViewBag.CurrentPage = page;
        return View("~/Views/Admin/ThucDon/QuanLy.cshtml", list);
    }

    [HttpGet]
    public IActionResult Them()
    {
        ViewBag.DanhMuc = _context.LoaiMon.Where(l => l.TrangThai == true).ToList();
        return View("~/Views/Admin/ThucDon/Them.cshtml");
    }

    [HttpPost]
    public IActionResult Them(ThucDon mon, IFormFile Upload)
    {
        ModelState.Remove("LoaiMon");
        ModelState.Remove("MonAn_Topping");
        ModelState.Remove("HinhAnh");
        ModelState.Remove("MoTa");
        if (ModelState.IsValid)
        {
            if (Upload != null && Upload.Length > 0)
            {
                var fileName = Path.GetFileName(Upload.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/ThucDon", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    Upload.CopyTo(stream);
                }

                mon.HinhAnh = fileName;
            }

            mon.NgayTao = DateTime.UtcNow.AddHours(7);
            _context.ThucDon.Add(mon);
            _context.SaveChanges();

            return RedirectToAction("QuanLy");
        }

        ViewBag.DanhMuc = _context.LoaiMon.Where(l => l.TrangThai == true).ToList();
        return View("~/Views/Admin/ThucDon/Them.cshtml", mon);
    }

    [HttpGet]
    public IActionResult Sua(int id)
    {
        var mon = _context.ThucDon.FirstOrDefault(m => m.MaMon == id);
        if (mon == null)
        {
            return NotFound();
        }

        ViewBag.DanhMuc = _context.LoaiMon.Where(l => l.TrangThai == true).ToList();
        return View("~/Views/Admin/ThucDon/Sua.cshtml", mon);
    }

    [HttpPost]
    public IActionResult Sua(ThucDon mon, IFormFile? Upload)
    {
        ModelState.Remove("LoaiMon");
        ModelState.Remove("MonAn_Topping");
        ModelState.Remove("HinhAnh");
        ModelState.Remove("MoTa");

        if (ModelState.IsValid)
        {
            var existing = _context.ThucDon.FirstOrDefault(m => m.MaMon == mon.MaMon);
            if (existing == null)
            {
                return NotFound();
            }

            // Cập nhật các trường
            existing.TenMon = mon.TenMon;
            existing.GiaGoc = mon.GiaGoc;
            existing.GiaBan = mon.GiaBan;
            existing.MoTa = mon.MoTa;
            existing.TrangThai = mon.TrangThai;
            existing.MaLoai = mon.MaLoai;
            existing.NgayCapNhat = DateTime.UtcNow.AddHours(7);

            // Cập nhật ảnh nếu có upload mới
            if (Upload != null && Upload.Length > 0)
            {
                var fileName = Path.GetFileName(Upload.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/ThucDon", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    Upload.CopyTo(stream);
                }

                existing.HinhAnh = fileName;
            }

            _context.SaveChanges();

            return RedirectToAction("QuanLy");
        }

        ViewBag.DanhMuc = _context.LoaiMon.Where(l => l.TrangThai == true).ToList();
        return View("~/Views/Admin/ThucDon/Sua.cshtml", mon);
    }

    public IActionResult Xoa(int id)
    {
        var mon = _context.ThucDon.FirstOrDefault(m => m.MaMon == id);
        if (mon == null)
        {
            return NotFound();
        }

        _context.ThucDon.Remove(mon);
        _context.SaveChanges();

        return RedirectToAction("QuanLy");
    }

    public IActionResult GanTopping(int id)
    {
        var mon = _context.ThucDon
                          .Include(m => m.MonAn_Topping)
                          .ThenInclude(mt => mt.Topping)
                          .FirstOrDefault(m => m.MaMon == id);

        if (mon == null) return NotFound();

        var toppingList = _context.Topping
                                  .Where(t => t.TrangThai)
                                  .ToList();

        var ganToppingVM = new GanToppingViewModel
        {
            MaMon = id,
            TenMon = mon.TenMon,
            DanhSachTopping = toppingList.Select(t => new ToppingItem
            {
                MaTopping = t.MaTopping,
                TenTopping = t.TenTopping,
                DuocChon = mon.MonAn_Topping.Any(mt => mt.MaTopping == t.MaTopping)
            }).ToList()
        };

        return View("~/Views/Admin/ThucDon/GanTopping.cshtml", ganToppingVM);
    }

    [HttpPost]
    public IActionResult GanTopping(GanToppingViewModel model)
    {
        var mon = _context.ThucDon
                          .Include(m => m.MonAn_Topping)
                          .FirstOrDefault(m => m.MaMon == model.MaMon);
        if (mon == null) return NotFound();

        // Xoá các topping cũ
        var dsCu = _context.MonAn_Topping.Where(mt => mt.MaMon == model.MaMon);
        _context.MonAn_Topping.RemoveRange(dsCu);

        // Thêm mới các topping được chọn
        if (model.DanhSachTopping != null)
        {
            foreach (var item in model.DanhSachTopping)
            {
                if (item.DuocChon)
                {
                    _context.MonAn_Topping.Add(new MonAn_Topping
                    {
                        MaMon = model.MaMon,
                        MaTopping = item.MaTopping
                    });
                }
            }
        }

        _context.SaveChanges();
        TempData["msg"] = "✅ Gán topping thành công!";
        return RedirectToAction("QuanLy");
    }

    [HttpGet]
    public IActionResult TimKiem(string query)
    {
        if (string.IsNullOrEmpty(query))
            return Json(new List<object>());

        var ketQua = _context.ThucDon
            .Where(m => m.TrangThai == true && m.TenMon.Contains(query))
            .Select(m => new
            {
                maMon = m.MaMon,
                tenMon = m.TenMon,
                hinhAnh = "/images/ThucDon/" + m.HinhAnh,
                giaBan = m.GiaBan.ToString("N0") + "đ"
            })
            .Take(5)
            .ToList();

        return Json(ketQua);
    }
}