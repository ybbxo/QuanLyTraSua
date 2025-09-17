using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Web_TraSua.Models;

namespace WebTraSua.Models
{
    [Table("SanPham")]
    public class SanPham
    {
        [Key] public int MaSP { get; set; }
        [Required] public string TenSP { get; set; }
        public int MaDM { get; set; }
        [ForeignKey(nameof(MaDM))] public DanhMucSanPham? DanhMuc { get; set; }
        public bool TrangThai { get; set; } = true; 
        public string? Anh { get; set; }           
        public ICollection<SanPhamSize> SanPhamSizes { get; set; } = new List<SanPhamSize>();
        public ICollection<BinhLuan> BinhLuans { get; set; } = new List<BinhLuan>();

    }

}
