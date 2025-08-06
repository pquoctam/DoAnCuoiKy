using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TexasChicken.Models
{
    public class CartItem
    {
        public int CartItemId { get; set; }
        public int CartId { get; set; }
        public int MaMon { get; set; }
        public int SoLuong { get; set; }

        public virtual Cart Cart { get; set; }

        public virtual ICollection<CartItemTopping> CartItemTopping { get; set; }

        [ForeignKey("MaMon")]
        public virtual ThucDon MaMonNavigation { get; set; }
    }
}
