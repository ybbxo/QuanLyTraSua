using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
namespace WebTraSua.Models
{
    [Table("KhachHang")]
    public class KhachHang
    {
        [Key]
        public int MaKH { get; set; }

        [Required]
        public string HoTen { get; set; }

        [Required]
        public string Email { get; set; } 
        public string MatKhau { get; set; }
        public string? SDT { get; set; }
        public int DiemTichLuy { get; set; }
        public DateTime NgayTao { get; set; }
    }

}
