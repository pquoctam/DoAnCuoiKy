using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TexasChicken.Models
{
    public class CartItemTopping
    {
        public int Id { get; set; }
        public int CartItemId { get; set; }
        public int MaTopping { get; set; }

        [ForeignKey("CartItemId")]
        public virtual CartItem CartItem { get; set; }


        [ForeignKey("MaTopping")]
        public virtual Topping MaToppingNavigation { get; set; }
    }
}

