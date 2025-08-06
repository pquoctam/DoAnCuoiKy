using System.ComponentModel.DataAnnotations.Schema;
namespace TexasChicken.Models
{
    public class ChiTietDonHangTopping
    {
        public int Id { get; set; }

        [ForeignKey("ChiTietDonHang")]
        public int MaChiTiet { get; set; }

        [ForeignKey("Topping")]
        public int MaTopping { get; set; }

        // Quan hệ
        public virtual ChiTietDonHang ChiTietDonHang { get; set; }
        public virtual Topping Topping { get; set; }
    }
}
