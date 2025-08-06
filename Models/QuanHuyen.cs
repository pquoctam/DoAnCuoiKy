using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TexasChicken.Models
{
    public class QuanHuyen
    {
        [Key]
        public int MaQH { get; set; }

        [Required]
        public string TenQH { get; set; }

        [ForeignKey("ThanhPho")]
        public int MaTP { get; set; }

        public ThanhPho ThanhPho { get; set; }

        public ICollection<NhaHang> NhaHang { get; set; }
    }
}
