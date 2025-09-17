using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebTraSua.Data;
using WebTraSua.Models;

namespace WebTraSua.Controllers
{
    [Authorize]  
    public class DonHangController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DonHangController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult LichSu(string search)
        {
            var maKH = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (maKH == null)
                return RedirectToAction("Login", "Account");
            var donHangs = _context.HoaDons
                .Include(h => h.ChiTietHoaDons)  
                .Include(h => h.NhanVien)  
                .Where(h => h.MaKH.ToString() == maKH);
            if (!string.IsNullOrEmpty(search))
            {
                donHangs = donHangs.Where(d => d.TrangThai.Contains(search));
            }
            var model = donHangs.OrderByDescending(d => d.NgayLap).ToList();
            ViewBag.SearchQuery = search;
            return View(model);
        }
        public IActionResult ChiTiet(int id)
        {
            var donHang = _context.HoaDons
                .Include(h => h.ChiTietHoaDons)              
                    .ThenInclude(ct => ct.SanPham)            
                .Include(h => h.ChiTietHoaDons)
                    .ThenInclude(ct => ct.Size)             
                .FirstOrDefault(h => h.MaHD == id);          

            if (donHang == null)
            {
                return NotFound();
            }
            if (donHang.ChiTietHoaDons == null || !donHang.ChiTietHoaDons.Any())
            {
                ViewBag.NoDetailsMessage = "Không có chi tiết sản phẩm cho đơn hàng này.";
            }
            return View(donHang);
        }
        [HttpPost]
        public IActionResult DuyetDon(int id)
        {
            var donHang = _context.HoaDons
                .Include(h => h.NhanVien)  
                .FirstOrDefault(h => h.MaHD == id);

            if (donHang == null)
            {
                return NotFound();
            }
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;  

            if (int.TryParse(userId, out int maNV))  
            {
                var nhanVien = _context.NhanViens.FirstOrDefault(u => u.MaNV == maNV); 

                if (nhanVien != null)
                {
                    donHang.NhanVien = nhanVien; 
                }
                else
                {                
                    ViewBag.Message = "Không tìm thấy nhân viên với mã ID hiện tại.";
                    return View(donHang);  
                }
            }
            else
            {
                ViewBag.Message = "ID người dùng không hợp lệ.";
                return View(donHang);
            }

            donHang.TrangThai = "Đã duyệt";  

            _context.SaveChanges();

            return RedirectToAction("LichSu", "DonHang"); 
        }



    }
}
