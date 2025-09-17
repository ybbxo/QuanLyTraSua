using WebTraSua.Models;
namespace Web_TraSua.ViewModels

{

    public class CongThucEditVM
    {
        public int MaSP { get; set; }
        public string TenSP { get; set; } = "";
        public int MaSizeBase { get; set; }
        public int DungTichBase { get; set; }
        public decimal TongMLBase => Rows?.Where(r => r.DonViTinh == "ml").Sum(r => r.SoLuongCoSo) ?? 0;
        public List<CongThucRow> Rows { get; set; } = new List<CongThucRow>();
        public List<NguyenLieu> AllNguyenLieu { get; set; } = new List<NguyenLieu>();
    }
}
