using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebTraSua.Data;
using WebTraSua.Models;

namespace WebTraSua.Controllers
{
    public class NguyenLieuQuanLyController : Controller
    {
        private readonly ApplicationDbContext _ctx;
        public NguyenLieuQuanLyController(ApplicationDbContext ctx) { _ctx = ctx; }

        private bool IsQuanLy() => HttpContext.Session.GetString("Role") == "QuanLy";
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Role") != "QuanLy")
                return View("~/Views/Shared/TrangTrang.cshtml");
            var list = _ctx.NguyenLieus.OrderBy(n => n.MaNL).ToList();
            var conn = _ctx.Database.GetDbConnection();
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT MaNL, Ton FROM dbo.v_TonKhoHienTai";
            var ton = new Dictionary<int, decimal>();
            using (var rd = cmd.ExecuteReader())
                while (rd.Read())
                    ton[rd.GetInt32(0)] = rd.IsDBNull(1) ? 0 : rd.GetDecimal(1);
            ViewBag.TonDict = ton;
            return View(list);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(string TenNL, string DonViTinh)
        {
            if (!IsQuanLy()) return Unauthorized();
            if (string.IsNullOrWhiteSpace(TenNL) || string.IsNullOrWhiteSpace(DonViTinh))
            { TempData["Error"] = "Tên & đơn vị không được trống."; return RedirectToAction(nameof(Index)); }
            _ctx.NguyenLieus.Add(new NguyenLieu { TenNL = TenNL.Trim(), DonViTinh = DonViTinh.Trim().ToLower() });
            _ctx.SaveChanges();
            TempData["Success"] = "Đã thêm nguyên liệu.";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(int MaNL, string TenNL, string DonViTinh)
        {
            if (!IsQuanLy()) return Unauthorized();
            var nl = _ctx.NguyenLieus.Find(MaNL);
            if (nl == null) return NotFound();
            if (string.IsNullOrWhiteSpace(TenNL) || string.IsNullOrWhiteSpace(DonViTinh))
            { TempData["Error"] = "Tên & đơn vị không được trống."; return RedirectToAction(nameof(Index)); }
            nl.TenNL = TenNL.Trim();
            nl.DonViTinh = DonViTinh.Trim().ToLower();
            _ctx.SaveChanges();
            TempData["Success"] = "Đã cập nhật nguyên liệu.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Delete(int MaNL)
        {
            if (!IsQuanLy()) return Unauthorized();
            var nl = _ctx.NguyenLieus.Find(MaNL);
            if (nl == null) return NotFound();
            var used = _ctx.CongThucPhaChes.Any(x => x.MaNL == MaNL);
            if (used)
            { TempData["Error"] = "Nguyên liệu đang được dùng trong công thức, không thể xoá."; return RedirectToAction(nameof(Index)); }
            _ctx.NguyenLieus.Remove(nl);
            _ctx.SaveChanges();
            TempData["Success"] = "Đã xoá nguyên liệu.";
            return RedirectToAction(nameof(Index));
        }
    }
}
