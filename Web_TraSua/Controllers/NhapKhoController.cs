using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebTraSua.Data;
using WebTraSua.Models;
using System.Linq;
using System.Threading.Tasks;
using System;

public class NhapKhoController : Controller
{
    private readonly ApplicationDbContext _ctx;
    public NhapKhoController(ApplicationDbContext ctx) { _ctx = ctx; }

    private bool IsQuanLy() => HttpContext.Session.GetString("Role") == "QuanLy";
    public IActionResult Index()
    {
        if (!IsQuanLy()) return View("~/Views/Shared/TrangTrang.cshtml");

        var list = _ctx.NhapKhos
                       .Include(n => n.NguyenLieu)
                       .Include(n => n.NhanVien)
                       .OrderByDescending(n => n.MaNK)
                       .ToList();
        return View(list);
    }

    [HttpGet]
    public IActionResult Create()
    {
        if (!IsQuanLy()) return View("~/Views/Shared/TrangTrang.cshtml");

        ViewBag.AllNguyenLieu = _ctx.NguyenLieus.OrderBy(x => x.TenNL).ToList();
        ViewBag.AllNhanVien = _ctx.NhanViens.OrderBy(x => x.HoTen).ToList();

        return View(new NhapKho { NgayNhap = DateTime.Now });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NhapKho nhapKho)
    {
        ModelState.Remove(nameof(NhapKho.NhanVien));
        ModelState.Remove(nameof(NhapKho.NguyenLieu));
        if (nhapKho.NgayNhap.Year < 1900) nhapKho.NgayNhap = DateTime.Now;
        if (!ModelState.IsValid)
        {
            ViewBag.AllNguyenLieu = _ctx.NguyenLieus.ToList();
            ViewBag.AllNhanVien = _ctx.NhanViens.ToList();
            return View(nhapKho);
        }
        await _ctx.Database.ExecuteSqlRawAsync(
            @"INSERT INTO dbo.NhapKho (MaNL, SoLuong, NgayNhap, MaNV)
          VALUES ({0}, {1}, {2}, {3});",
            nhapKho.MaNL, nhapKho.SoLuong, nhapKho.NgayNhap, nhapKho.MaNV);
        return RedirectToAction(nameof(Index));
    }
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var nk = await _ctx.NhapKhos.FindAsync(id);
        if (nk == null) return NotFound();
        ViewBag.AllNguyenLieu = _ctx.NguyenLieus.OrderBy(x => x.TenNL).ToList();
        ViewBag.AllNhanVien = _ctx.NhanViens.OrderBy(x => x.HoTen).ToList();
        return View(nk);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit([Bind("MaNK,MaNL,SoLuong,NgayNhap,MaNV")] NhapKho nhapKho)
    {
        ModelState.Remove(nameof(NhapKho.NhanVien));
        ModelState.Remove(nameof(NhapKho.NguyenLieu));

        if (nhapKho.NgayNhap.Year < 1900) nhapKho.NgayNhap = DateTime.Now;

        if (!ModelState.IsValid)
        {
            ViewBag.AllNguyenLieu = _ctx.NguyenLieus.ToList();
            ViewBag.AllNhanVien = _ctx.NhanViens.ToList();
            return View(nhapKho);
        }
        var rows = await _ctx.Database.ExecuteSqlRawAsync(
            @"UPDATE dbo.NhapKho
          SET MaNL={0}, SoLuong={1}, NgayNhap={2}, MaNV={3}
          WHERE MaNK={4};",
            nhapKho.MaNL, nhapKho.SoLuong, nhapKho.NgayNhap, nhapKho.MaNV, nhapKho.MaNK);
        if (rows == 0) return NotFound(); 
        TempData["Success"] = $"Đã cập nhật phiếu nhập #{nhapKho.MaNK}";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _ctx.Database.ExecuteSqlRawAsync(
            "DELETE FROM dbo.NhapKho WHERE MaNK={0};", id);
        return RedirectToAction(nameof(Index));
    }
}
