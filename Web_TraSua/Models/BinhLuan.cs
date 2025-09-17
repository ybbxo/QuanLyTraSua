using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebTraSua.Models
{
    [Table("BinhLuan")] 
    public class BinhLuan
    {
        [Key]
        public int MaBL { get; set; }
        [Required]
        public int MaSP { get; set; } 
        [Required]
        [StringLength(100)]
        public string TenNguoiDung { get; set; }
        [Required]
        public string NoiDung { get; set; }
        [Range(1, 5)]
        public int Sao { get; set; }
        public DateTime? NgayBinhLuan { get; set; }
        public string? TrangThai { get; set; }  
        public string? PhanHoi { get; set; }  
        public DateTime? NgayPhanHoi { get; set; }
        [ForeignKey("MaSP")]
        public virtual SanPham? SanPham { get; set; }
    }
}
