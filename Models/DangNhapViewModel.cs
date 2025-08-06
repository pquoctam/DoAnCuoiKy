using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TexasChicken.Models
{
    public class DangNhapViewModel
    {
        public string Email { get; set; }
        public string MatKhau { get; set; }
    }
}