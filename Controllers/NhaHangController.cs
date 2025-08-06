using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TexasChicken.Models;
using TexasChicken.Helpers;

namespace TexasChicken.Controllers
{
    public class NhaHangController : Controller
    {
        private readonly TexasContext _context;

        public NhaHangController(TexasContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var dsNhaHang = _context.NhaHang
                .Include(nh => nh.QuanHuyen)
                .ThenInclude(qh => qh.ThanhPho)
                .ToList();

            var dsThanhPho = _context.ThanhPho.ToList();

            ViewBag.ThanhPhoList = dsThanhPho;

            return View(dsNhaHang);
        }
        [HttpPost]
        public async Task<IActionResult> CapNhatToaDo([FromBody] CapNhatToaDoModel model)
        {
            var nhaHang = await _context.NhaHang.FindAsync(model.Id);
            if (nhaHang == null) return NotFound();

            nhaHang.Latitude = model.Lat;
            nhaHang.Longitude = model.Lng;

            await _context.SaveChangesAsync();
            return Ok();
        }

        public class CapNhatToaDoModel
        {
            public int Id { get; set; }
            public double Lat { get; set; }
            public double Lng { get; set; }
        }

    }
}
