using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebTraSua.Data;

public class BaoCaoKhoController : Controller
{
    private readonly ApplicationDbContext _ctx;
    public BaoCaoKhoController(ApplicationDbContext ctx) { _ctx = ctx; }

    private bool IsQuanLy() => HttpContext.Session.GetString("Role") == "QuanLy";

    public async Task<IActionResult> TonHienTai()
    {
        if (!IsQuanLy()) return View("~/Views/Shared/TrangTrang.cshtml");
        var conn = _ctx.Database.GetDbConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT nl.MaNL, nl.TenNL, nl.DonViTinh, ISNULL(t.Ton,0) Ton
            FROM dbo.NguyenLieu nl
            LEFT JOIN dbo.v_TonKhoHienTai t ON t.MaNL = nl.MaNL
            ORDER BY nl.TenNL";
        var rows = new List<(int MaNL, string TenNL, string DVT, decimal Ton)>();
        using (var rd = await cmd.ExecuteReaderAsync())
            while (await rd.ReadAsync())
                rows.Add((rd.GetInt32(0), rd.GetString(1), rd.GetString(2), rd.IsDBNull(3) ? 0 : rd.GetDecimal(3)));
        return View(rows);
    }

    public async Task<IActionResult> TieuThu(DateTime? tu = null, DateTime? den = null)
    {
        if (!IsQuanLy()) return View("~/Views/Shared/TrangTrang.cshtml");
        var from = (tu ?? DateTime.Today.AddDays(-7)).Date;
        var to = (den ?? DateTime.Today).Date.AddDays(1).AddTicks(-1);

        var conn = _ctx.Database.GetDbConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT nl.MaNL, nl.TenNL, nl.DonViTinh, SUM(-kps.SoLuong) AS Xuat
            FROM dbo.KhoPhatSinh kps
            JOIN dbo.NguyenLieu nl ON nl.MaNL = kps.MaNL
            WHERE kps.Loai='Xuat' AND kps.Ngay BETWEEN @from AND @to
            GROUP BY nl.MaNL, nl.TenNL, nl.DonViTinh
            ORDER BY Xuat DESC";
        var p1 = cmd.CreateParameter(); p1.ParameterName = "@from"; p1.Value = from; cmd.Parameters.Add(p1);
        var p2 = cmd.CreateParameter(); p2.ParameterName = "@to"; p2.Value = to; cmd.Parameters.Add(p2);

        var rows = new List<(int MaNL, string TenNL, string DVT, decimal Xuat)>();
        using (var rd = await cmd.ExecuteReaderAsync())
            while (await rd.ReadAsync())
                rows.Add((rd.GetInt32(0), rd.GetString(1), rd.GetString(2), rd.IsDBNull(3) ? 0 : rd.GetDecimal(3)));
        ViewBag.From = from; ViewBag.To = to;
        return View(rows);
    }
}
