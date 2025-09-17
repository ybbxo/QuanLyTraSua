using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebTraSua.Data;
using WebTraSua.Models;

namespace WebTraSua.Controllers
{
    public class BinhLuanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BinhLuanController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var binhLuans = await _context.BinhLuans
                .Include(b => b.SanPham)
                .OrderByDescending(b => b.NgayBinhLuan)
                .ToListAsync();

            return View(binhLuans); 
        }

        #region Người dùng thêm bình luận

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemBinhLuan(int MaSP, string TenNguoiDung, string NoiDung, int Sao)
        {
            if (string.IsNullOrWhiteSpace(TenNguoiDung) || string.IsNullOrWhiteSpace(NoiDung) || Sao < 1 || Sao > 5)
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ.";
                return RedirectToAction("ChiTiet", "SanPham", new { id = MaSP });
            }

            var binhLuan = new BinhLuan
            {
                MaSP = MaSP,
                TenNguoiDung = TenNguoiDung,
                NoiDung = NoiDung,
                Sao = Sao,
                NgayBinhLuan = DateTime.Now,
                TrangThai = "ChoDuyet" 
            };

            _context.BinhLuans.Add(binhLuan);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cảm ơn bạn đã bình luận!";
            return RedirectToAction("ChiTiet", "SanPham", new { id = MaSP });
        }

        #endregion

        #region Admin quản lý bình luận
        public async Task<IActionResult> QuanLyBinhLuan(string trangThai, string search)
        {
            var binhLuans = _context.BinhLuans
                .Include(b => b.SanPham)
                .AsQueryable();
            if (!string.IsNullOrEmpty(trangThai))
                binhLuans = binhLuans.Where(b => b.TrangThai == trangThai);

            if (!string.IsNullOrEmpty(search))
                binhLuans = binhLuans.Where(b => b.TenNguoiDung.Contains(search) || b.SanPham.TenSP.Contains(search));

            return View("Index", await binhLuans.OrderByDescending(b => b.NgayBinhLuan).ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> CapNhatTrangThai(int MaBL, string TrangThaiMoi)
        {
            var binhLuan = await _context.BinhLuans.FindAsync(MaBL);
            if (binhLuan == null)
                return Json(new { success = false, message = "Bình luận không tồn tại" });

            binhLuan.TrangThai = TrangThaiMoi;
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Cập nhật trạng thái thành công" });
        }

        public async Task<IActionResult> ChiTiet(int id)
        {
            var binhLuan = await _context.BinhLuans
                .Include(b => b.SanPham)
                .FirstOrDefaultAsync(b => b.MaBL == id);

            if (binhLuan == null)
                return NotFound();

            return View(binhLuan);
        }

        [HttpPost]
        public async Task<IActionResult> Xoa(int MaBL)
        {
            var binhLuan = await _context.BinhLuans.FindAsync(MaBL);
            if (binhLuan == null)
                return Json(new { success = false, message = "Bình luận không tồn tại" });

            _context.BinhLuans.Remove(binhLuan);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Xóa bình luận thành công" });
        }
        [HttpPost]
        public IActionResult PhanHoi(int maBL, string PhanHoi)
        {
            // Kiểm tra nếu Phản hồi bị trống
            if (string.IsNullOrWhiteSpace(PhanHoi))
            {
                return Json(new { success = false, message = "Phản hồi không được để trống!" });
            }          
            var binhLuan = _context.BinhLuans.FirstOrDefault(b => b.MaBL == maBL);
            if (binhLuan == null)
            {
                return Json(new { success = false, message = "Bình luận không tồn tại!" });
            }

            binhLuan.PhanHoi = PhanHoi;
            binhLuan.NgayPhanHoi = DateTime.Now;  
            binhLuan.TrangThai = "Duyet";  
            try
            {
                _context.SaveChanges();
                return Json(new { success = true, message = "Phản hồi đã được gửi!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi lưu phản hồi. " + ex.Message });
            }
        }
        public async Task<IActionResult> TimKiem(string trangThai, string search)
        {
            var binhLuans = _context.BinhLuans
                .Include(b => b.SanPham)
                .AsQueryable();

            if (!string.IsNullOrEmpty(trangThai))
                binhLuans = binhLuans.Where(b => b.TrangThai == trangThai);
            if (!string.IsNullOrWhiteSpace(search))
            {
                binhLuans = binhLuans.Where(b =>
                    (b.TenNguoiDung ?? "").Contains(search) ||
                    (b.SanPham.TenSP ?? "").Contains(search) ||
                    (b.NoiDung ?? "").Contains(search)
                );
            }

            var model = await binhLuans
                .OrderByDescending(b => b.NgayBinhLuan)
                .ToListAsync();

            return View("Index", model); 
        }
        #endregion
    }

}
