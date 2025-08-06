using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TexasChicken.Models
{
    public class ThanhPho
    {
        [Key]
        public int MaTP { get; set; }

        [Required]
        public string TenTP { get; set; }

        public ICollection<QuanHuyen> QuanHuyen { get; set; }
    }
}
