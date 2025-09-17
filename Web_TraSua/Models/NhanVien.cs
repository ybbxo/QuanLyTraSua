using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WebTraSua.Models
{
    [Table("NhanVien")]
    public class NhanVien
    {
        [Key] public int MaNV { get; set; }
        public string? HoTen { get; set; }
        public string? GioiTinh { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string? SDT { get; set; }
        public string? DiaChi { get; set; }
        public string? TenChucVu { get; set; }
        [Required] public string Email { get; set; } = "";
        [Required] public string MatKhau { get; set; } = "";
        public string VaiTro { get; set; } = "Admin"; 
    }
}