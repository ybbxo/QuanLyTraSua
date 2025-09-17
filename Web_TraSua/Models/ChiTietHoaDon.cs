using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebTraSua.Models;

[Table("ChiTietHoaDon")]
public class ChiTietHoaDon
{
    [Key, Column(Order = 0)] public int MaHD { get; set; }
    [Key, Column(Order = 1)] public int MaSP { get; set; }
    [Key, Column(Order = 2)] public int MaSize { get; set; }    
    public int SoLuong { get; set; }
    public decimal DonGia { get; set; }
    [NotMapped]
    public decimal ThanhTien => SoLuong * DonGia;
    [ForeignKey(nameof(MaHD))] public HoaDon HoaDon { get; set; }
    [ForeignKey(nameof(MaSP))] public SanPham SanPham { get; set; }
    [ForeignKey(nameof(MaSize))] public Size Size { get; set; } 

}
