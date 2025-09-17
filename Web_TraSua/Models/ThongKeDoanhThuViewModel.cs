namespace WebTraSua.Models
{
    public class ThongKeDoanhThuTheoNgayViewModel
    {
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public List<DoanhThuNgay> DoanhThuTheoNgay { get; set; } = new List<DoanhThuNgay>();
        public decimal TongDoanhThu => DoanhThuTheoNgay.Sum(x => x.DoanhThu);
    }
    public class DoanhThuNgay
    {
        public DateTime Ngay { get; set; }
        public decimal DoanhThu { get; set; }
    }
}
