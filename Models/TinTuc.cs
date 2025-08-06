using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TexasChicken.Models
{
    public class TinTuc
    {
        [Key]
        public int MaTin { get; set; }

        [Required]
        public string TieuDe { get; set; }

        public string NoiDung { get; set; }

        public string HinhAnh { get; set; }

        public DateTime NgayDang { get; set; } = DateTime.Now;

        public string TacGia { get; set; }

        public bool TrangThai { get; set; }  // True: hiện / False: ẩn
    }
}