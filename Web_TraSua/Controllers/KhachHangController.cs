using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebTraSua.Data;
using WebTraSua.Models;
using WebTraSua.Models;
using WebTraSua.ViewModels;


namespace WebTraSua.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly ApplicationDbContext _context;
        public KhachHangController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách khách hàng + tìm kiếm
        public async Task<IActionResult> Index(string search)
        {
            var khachHangs = _context.KhachHangs.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                khachHangs = khachHangs.Where(k => k.HoTen.Contains(search)
                                                || k.Email.Contains(search)
                                                || k.SDT.Contains(search));
            }

            return View(await khachHangs.OrderByDescending(k => k.NgayTao).ToListAsync());
        }

        // Chi tiết khách hàng
        public IActionResult ChiTiet(int id)
        {
            var kh = _context.KhachHangs.FirstOrDefault(k => k.MaKH == id);
            if (kh == null) return NotFound();

            var hoaDons = _context.HoaDons
                .Where(h => h.MaKH == id)
                .OrderByDescending(h => h.NgayLap)
                .ToList();

            var vm = new KhachHangDetailViewModel
            {
                KhachHang = kh,
                HoaDons = hoaDons
            };

            return View(vm); // 👈 Phải truyền ViewModel, không phải KhachHang
        }

        // Xóa khách hàng
        [HttpPost]
        public async Task<IActionResult> Xoa(int id)
        {
            try
            {
                var kh = await _context.KhachHangs.FindAsync(id);
                if (kh == null)
                    return Json(new { success = false, message = "Khách hàng không tồn tại" });

                // Kiểm tra có đơn hàng liên kết không
                var hasOrder = await _context.HoaDons.AnyAsync(h => h.MaKH == id);
                if (hasOrder)
                    return Json(new { success = false, message = "Không thể xóa vì khách hàng đã có đơn hàng liên kết." });

                _context.KhachHangs.Remove(kh);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Đã xóa khách hàng thành công." });
            }
            catch (Exception ex)
            {
                // 👇 Gửi lỗi chi tiết về client để debug
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }


        // Bulk action: Xóa nhiều khách hàng
        [HttpPost]
        public async Task<IActionResult> XoaNhieu([FromBody] int[] ids)
        {
            var khachHangs = await _context.KhachHangs.Where(k => ids.Contains(k.MaKH)).ToListAsync();
            if (!khachHangs.Any()) return Json(new { success = false, message = "Không tìm thấy khách hàng nào" });

            _context.KhachHangs.RemoveRange(khachHangs);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = $"Đã xóa {khachHangs.Count} khách hàng" });
        }
        [HttpPost]
        public IActionResult CapNhat(KhachHang kh)
        {
            var existing = _context.KhachHangs.FirstOrDefault(k => k.MaKH == kh.MaKH);
            if (existing == null) return Json(new { success = false, message = "Khách hàng không tồn tại" });

            existing.HoTen = kh.HoTen;
            existing.Email = kh.Email;
            existing.SDT = kh.SDT;
            existing.DiemTichLuy = kh.DiemTichLuy;

            _context.SaveChanges();
            return Json(new { success = true, message = "Cập nhật thông tin khách hàng thành công" });
        }
      



[HttpPost]
    public async Task<IActionResult> CapNhatTrangThaiDonHang(int maHD, string trangThaiMoi)
    {
        var hd = _context.HoaDons.FirstOrDefault(h => h.MaHD == maHD);
        if (hd == null) return Json(new { success = false, message = "Không tìm thấy đơn hàng" });

        hd.TrangThai = trangThaiMoi;
        _context.SaveChanges();

        // 🔻 GỌI PROC TRỪ KHO KHI HOÀN TẤT
        if (trangThaiMoi == "HoanTat")
        {
            await _context.Database.ExecuteSqlRawAsync("EXEC dbo.usp_XuatKho_KhiHoanTat {0}", maHD);
        }

        return Json(new { success = true, message = $"Đã cập nhật trạng thái đơn hàng: {trangThaiMoi}" });
    }


}
}
