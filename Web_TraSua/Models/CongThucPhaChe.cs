using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace WebTraSua.Models
{
    [Table("CongThucPhaChe")]
    public class CongThucPhaChe
    {
        [Key, Column(Order = 0)] public int MaSP { get; set; }
        [Key, Column(Order = 1)] public int MaNL { get; set; }
        public decimal SoLuongCoSo { get; set; } 
        [ForeignKey(nameof(MaSP))] public WebTraSua.Models.SanPham SanPham { get; set; }
        [ForeignKey(nameof(MaNL))] public NguyenLieu NguyenLieu { get; set; }
    }
}