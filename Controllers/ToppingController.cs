using Microsoft.AspNetCore.Mvc;
using TexasChicken.Models;

public class ToppingController : Controller
{
    private readonly TexasContext _context;

    public ToppingController(TexasContext context)
    {
        _context = context;
    }

    public IActionResult Index(int page = 1)
    {
        int pageSize = 10;
        int totalItems = _context.Topping.Count();

        var ds = _context.Topping
                         .OrderBy(t => t.MaTopping)
                         .Skip((page - 1) * pageSize)
                         .Take(pageSize)
                         .ToList();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        return View("~/Views/Admin/Topping/Index.cshtml", ds);
    }

    [HttpGet]
    public IActionResult Them()
    {
        return View("~/Views/Admin/Topping/Them.cshtml");
    }

    [HttpPost]
    public IActionResult Them(Topping topping, IFormFile Upload)
    {
        ModelState.Remove("HinhAnh");
        ModelState.Remove("MonAn_Topping");

        if (ModelState.IsValid)
        {
            if (Upload != null && Upload.Length > 0)
            {
                string fileName = $"{Guid.NewGuid()}_{Path.GetFileName(Upload.FileName)}";
                string folderPath = Path.Combine("wwwroot", "images", "Topping");

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), folderPath, fileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    Upload.CopyTo(stream);
                }

                topping.HinhAnh = fileName;
            }

            topping.NgayTao = DateTime.UtcNow.AddHours(7);

            _context.Topping.Add(topping);
            _context.SaveChanges();

            TempData["msg"] = "✅ Thêm topping thành công!";
            return RedirectToAction("Index");
        }

        return View("~/Views/Admin/Topping/Them.cshtml", topping);
    }

    public IActionResult Sua(int id)
    {
        var topping = _context.Topping.FirstOrDefault(t => t.MaTopping == id);
        if (topping == null) return NotFound();
        return View("~/Views/Admin/Topping/Sua.cshtml", topping);
    }

    [HttpPost]
    public async Task<IActionResult> Sua(Topping topping, IFormFile? Upload)
    {
        ModelState.Remove("HinhAnh");
        ModelState.Remove("MonAn_Topping");

        if (!ModelState.IsValid)
        {
            return View("~/Views/Admin/Topping/Sua.cshtml", topping);
        }

        var existing = await _context.Topping.FindAsync(topping.MaTopping);
        if (existing == null) return NotFound();

        existing.TenTopping = topping.TenTopping;
        existing.GiaTopping = topping.GiaTopping;
        existing.Loai = topping.Loai;
        existing.TrangThai = topping.TrangThai;

        if (Upload != null && Upload.Length > 0)
        {
            string fileName = $"{Guid.NewGuid()}_{Path.GetFileName(Upload.FileName)}";
            string folderPath = Path.Combine("wwwroot", "images", "Topping");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), folderPath, fileName);
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await Upload.CopyToAsync(stream);
            }

            existing.HinhAnh = fileName;
        }

        existing.NgayCapNhat = DateTime.UtcNow.AddHours(7);

        await _context.SaveChangesAsync();
        TempData["msg"] = "Cập nhật topping thành công!";
        return RedirectToAction("Index");
    }

    public IActionResult Xoa(int id)
    {
        var topping = _context.Topping.Find(id);
        if (topping != null)
        {
            _context.Topping.Remove(topping);
            _context.SaveChanges();
            TempData["msg"] = "Đã xoá topping vĩnh viễn!";
        }
        return RedirectToAction("Index");
    }
}
