using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebTraSua.Data;
using WebTraSua.Models;
using System.Linq;
namespace WebTraSua.Controllers
{
    public class QuanLyDonHangController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string[] validTrangThai = new[] { "ChoXacNhan", "HoanTat", "Huy" };

        public QuanLyDonHangController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(string trangThai, string search)
        {
            var donHangs = _context.HoaDons
                .Include(h => h.KhachHang) 
                .Include(h => h.NhanVien)
                .AsQueryable();
            if (!string.IsNullOrEmpty(trangThai))
                donHangs = donHangs.Where(d => d.TrangThai == trangThai);
            if (!string.IsNullOrEmpty(search))
                donHangs = donHangs.Where(d => d.KhachHang.HoTen.Contains(search));
            return View(donHangs.OrderByDescending(d => d.NgayLap).ToList());
        }
        public IActionResult Details(int id)
        {
            var donHang = _context.HoaDons
                .Include(h => h.KhachHang)
                .Include(h => h.NhanVien)
                .Include(h => h.ChiTietHoaDons)
                    .ThenInclude(ct => ct.SanPham)
                .Include(h => h.ChiTietHoaDons)
                    .ThenInclude(ct => ct.Size)
                .FirstOrDefault(h => h.MaHD == id);
            if (donHang == null)
                return NotFound();
            return View(donHang);
        }

        [HttpPost]
        public async Task<IActionResult> CapNhatTrangThai(int maHD, string trangThaiMoi)
        {
            var donHang = await _context.HoaDons.FindAsync(maHD);
            if (donHang == null) return Json(new { success = false, message = "Đơn hàng không tồn tại" });
            var trangThaiCu = donHang.TrangThai;
            donHang.TrangThai = trangThaiMoi;
            await _context.SaveChangesAsync();
            if (trangThaiMoi == "HoanTat")
                await _context.Database.ExecuteSqlRawAsync("EXEC dbo.usp_XuatKho_KhiHoanTat {0}", maHD);
            if (trangThaiMoi == "Huy" && trangThaiCu == "HoanTat")
                await _context.Database.ExecuteSqlRawAsync("EXEC dbo.usp_HoanTacXuatKho_HoaDon {0}", maHD);
            return Json(new { success = true, message = "Cập nhật trạng thái thành công" });
        }
        [HttpPost]
        public async Task<IActionResult> HuyDonHang(int maHD)
        {
            Console.WriteLine("Đã nhận maHD: " + maHD);
            var donHang = await _context.HoaDons.FindAsync(maHD);
            if (donHang == null)
                return Json(new { success = false, message = "Đơn hàng không tồn tại" });
            if (donHang.TrangThai != "ChoXacNhan")
                return Json(new { success = false, message = "Đơn hàng không thể hủy, vì đã được xác nhận hoặc đã xử lý" });
            donHang.TrangThai = "Huy";
            await _context.SaveChangesAsync();
            await _context.Database.ExecuteSqlRawAsync("EXEC dbo.usp_HoanTacXuatKho_HoaDon {0}", maHD);
            return Json(new { success = true, message = "Đơn hàng đã hủy thành công" });
        }
    }
}
