namespace WebTraSua.Models
{
    public class ThongKeDoanhThuTheoThangViewModel
    {
        public int Nam { get; set; }
        public List<DoanhThuThang> DoanhThuTheoThang { get; set; } = new List<DoanhThuThang>();

        public decimal TongDoanhThu => DoanhThuTheoThang.Sum(x => x.DoanhThu);
    }
    public class DoanhThuThang
    {
        public int Thang { get; set; }
        public decimal DoanhThu { get; set; }
    }
}
