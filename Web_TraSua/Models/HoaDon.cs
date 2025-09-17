using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WebTraSua.Models
{
    [Table("HoaDon")]
    public class HoaDon
    {
        [Key] public int MaHD { get; set; }

        [ForeignKey(nameof(KhachHang))]
        public int? MaKH { get; set; }
        public KhachHang KhachHang { get; set; }

        [ForeignKey(nameof(NhanVien))]
        public int? MaNV { get; set; }
        public NhanVien? NhanVien { get; set; }
        public DateTime NgayLap { get; set; }
        public decimal TongTien { get; set; }
        public int? MaHTTT { get; set; }
        public ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; }
        public string? TrangThai { get; set; }
        public string? TenNguoiNhan { get; set; }
        public string? DiaChiNhan { get; set; }
        public string? SoDienThoaiNhan { get; set; }
    }

}
