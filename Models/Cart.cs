using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TexasChicken.Models
{
    public class Cart
    {
        public int CartId { get; set; }
        public int MaKH { get; set; }  // FK đến KhachHang
        public DateTime CreatedAt { get; set; }

        [ForeignKey("MaKH")]
        public virtual KhachHang KhachHang { get; set; }
        public virtual ICollection<CartItem> CartItem { get; set; }
    }
}
