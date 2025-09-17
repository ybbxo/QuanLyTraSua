using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_TraSua.Models;
using WebTraSua.Data;
using WebTraSua.Models;
using System.Linq;
using Web_TraSua.ViewModels;

namespace WebTraSua.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SanPhamController(ApplicationDbContext context)
        {
            _context = context;
        }
        private bool IsQuanLy()
        {
            return HttpContext.Session.GetString("Role") == "QuanLy";
        }
        public IActionResult Index()
        {
            var list = _context.SanPhams
                .Include(sp => sp.DanhMuc)
                .Include(sp => sp.SanPhamSizes).ThenInclude(sps => sps.Size)
                .ToList();
            ViewBag.DanhMucs = _context.DanhMucSanPhams.ToList();
            ViewBag.SizeList = _context.Sizes.Select(s => new SizeGiaModel
            {
                MaSize = s.MaSize,
                TenSize = s.TenSize,
                Gia = 0,
                DuocChon = false
            }).ToList();
            ViewBag.SizeMeta = _context.Sizes
                .Select(s => new {
                    s.MaSize,
                    s.TenSize,
                    s.IsBase,
                    s.HeSoGia,
                    s.PhuThu,
                    s.HeSoDinhLuong,
                    s.DungTichML
                }).ToList();
            ViewBag.NguyenLieus = _context.NguyenLieus
                .OrderBy(n => n.TenNL)
                .Select(n => new { n.MaNL, n.TenNL, n.DonViTinh })
                .ToList();
            ViewBag.DungTichS = _context.Sizes.FirstOrDefault(x => x.IsBase).DungTichML;
            return View(list);
        }
        [HttpPost]
        public IActionResult Create(SanPhamCreateViewModel model, List<CongThucRow> CongThuc)
        {
            if (HttpContext.Session.GetString("Role") != "QuanLy")
                return RedirectToAction("Login", "Account");

            var isExist = _context.SanPhams.Any(s => s.TenSP == model.TenSP);
            if (isExist)
            {
                TempData["Error"] = "❌ Sản phẩm đã tồn tại!";
            }
            else if (ModelState.IsValid)
            {
                var sp = new SanPham { TenSP = model.TenSP, MaDM = model.MaDM };
                _context.SanPhams.Add(sp);
                _context.SaveChanges();
                foreach (var size in model.Sizes ?? new())
                {
                    if (size.DuocChon)
                    {
                        _context.SanPhamSizes.Add(new SanPhamSize
                        {
                            MaSP = sp.MaSP,
                            MaSize = size.MaSize,
                            Gia = size.Gia
                        });
                    }
                }
                var baseSize = _context.Sizes.First(s => s.IsBase);
                var tongMl = (CongThuc ?? new()).Where(x => x.DonViTinh == "ml").Sum(x => x.SoLuongCoSo);
                if (tongMl > baseSize.DungTichML)
                {
                    TempData["Error"] = $"Tổng ml base ({tongMl}) vượt dung tích size {baseSize.TenSize} = {baseSize.DungTichML} ml.";
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
                foreach (var r in CongThuc ?? new())
                {
                    _context.CongThucPhaChes.Add(new CongThucPhaChe
                    {
                        MaSP = sp.MaSP,
                        MaNL = r.MaNL,
                        SoLuongCoSo = r.SoLuongCoSo
                    });
                }
                _context.SaveChanges();
                TempData["Success"] = "✔️ Thêm sản phẩm & công thức thành công!";
                return RedirectToAction("Index");
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["Error"] = "❌ Lỗi: " + string.Join(" | ", errors);
            }
            var list = _context.SanPhams
                .Include(sp => sp.DanhMuc)
                .Include(sp => sp.SanPhamSizes).ThenInclude(sps => sps.Size)
                .ToList();
            ViewBag.DanhMucs = _context.DanhMucSanPhams.ToList();
            ViewBag.SizeList = _context.Sizes.Select(s => new SizeGiaModel
            {
                MaSize = s.MaSize,
                TenSize = s.TenSize,
                Gia = 0,
                DuocChon = false
            }).ToList();
            ViewBag.SizeMeta = _context.Sizes.Select(s => new {
                s.MaSize,
                s.TenSize,
                s.IsBase,
                s.HeSoGia,
                s.PhuThu,
                s.HeSoDinhLuong,
                s.DungTichML
            }).ToList();
            ViewBag.NguyenLieus = _context.NguyenLieus
                .OrderBy(n => n.TenNL)
                .Select(n => new { n.MaNL, n.TenNL, n.DonViTinh })
                .ToList();
            ViewBag.DungTichS = _context.Sizes.FirstOrDefault(x => x.IsBase).DungTichML;
            return View("Index", list);
        }
        [HttpPost]
        public IActionResult EditFromIndex(int MaSP, string TenSP, int MaDM, decimal GiaS)
        {
            if (HttpContext.Session.GetString("Role") != "QuanLy")
                return RedirectToAction("Login", "Account");
            var sp = _context.SanPhams
                .Include(x => x.SanPhamSizes)
                .FirstOrDefault(x => x.MaSP == MaSP);
            if (sp == null)
            {
                TempData["Error"] = "Sản phẩm không tồn tại!";
                return RedirectToAction("Index");
            }
            sp.TenSP = TenSP;
            sp.MaDM = MaDM; 
            var sMeta = _context.Sizes.FirstOrDefault(s => s.IsBase);     
            var mMeta = _context.Sizes.FirstOrDefault(s => s.TenSize == "M");
            var lMeta = _context.Sizes.FirstOrDefault(s => s.TenSize == "L");
            decimal RoundTo1k(decimal v)
                => Math.Round(v / 1000m, MidpointRounding.AwayFromZero) * 1000m;
            if (sMeta != null)
            {
                var rowS = sp.SanPhamSizes.FirstOrDefault(x => x.MaSize == sMeta.MaSize);
                if (rowS == null)
                    _context.SanPhamSizes.Add(new SanPhamSize { MaSP = MaSP, MaSize = sMeta.MaSize, Gia = GiaS });
                else
                    rowS.Gia = GiaS;
            }
            if (mMeta != null)
            {
                var rowM = sp.SanPhamSizes.FirstOrDefault(x => x.MaSize == mMeta.MaSize);
                if (rowM != null)
                    rowM.Gia = RoundTo1k(GiaS * mMeta.HeSoGia + mMeta.PhuThu);
            }
            if (lMeta != null)
            {
                var rowL = sp.SanPhamSizes.FirstOrDefault(x => x.MaSize == lMeta.MaSize);
                if (rowL != null)
                    rowL.Gia = RoundTo1k(GiaS * lMeta.HeSoGia + lMeta.PhuThu);
            }
            _context.SaveChanges();
            TempData["Success"] = "✅ Đã cập nhật sản phẩm và tự tính giá M/L từ S.";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult GetCongThucJson(int id) 
        {
            var rows = _context.CongThucPhaChes
                .Where(c => c.MaSP == id)
                .Join(_context.NguyenLieus, c => c.MaNL, n => n.MaNL,
                    (c, n) => new {
                        maNL = n.MaNL,
                        tenNL = n.TenNL,
                        donViTinh = n.DonViTinh,
                        soLuongCoSo = c.SoLuongCoSo
                    })
                .OrderBy(x => x.tenNL)
                .ToList();
            return Json(rows);
        }
        [HttpGet]
        public IActionResult GetSanPhamJson(int id)
        {
            var sp = _context.SanPhams
                .Include(x => x.SanPhamSizes)
                .FirstOrDefault(x => x.MaSP == id);
            if (sp == null) return NotFound();
            var sizes = sp.SanPhamSizes
                .Select(s => new { maSize = s.MaSize, gia = s.Gia })
                .ToList();
            return Json(new
            {
                maSP = sp.MaSP,
                tenSP = sp.TenSP,
                maDM = sp.MaDM,
                sizes
            });
        }
        public IActionResult DanhMuc(int id)
        {
            HttpContext.Session.SetInt32("LastDanhMuc", id);
            var sanPhams = _context.SanPhams
                .Where(sp => sp.MaDM == id) 
                .Include(sp => sp.DanhMuc)
                .Include(sp => sp.SanPhamSizes)
                    .ThenInclude(sps => sps.Size)
                .ToList();

            return View(sanPhams);
        }

        [HttpGet]
        public IActionResult GetNguyenLieuByDanhMuc(int maDM)
        {
            var list = _context.DanhMucNguyenLieus
                        .Where(x => x.MaDM == maDM)
                        .Select(x => new {
                            maNL = x.MaNL,
                            tenNL = x.NguyenLieu.TenNL,
                            donViTinh = x.NguyenLieu.DonViTinh
                        })
                        .ToList();
            return Json(list);
        }
        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("Role") != "QuanLy")
                return RedirectToAction("Login", "Account");
            var sp = _context.SanPhams
                .Include(s => s.SanPhamSizes)
                .FirstOrDefault(s => s.MaSP == id);
            if (sp == null) return RedirectToAction("Index");
            try
            {
                var cts = _context.CongThucPhaChes.Where(x => x.MaSP == id).ToList();
                _context.CongThucPhaChes.RemoveRange(cts);
                var sizes = _context.SanPhamSizes.Where(s => s.MaSP == id).ToList();
                _context.SanPhamSizes.RemoveRange(sizes);
                _context.SanPhams.Remove(sp);
                _context.SaveChanges();
                TempData["Success"] = "🗑️ Đã xoá sản phẩm.";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "❌ Không thể xoá vì sản phẩm đã phát sinh hoá đơn.";
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult TimKiem(string keyword)
        {
            var query = _context.SanPhams
                .Include(sp => sp.DanhMuc)
                .Include(sp => sp.SanPhamSizes).ThenInclude(sps => sps.Size)
                .AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(sp => sp.TenSP.Contains(keyword));
            }
            var list = query.ToList();
            ViewBag.Keyword = keyword;
            return View(list);
        }
        public async Task<IActionResult> ChiTiet(int id)
        {
            var sanPham = await _context.SanPhams
                .Include(sp => sp.DanhMuc)
                .Include(sp => sp.SanPhamSizes)
                    .ThenInclude(sps => sps.Size)
                 .Include(sp => sp.BinhLuans)  
                .FirstOrDefaultAsync(sp => sp.MaSP == id);
            if (sanPham == null)
            {
                return NotFound();
            }
            HttpContext.Session.SetInt32("LastDanhMuc", sanPham.MaDM);
            return View(sanPham);
        }
        [HttpPost]
        public IActionResult ThemBinhLuan(int MaSP, string TenNguoiDung, string NoiDung, int Sao)
        {
            if (string.IsNullOrWhiteSpace(TenNguoiDung) || string.IsNullOrWhiteSpace(NoiDung) || Sao < 1 || Sao > 5)
            {
                TempData["Error"] = "Dữ liệu không hợp lệ.";
                return RedirectToAction("ChiTiet", new { id = MaSP });
            }
            var binhLuan = new BinhLuan
            {
                MaSP = MaSP,
                TenNguoiDung = TenNguoiDung.Trim(),
                NoiDung = NoiDung.Trim(),
                Sao = Sao,
                NgayBinhLuan = DateTime.Now
            };
            _context.BinhLuans.Add(binhLuan);
            _context.SaveChanges();
            return RedirectToAction("ChiTiet", new { id = MaSP });
        }
    }
}
