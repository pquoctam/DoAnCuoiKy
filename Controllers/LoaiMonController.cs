using Microsoft.AspNetCore.Mvc;
using TexasChicken.Models;

public class LoaiMonController : Controller
{
    private readonly TexasContext _context;

    public LoaiMonController(TexasContext context)
    {
        _context = context;
    }

    public IActionResult Index(int page = 1)
    {
        int pageSize = 10;
        int totalItems = _context.LoaiMon.Count(l => l.TrangThai);

        var ds = _context.LoaiMon
                         .OrderBy(l => l.MaLoai)
                         .Skip((page - 1) * pageSize)
                         .Take(pageSize)
                         .ToList();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        return View("~/Views/Admin/LoaiMon/Index.cshtml", ds);
    }

    [HttpGet]
    public IActionResult Them()
    {
        return View("~/Views/Admin/LoaiMon/Them.cshtml");
    }

    [HttpPost]
    public IActionResult Them(LoaiMon loai, IFormFile Upload)
    {
        ModelState.Remove("HinhAnh");
        ModelState.Remove("ThucDon");

        if (ModelState.IsValid)
        {
            if (Upload != null && Upload.Length > 0)
            {
                string fileName = Path.GetFileName(Upload.FileName);
                string folderPath = Path.Combine("wwwroot", "images", "LoaiMon");

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), folderPath, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    Upload.CopyTo(stream);
                }

                loai.HinhAnh = fileName;
            }

            // Gán ngày tạo
            loai.NgayTao = DateTime.UtcNow.AddHours(7);

            _context.LoaiMon.Add(loai);
            _context.SaveChanges();
            TempData["msg"] = "Thêm thành công!";
            return RedirectToAction("Index");
        }

        return View("~/Views/Admin/LoaiMon/Them.cshtml", loai);
    }

    public IActionResult Sua(int id)
    {
        var loai = _context.LoaiMon.FirstOrDefault(l => l.MaLoai == id);
        if (loai == null) return NotFound();
        return View("~/Views/Admin/LoaiMon/Sua.cshtml", loai);
    }

    [HttpPost]
    public async Task<IActionResult> Sua(LoaiMon loai, IFormFile? Upload)
    {
        ModelState.Remove("HinhAnh");
        ModelState.Remove("ThucDon");

        if (!ModelState.IsValid)
        {
            return View("~/Views/Admin/LoaiMon/Sua.cshtml", loai);
        }

        var existing = await _context.LoaiMon.FindAsync(loai.MaLoai);
        if (existing == null) return NotFound();

        existing.TenLoai = loai.TenLoai;
        existing.TrangThai = loai.TrangThai;

        if (Upload != null && Upload.Length > 0)
        {
            string fileName = $"{Guid.NewGuid()}_{Path.GetFileName(Upload.FileName)}";
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/LoaiMon", fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await Upload.CopyToAsync(stream);
            }

            existing.HinhAnh = fileName;
        }

        // Gán ngày cập nhật
        existing.NgayCapNhat = DateTime.UtcNow.AddHours(7);

        await _context.SaveChangesAsync();
        TempData["msg"] = "Đã cập nhật thành công!";
        return RedirectToAction("Index");
    }

    public IActionResult Xoa(int id)
    {
        var loai = _context.LoaiMon.Find(id);
        if (loai != null)
        {
            _context.LoaiMon.Remove(loai);
            _context.SaveChanges();
            TempData["msg"] = "Đã xóa vĩnh viễn!";
        }
        return RedirectToAction("Index");
    }
}
