using Microsoft.AspNetCore.Mvc;
using WebTraSua.Data;
using WebTraSua.Models;
using System.Linq;

namespace Web_TraSua.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TaiKhoanController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult ThongTin()
        {
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(role))
            {
                TempData["Error"] = "Bạn cần đăng nhập để xem thông tin cá nhân.";
                return RedirectToAction("Login", "Account");
            }
            if (role == "KhachHang")
            {
                int? maKH = HttpContext.Session.GetInt32("MaKH");
                if (maKH == null)
                {
                    TempData["Error"] = "Bạn cần đăng nhập để xem thông tin cá nhân.";
                    return RedirectToAction("Login", "Account");
                }
                var kh = _context.KhachHangs.FirstOrDefault(k => k.MaKH == maKH.Value);
                if (kh == null)
                {
                    TempData["Error"] = "Tài khoản không tồn tại.";
                    return RedirectToAction("Login", "Account");
                }
                return View("ThongTinKhachHang", kh);
            }
            if (role == "QuanLy")
            {
                var admin = _context.NhanViens.FirstOrDefault(n => n.VaiTro == "Admin");
                if (admin == null)
                {
                    TempData["Error"] = "Admin không tồn tại.";
                    return RedirectToAction("Login", "Account");
                }
                return View("ThongTinAdmin", admin); 
            }

            TempData["Error"] = "Bạn cần đăng nhập để xem thông tin cá nhân.";
            return RedirectToAction("Login", "Account");
        }
        public IActionResult ChinhSuaKhachHang(int id)
        {
            int? maKH = HttpContext.Session.GetInt32("MaKH");
            if (maKH == null || maKH != id)
            {
                TempData["Error"] = "Bạn không có quyền chỉnh sửa tài khoản này.";
                return RedirectToAction("ThongTin");
            }
            var kh = _context.KhachHangs.FirstOrDefault(k => k.MaKH == id);
            if (kh == null)
            {
                TempData["Error"] = "Tài khoản không tồn tại.";
                return RedirectToAction("ThongTin");
            }
            return View(kh);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChinhSuaKhachHang(KhachHang model)
        {
            int? maKH = HttpContext.Session.GetInt32("MaKH");
            if (maKH == null || maKH != model.MaKH)
            {
                TempData["Error"] = "Bạn không có quyền chỉnh sửa tài khoản này.";
                return RedirectToAction("ThongTin");
            }
            var kh = _context.KhachHangs.FirstOrDefault(k => k.MaKH == model.MaKH);
            if (kh == null)
            {
                TempData["Error"] = "Tài khoản không tồn tại.";
                return RedirectToAction("ThongTin");
            }
            kh.HoTen = model.HoTen;
            kh.SDT = model.SDT;
            kh.MatKhau = model.MatKhau;
            _context.SaveChanges();
            HttpContext.Session.SetString("HoTen", kh.HoTen);
            TempData["Success"] = "Cập nhật thông tin thành công!";
            return RedirectToAction("ThongTin");
        }
        public IActionResult ChinhSuaAdmin()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "QuanLy")
            {
                TempData["Error"] = "Bạn không có quyền chỉnh sửa Admin.";
                return RedirectToAction("ThongTin");
            }
            var admin = _context.NhanViens.FirstOrDefault(n => n.VaiTro == "Admin");
            if (admin == null)
            {
                TempData["Error"] = "Admin không tồn tại.";
                return RedirectToAction("ThongTin");
            }
            return View(admin);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChinhSuaAdmin(string hoTen, string matKhau)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "QuanLy")
            {
                TempData["Error"] = "Bạn không có quyền chỉnh sửa Admin.";
                return RedirectToAction("ThongTin");
            }
            var admin = _context.NhanViens.FirstOrDefault(n => n.VaiTro == "Admin");
            if (admin == null)
            {
                TempData["Error"] = "Admin không tồn tại.";
                return RedirectToAction("ThongTin");
            }
            admin.HoTen = hoTen;
            if (!string.IsNullOrEmpty(matKhau))
                admin.MatKhau = matKhau;
            _context.SaveChanges();
            HttpContext.Session.SetString("HoTen", hoTen);
            TempData["Success"] = "Cập nhật thông tin Admin thành công!";
            return RedirectToAction("ThongTin");
        }
        public IActionResult DoiMatKhau()
        {
            var role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(role))
            {
                TempData["Error"] = "Bạn cần đăng nhập để đổi mật khẩu.";
                return RedirectToAction("Login", "Account");
            }
            if (role == "KhachHang")
                return View("DoiMatKhauKhachHang");
            if (role == "QuanLy")
                return View("DoiMatKhauAdmin");
            TempData["Error"] = "Bạn cần đăng nhập để đổi mật khẩu.";
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DoiMatKhauKhachHang(string matKhauCu, string matKhauMoi, string xacNhan)
        {
            int? maKH = HttpContext.Session.GetInt32("MaKH");
            if (maKH == null)
            {
                TempData["Error"] = "Bạn cần đăng nhập để đổi mật khẩu.";
                return RedirectToAction("Login", "Account");
            }
            var kh = _context.KhachHangs.FirstOrDefault(k => k.MaKH == maKH.Value);
            if (kh == null)
            {
                TempData["Error"] = "Tài khoản không tồn tại.";
                return RedirectToAction("Login", "Account");
            }
            if (kh.MatKhau != matKhauCu)
            {
                TempData["Error"] = "Mật khẩu cũ không đúng.";
                return RedirectToAction("DoiMatKhau");
            }
            if (matKhauMoi != xacNhan)
            {
                TempData["Error"] = "Xác nhận mật khẩu mới không trùng khớp.";
                return RedirectToAction("DoiMatKhau");
            }
            kh.MatKhau = matKhauMoi;
            _context.SaveChanges();
            TempData["Success"] = "Đổi mật khẩu thành công!";
            return RedirectToAction("ThongTin");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DoiMatKhauAdmin(string matKhauCu, string matKhauMoi, string xacNhan)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "QuanLy")
            {
                TempData["Error"] = "Bạn không có quyền đổi mật khẩu Admin.";
                return RedirectToAction("ThongTin");
            }
            var admin = _context.NhanViens.FirstOrDefault(n => n.VaiTro == "Admin");
            if (admin == null)
            {
                TempData["Error"] = "Admin không tồn tại.";
                return RedirectToAction("ThongTin");
            }
            if (admin.MatKhau != matKhauCu)
            {
                TempData["Error"] = "Mật khẩu cũ không đúng.";
                return RedirectToAction("DoiMatKhau");
            }
            if (matKhauMoi != xacNhan)
            {
                TempData["Error"] = "Xác nhận mật khẩu mới không trùng khớp.";
                return RedirectToAction("DoiMatKhau");
            }
            admin.MatKhau = matKhauMoi;
            _context.SaveChanges();
            TempData["Success"] = "Đổi mật khẩu Admin thành công!";
            return RedirectToAction("ThongTin");
        }
    }
}
