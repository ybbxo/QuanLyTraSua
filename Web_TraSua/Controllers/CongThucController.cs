using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_TraSua.ViewModels;
using WebTraSua.Data;
using WebTraSua.ViewModels;
using WebTraSua.Models;
public class CongThucController : Controller
{
    private readonly ApplicationDbContext _ctx;
    public CongThucController(ApplicationDbContext ctx) { _ctx = ctx; }

    private bool IsQuanLy() => HttpContext.Session.GetString("Role") == "QuanLy";

    [HttpGet]
    public async Task<IActionResult> Edit(int maSP)
    {
        if (!IsQuanLy()) return View("~/Views/Shared/TrangTrang.cshtml");

        var sp = await _ctx.SanPhams.FindAsync(maSP);
        if (sp == null) return NotFound();

        var baseSize = await _ctx.Sizes.FirstAsync(s => s.IsBase);
        var rows = await (from ct in _ctx.CongThucPhaChes
                          join nl in _ctx.NguyenLieus on ct.MaNL equals nl.MaNL
                          where ct.MaSP == maSP
                          select new CongThucRow
                          {
                              MaNL = nl.MaNL,
                              TenNL = nl.TenNL,
                              DonViTinh = nl.DonViTinh,
                              SoLuongCoSo = ct.SoLuongCoSo
                          }).ToListAsync();

        var vm = new CongThucEditVM
        {
            MaSP = maSP,
            TenSP = sp.TenSP,
            MaSizeBase = baseSize.MaSize,
            DungTichBase = baseSize.DungTichML,
            Rows = rows ?? new List<CongThucRow>(), 
            AllNguyenLieu = await _ctx.NguyenLieus.OrderBy(n => n.TenNL).ToListAsync() ?? new List<NguyenLieu>()

        };

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(CongThucEditVM vm)
    {
        if (!IsQuanLy()) return View("~/Views/Shared/TrangTrang.cshtml");
        if (vm.Rows == null || vm.AllNguyenLieu == null)
        {
            ModelState.AddModelError("", "Dữ liệu không hợp lệ, vui lòng thử lại.");
            return View(vm);
        }

        var baseSize = await _ctx.Sizes.FirstAsync(s => s.IsBase);
        var tongML = vm.Rows.Where(r => r.DonViTinh == "ml").Sum(r => r.SoLuongCoSo);
        if (tongML > baseSize.DungTichML)
        {
            ModelState.AddModelError("", $"Tổng ml ({tongML}) vượt dung tích size base {baseSize.TenSize} = {baseSize.DungTichML} ml.");
        }

        if (!ModelState.IsValid)
        {
            vm.AllNguyenLieu = await _ctx.NguyenLieus.OrderBy(n => n.TenNL).ToListAsync();
            return View(vm);
        }
        var olds = _ctx.CongThucPhaChes.Where(c => c.MaSP == vm.MaSP);
        _ctx.CongThucPhaChes.RemoveRange(olds);
        foreach (var r in vm.Rows)
        {
            _ctx.CongThucPhaChes.Add(new CongThucPhaChe
            {
                MaSP = vm.MaSP,
                MaNL = r.MaNL,
                SoLuongCoSo = r.SoLuongCoSo
            });
        }
        await _ctx.SaveChangesAsync();

        TempData["Success"] = "Đã lưu công thức size base. (Size M/L sẽ tự nhân theo HeSoDinhLuong)";
        return RedirectToAction("Edit", new { maSP = vm.MaSP });
    }
    [HttpPost]
    public async Task<IActionResult> CheckTongMl([FromBody] List<CongThucRow> rows)
    {
        var baseSize = await _ctx.Sizes.FirstAsync(s => s.IsBase);
        var tongML = rows.Where(r => r.DonViTinh == "ml").Sum(r => r.SoLuongCoSo);
        var hopLe = tongML <= baseSize.DungTichML;
        return Ok(new { tongML, gioiHan = baseSize.DungTichML, hopLe });
    }
}
