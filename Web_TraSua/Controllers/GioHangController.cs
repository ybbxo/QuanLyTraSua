using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Web_TraSua.Models;
using WebTraSua.Data;
using WebTraSua.Models;
using WebTraSua.ViewModels;

namespace WebTraSua.Controllers
{
    public class GioHangController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GioHangController(ApplicationDbContext context)
        {
            _context = context;
        }
        private List<GioHangItem> LayGioHang()
        {
            var gio = HttpContext.Session.GetObjectFromJson<List<GioHangItem>>("GioHang");
            if (gio == null)
            {
                gio = new List<GioHangItem>();
                HttpContext.Session.SetObjectAsJson("GioHang", gio);
            }
            return gio;
        }

        [HttpPost]
        public IActionResult Them(int MaSP, List<int>? SelectedSizes, Dictionary<int, int>? SoLuong)
        {
            var gioHang = LayGioHang();

            foreach (var maSize in (SelectedSizes ?? Enumerable.Empty<int>()))
            {
                var sl = (SoLuong != null && SoLuong.TryGetValue(maSize, out var v)) ? v : 0;
                if (sl <= 0) continue;

                var sps = _context.SanPhamSizes
                    .Where(s => s.MaSP == MaSP && s.MaSize == maSize)
                    .Select(s => new { s.SanPham.TenSP, s.SanPham.Anh, s.Size.TenSize, s.Gia })
                    .FirstOrDefault();
                if (sps == null) continue;

                var ex = gioHang.FirstOrDefault(g => g.MaSP == MaSP && g.MaSize == maSize);
                if (ex != null) ex.SoLuong += sl;
                else
                {
                    gioHang.Add(new GioHangItem
                    {
                        MaSP = MaSP,
                        MaSize = maSize,
                        TenSP = sps.TenSP,
                        TenSize = sps.TenSize,
                        DonGia = sps.Gia,
                        SoLuong = sl,
                    });
                }
            }

            HttpContext.Session.SetObjectAsJson("GioHang", gioHang);
            return RedirectToAction("XemGioHang");
        }

        public IActionResult XemGioHang()
        {
            var gioHang = LayGioHang();
            return View(gioHang);
        }

        public IActionResult XoaItem(int MaSP, int MaSize)
        {
            var gio = LayGioHang();
            var item = gio.FirstOrDefault(x => x.MaSP == MaSP && x.MaSize == MaSize);
            if (item != null)
            {
                gio.Remove(item);
                HttpContext.Session.SetObjectAsJson("GioHang", gio);
                TempData["Message"] = $"❌ Đã xóa {item.TenSP} ({item.TenSize}) khỏi giỏ hàng.";
            }
            return RedirectToAction("XemGioHang");
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult DatHang(DatHangViewModel model)
        {
            var gioHang = LayGioHang();
            if (gioHang == null || !gioHang.Any())
                return RedirectToAction("XemGioHang");
            if (string.IsNullOrWhiteSpace(model.TenKhachHang) ||
                string.IsNullOrWhiteSpace(model.DiaChi) ||
                string.IsNullOrWhiteSpace(model.SoDienThoai))
            {
                TempData["Error"] = "Vui lòng nhập đầy đủ thông tin nhận hàng.";
                return RedirectToAction("XemGioHang");
            }
            var maKH = HttpContext.Session.GetInt32("MaKH");
            var hoaDon = new HoaDon
            {
                MaKH = maKH,                                  
                MaNV = null,                                  
                NgayLap = DateTime.Now,
                TongTien = gioHang.Sum(x => x.ThanhTien),
                TrangThai = "ChoXacNhan",
                MaHTTT = model.MaHTTT,
                TenNguoiNhan = model.TenKhachHang,
                DiaChiNhan = model.DiaChi,
                SoDienThoaiNhan = model.SoDienThoai
            };

            _context.HoaDons.Add(hoaDon);
            _context.SaveChanges();

            foreach (var item in gioHang)
            {
                var existing = _context.ChiTietHoaDons.FirstOrDefault(c =>
                    c.MaHD == hoaDon.MaHD && c.MaSP == item.MaSP && c.MaSize == item.MaSize);

                if (existing == null)
                {
                    _context.ChiTietHoaDons.Add(new ChiTietHoaDon
                    {
                        MaHD = hoaDon.MaHD,
                        MaSP = item.MaSP,
                        MaSize = item.MaSize,
                        SoLuong = item.SoLuong,
                        DonGia = item.DonGia
                    });
                }
                else
                {
                    existing.SoLuong += item.SoLuong;
                }
            }
            _context.SaveChanges();
            HttpContext.Session.Remove("GioHang");
            TempData["Success"] = "Đặt hàng thành công!";
            return RedirectToAction("DatHangThanhCong");
        }

        public IActionResult DatHangThanhCong() => View();
        [HttpPost]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult ThemAjax(int MaSP, List<int> SelectedSizes, Dictionary<int, int> SoLuong)
        {
            var gio = LayGioHang();

            foreach (var maSize in SelectedSizes ?? new())
            {
                if (SoLuong.TryGetValue(maSize, out var sl) && sl > 0)
                {
                    var sps = _context.SanPhamSizes
                        .Where(x => x.MaSP == MaSP && x.MaSize == maSize)
                        .Select(x => new { x.SanPham.TenSP, x.SanPham.Anh, x.Size.TenSize, x.Gia })
                        .FirstOrDefault();
                    if (sps == null) continue;

                    var ex = gio.FirstOrDefault(g => g.MaSP == MaSP && g.MaSize == maSize);
                    if (ex != null) ex.SoLuong += sl;
                    else
                    {
                        gio.Add(new GioHangItem
                        {
                            MaSP = MaSP,
                            MaSize = maSize,
                            TenSP = sps.TenSP,
                            TenSize = sps.TenSize,
                            DonGia = sps.Gia,
                            SoLuong = sl
                        });
                    }
                }
            }

            HttpContext.Session.SetObjectAsJson("GioHang", gio);
            return Content(RenderCartRows(gio), "text/html; charset=utf-8");
        }

        [HttpPost]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult XoaItemAjax(int MaSP, int MaSize)
        {
            var gio = LayGioHang();
            var it = gio.FirstOrDefault(x => x.MaSP == MaSP && x.MaSize == MaSize);
            if (it != null)
            {
                gio.Remove(it);
                HttpContext.Session.SetObjectAsJson("GioHang", gio);
            }
            return Content(RenderCartRows(gio), "text/html; charset=utf-8");
        }

        [HttpGet]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult MiniRows()
        {
            var gio = LayGioHang();
            return Content(RenderCartRows(gio), "text/html; charset=utf-8");
        }

        private string RenderCartRows(List<GioHangItem> gio)
        {
            if (gio == null || !gio.Any())
                return "<tr class='text-muted'><td colspan='5'>Giỏ hàng trống.</td></tr>";

            var sb = new StringBuilder();
            decimal tong = 0;
            foreach (var i in gio)
            {
                tong += i.ThanhTien;
                var ten = System.Net.WebUtility.HtmlEncode(i.TenSP);
                sb.Append($@"<tr>
                     <td class='truncate' title='{ten}'>{ten}</td>
                      <td class='text-center nowrap'>{System.Net.WebUtility.HtmlEncode(i.TenSize)}</td>
                      <td class='text-center nowrap'>{i.SoLuong}</td>
                      <td class='text-end nowrap'>{i.ThanhTien:N0} đ</td>
                      <td class='text-end'>
                        <button type='button' class='btn btn-sm btn-outline-danger' onclick='cartRemove({i.MaSP},{i.MaSize})'>✕</button>
                         </td>
                        /tr>");
            }
                                    sb.Append($@"<tr class='table-light fw-bold'>
                          <td colspan='3' class='text-end'>Tổng:</td>
                          <td class='text-end nowrap'>{tong:N0} đ</td>
                          <td></td>
                        </tr>");
            return sb.ToString();
        }

        [HttpPost]
        public IActionResult CapNhat(int MaSP, int MaSize, int SoLuong)
        {
            var gio = LayGioHang();
            var item = gio.FirstOrDefault(x => x.MaSP == MaSP && x.MaSize == MaSize);
            if (item != null)
            {
                if (SoLuong > 0)
                {
                    item.SoLuong = SoLuong;
                    TempData["Message"] = $"✅ Đã cập nhật số lượng cho {item.TenSP} ({item.TenSize}).";
                }
                else
                {
                    gio.Remove(item);
                    TempData["Message"] = $"❌ Đã xóa {item.TenSP} ({item.TenSize}) khỏi giỏ hàng.";
                }
                HttpContext.Session.SetObjectAsJson("GioHang", gio);
            }
            return RedirectToAction("XemGioHang");
        }

        [HttpPost]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult CapNhatAjax(int MaSP, int MaSize, int SoLuong)
        {
            var gio = LayGioHang();
            var item = gio.FirstOrDefault(x => x.MaSP == MaSP && x.MaSize == MaSize);
            if (item != null)
            {
                if (SoLuong > 0) item.SoLuong = SoLuong;
                else gio.Remove(item);

                HttpContext.Session.SetObjectAsJson("GioHang", gio);
            }
            return Content(RenderCartRows(gio), "text/html; charset=utf-8");
        }

        public IActionResult XoaHet()
        {
            HttpContext.Session.Remove("GioHang");
            TempData["Message"] = "🗑️ Giỏ hàng đã được làm trống.";
            return RedirectToAction("XemGioHang");
        }
    }
}
