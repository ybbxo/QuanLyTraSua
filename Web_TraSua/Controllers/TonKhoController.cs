using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebTraSua.Data;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace WebTraSua.Controllers
{
    public class TonKhoController : Controller
    {
        private readonly ApplicationDbContext _ctx;
        public TonKhoController(ApplicationDbContext ctx) { _ctx = ctx; }
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("Role") != "QuanLy")
                return View("~/Views/Shared/TrangTrang.cshtml");
            var conn = _ctx.Database.GetDbConnection();
            await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT nl.MaNL, nl.TenNL, nl.DonViTinh,
                       ISNULL(SUM(kps.SoLuong),0) AS Ton,
                       MAX(kps.Ngay) AS LastUpdated
                FROM dbo.NguyenLieu nl
                LEFT JOIN dbo.KhoPhatSinh kps ON kps.MaNL = nl.MaNL
                GROUP BY nl.MaNL, nl.TenNL, nl.DonViTinh
                ORDER BY nl.TenNL;
            ";
            var list = new List<TonKhoRowVM>();
            using var rd = await cmd.ExecuteReaderAsync();
            while (await rd.ReadAsync())
            {
                list.Add(new TonKhoRowVM
                {
                    MaNL = rd.GetInt32(0),
                    TenNL = rd.GetString(1),
                    DVT = rd.GetString(2),
                    Ton = rd.IsDBNull(3) ? 0 : rd.GetDecimal(3),
                    LastUpdated = rd.IsDBNull(4) ? (DateTime?)null : rd.GetDateTime(4)
                });
            }
            return View(list);
        }
    }
    public class TonKhoRowVM
    {
        public int MaNL { get; set; }
        public string TenNL { get; set; } = "";
        public string DVT { get; set; } = "";
        public decimal Ton { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}
