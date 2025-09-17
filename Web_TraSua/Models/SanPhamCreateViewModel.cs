using System.ComponentModel.DataAnnotations;
using Web_TraSua.ViewModels;

namespace WebTraSua.Models
{
    public class SanPhamCreateViewModel
    {
        [Required]
        public string TenSP { get; set; }
        public int MaDM { get; set; }
        [Range(0, 10000000)]
        public decimal GiaS { get; set; }
        public List<SizeGiaModel> Sizes { get; set; } = new();
        public List<CongThucRow> CongThuc { get; set; } = new();
}
    public class SizeGiaModel
    {
        public int MaSize { get; set; }
        public string TenSize { get; set; }
        public bool DuocChon { get; set; }
        public decimal Gia { get; set; }
    }
}
