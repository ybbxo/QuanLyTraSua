namespace Web_TraSua.ViewModels
{
    public class SanPhamChiTietBySizeDto
    {
        public int MaSP { get; set; }
        public string TenSP { get; set; } = "";
        public int MaSize { get; set; }
        public string TenSize { get; set; } = "";
        public decimal Gia { get; set; }
        public List<NguyenLieuTheoSizeDto> NguyenLieus { get; set; } = new();
    }
}
