using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TexasChicken.Models
{
    public class LoaiMon
    {
        [Key]
        public int MaLoai { get; set; }

        [Required(ErrorMessage = "Tên loại món không được để trống.")]
        [Display(Name = "Tên Loại Món")]
        public string TenLoai { get; set; }

        public string HinhAnh { get; set; }

        public bool TrangThai { get; set; } = true;

        public DateTime NgayTao { get; set; }

        public DateTime? NgayCapNhat { get; set; }

        // Navigation property: Một loại món có nhiều món ăn
        [NotMapped]
        public ICollection<ThucDon> ThucDon { get; set; }
    }
}
