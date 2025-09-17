using System.Collections.Generic;
using WebTraSua.Models;
namespace WebTraSua.ViewModels
{
    public class KhachHangDetailViewModel
    {
        public KhachHang KhachHang { get; set; }
        public List<HoaDon> HoaDons { get; set; } = new();
    }
}
