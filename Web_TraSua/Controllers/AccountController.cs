using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using Web_TraSua.ViewModels;
using WebTraSua.Data;
using WebTraSua.Models;

namespace Web_TraSua.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }
            var admin = _context.NhanViens
                .FirstOrDefault(n => n.Email == model.Email && n.MatKhau == model.MatKhau && n.VaiTro == "Admin");
            if (admin != null)
            {
                // Lưu vào session
                HttpContext.Session.SetString("HoTen", admin.HoTen ?? "Admin");
                HttpContext.Session.SetString("Role", "QuanLy");

                // Lưu vào Claims
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, admin.MaNV.ToString()),
            new Claim(ClaimTypes.Name, admin.HoTen ?? "Admin")
        };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                // Chuyển hướng về trang yêu cầu nếu có
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "QuanLy");
            }

            // Tìm người dùng trong bảng Khách hàng
            var kh = _context.KhachHangs.FirstOrDefault(k => k.Email == model.Email && k.MatKhau == model.MatKhau);
            if (kh != null)
            {
                HttpContext.Session.SetString("HoTen", kh.HoTen ?? "Khách hàng");
                HttpContext.Session.SetString("Role", "KhachHang");
                HttpContext.Session.SetInt32("MaKH", kh.MaKH);

                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, kh.MaKH.ToString()),
            new Claim(ClaimTypes.Name, kh.HoTen ?? "Khách hàng")
        };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không đúng.");
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }



        [HttpGet]
        public IActionResult Register(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            var existingUser = _context.KhachHangs.FirstOrDefault(k => k.Email == model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Email này đã được sử dụng.");
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            var newKhachHang = new KhachHang
            {
                HoTen = model.HoTen ?? string.Empty,
                Email = model.Email ?? string.Empty,
                MatKhau = model.MatKhau ?? string.Empty,
                DiemTichLuy = 0,
                NgayTao = DateTime.Now
            };

            _context.KhachHangs.Add(newKhachHang);
            _context.SaveChanges();

            HttpContext.Session.SetString("HoTen", newKhachHang.HoTen ?? "Khách hàng");
            HttpContext.Session.SetString("Role", "KhachHang");
            HttpContext.Session.SetInt32("MaKH", newKhachHang.MaKH);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {

            HttpContext.Session.Clear();
            TempData["Success"] = "Bạn đã đăng xuất thành công!";
            return RedirectToAction("Login");
        }
    }
}
