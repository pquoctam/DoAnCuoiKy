using System.ComponentModel.DataAnnotations;

namespace TexasChicken.Models
{
    public class MonAn_Topping
    {
        public int MaMon { get; set; }
        public ThucDon MonAn { get; set; }

        public int MaTopping { get; set; }
        public Topping Topping { get; set; }
    }
}