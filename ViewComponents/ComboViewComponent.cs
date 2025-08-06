using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using TexasChicken.Models;

namespace TexasChicken.ViewComponents
{
    public class ComboViewComponent : ViewComponent
    {
        private readonly TexasContext _context;

        public ComboViewComponent(TexasContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var combos = _context.ThucDon
                .Where(t => t.TrangThai == true)
                .OrderByDescending(t => t.GiaBan)
                .Take(5)
                .Select(t => new
                {
                    MaMon = t.MaMon,
                    ImageUrl = "/images/ThucDon/" + (string.IsNullOrEmpty(t.HinhAnh) ? "default.png" : t.HinhAnh),
                    Title = t.TenMon,
                    CurrentPrice = t.GiaBan.ToString("N0") + "đ",
                    OriginalPrice = t.GiaGoc > t.GiaBan
                        ? t.GiaGoc.ToString("N0") + "đ"
                        : null
                })
                .ToList();
            return View(combos);
        }
    }
}
