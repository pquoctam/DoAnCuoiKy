using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TexasChicken.Models;
using System.Linq;
using System.Threading.Tasks;

namespace TexasChicken.Controllers
{
    public class TinTucController : Controller
    {
        private readonly TexasContext _context;

        public TinTucController(TexasContext context)
        {
            _context = context;
        }

        // GET: /TinTuc
        public IActionResult Index(int page = 1)
        {
            int pageSize = 5;
            var allNews = _context.TinTuc.OrderByDescending(t => t.NgayDang).ToList();

            var pagedNews = allNews
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)allNews.Count / pageSize);

            return View(pagedNews);
        }

        public IActionResult ChiTiet(int MaTin)
        {
            var tin = _context.TinTuc.FirstOrDefault(t => t.MaTin == MaTin);
            if (tin == null)
            {
                return NotFound();
            }
            return View(tin);
        }

        public IActionResult QuanLy(int page = 1)
        {
            int pageSize = 10;
            var query = _context.TinTuc.AsQueryable();
            int totalItems = query.Count();

            var ds = query.OrderByDescending(t => t.NgayDang)
                          .Skip((page - 1) * pageSize)
                          .Take(pageSize)
                          .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            return View("~/Views/Admin/TinTuc/QuanLy.cshtml", ds);
        }

        [HttpGet]
        public IActionResult Them()
        {
            return View("~/Views/Admin/TinTuc/Them.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Them(TinTuc tin, IFormFile? Upload)
        {
            ModelState.Remove("HinhAnh");

            if (ModelState.IsValid)
            {
                if (Upload != null && Upload.Length > 0)
                {
                    string fileName = Path.GetFileName(Upload.FileName);
                    string folder = Path.Combine("wwwroot", "images", "TinTuc");

                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    string path = Path.Combine(Directory.GetCurrentDirectory(), folder, fileName);
                    using var stream = new FileStream(path, FileMode.Create);
                    Upload.CopyTo(stream);

                    tin.HinhAnh = fileName;
                }

                // ✅ Giữ nguyên giá trị trạng thái người dùng chọn
                tin.NgayDang = DateTime.UtcNow.AddHours(7);

                _context.TinTuc.Add(tin);
                _context.SaveChanges();
                TempData["msg"] = "Đã thêm tin tức thành công!";
                return RedirectToAction("QuanLy");
            }

            return View("~/Views/Admin/TinTuc/Them.cshtml", tin);
        }

        public IActionResult Sua(int id)
        {
            var tin = _context.TinTuc.Find(id);
            if (tin == null) return NotFound();
            return View("~/Views/Admin/TinTuc/Sua.cshtml", tin);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Sua(TinTuc tin, IFormFile? Upload)
        {
            ModelState.Remove("HinhAnh");

            if (!ModelState.IsValid) return View("~/Views/Admin/TinTuc/Sua.cshtml", tin);

            var existing = _context.TinTuc.Find(tin.MaTin);
            if (existing == null) return NotFound();

            existing.TieuDe = tin.TieuDe;
            existing.NoiDung = tin.NoiDung;
            existing.TacGia = tin.TacGia;
            existing.TrangThai = tin.TrangThai;

            if (Upload != null && Upload.Length > 0)
            {
                string fileName = $"{Guid.NewGuid()}_{Upload.FileName}";
                string folder = Path.Combine("wwwroot", "images", "TinTuc");
                string path = Path.Combine(Directory.GetCurrentDirectory(), folder, fileName);

                using var stream = new FileStream(path, FileMode.Create);
                Upload.CopyTo(stream);
                existing.HinhAnh = fileName;
            }

            _context.SaveChanges();
            TempData["msg"] = "✅ Đã cập nhật thành công!";
            return RedirectToAction("QuanLy");
        }

        public IActionResult Xoa(int id)
        {
            var tin = _context.TinTuc.Find(id);
            if (tin != null)
            {
                // Xoá luôn khỏi database
                _context.TinTuc.Remove(tin);
                _context.SaveChanges();
                TempData["msg"] = "Tin tức đã bị xóa vĩnh viễn khỏi hệ thống!";
            }
            else
            {
                TempData["msg"] = "Không tìm thấy tin tức để xóa.";
            }

            return RedirectToAction("QuanLy");
        }
    }
}
