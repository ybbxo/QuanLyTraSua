using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebTraSua.Models;
namespace Web_TraSua.Models
{
    [Table("SanPhamSize")]
    public class SanPhamSize
    {
        [Key, Column(Order = 0)]
        [ForeignKey("SanPham")]
        public int MaSP { get; set; }
        public SanPham SanPham { get; set; }
        [Key, Column(Order = 1)]
        [ForeignKey("Size")]
        public int MaSize { get; set; }
        public Size Size { get; set; }
        public decimal Gia { get; set; }
    }
}
