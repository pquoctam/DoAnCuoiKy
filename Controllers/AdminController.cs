using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
public class AdminController : Controller
{
    private readonly TexasContext _context;

    public AdminController(TexasContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
        return ThongKe();
    }
    public IActionResult ThongKe()
    {
        var thang = DateTime.Now.Month;
        var nam = DateTime.Now.Year;

        var dauThang = new DateTime(nam, thang, 1);
        var cuoiThang = dauThang.AddMonths(1);

        ViewBag.TongKhachHang = _context.KhachHang.Count();

        ViewBag.TongDonHang = _context.DonHang
            .Count(d => d.NgayDat >= dauThang && d.NgayDat < cuoiThang);

        ViewBag.TongDoanhThu = _context.DonHang
            .Where(d => d.NgayDat >= dauThang && d.NgayDat < cuoiThang)
            .Sum(d => (decimal?)d.TongTien) ?? 0;

        ViewBag.DoanhThuTheoNgay = _context.DonHang
            .Where(d => d.NgayDat >= dauThang && d.NgayDat < cuoiThang)
            .GroupBy(d => d.NgayDat.Date)
            .Select(g => new { Ngay = g.Key.Day, TongTien = g.Sum(x => x.TongTien) })
            .OrderBy(x => x.Ngay)
            .ToList();

        ViewBag.TopMonAn = _context.ChiTietDonHang
            .Include(ct => ct.MonAn)
            .Include(ct => ct.DonHang)
            .Where(ct => ct.DonHang.NgayDat >= dauThang && ct.DonHang.NgayDat < cuoiThang)
            .GroupBy(ct => ct.MaMon)
            .Select(g => new
            {
                TenMon = g.First().MonAn.TenMon,
                SoLuong = g.Sum(x => x.SoLuong)
            })
            .OrderByDescending(x => x.SoLuong)
            .Take(5)
            .ToList();

        return View("~/Views/Admin/ThongKe/Index.cshtml");
    }
}
