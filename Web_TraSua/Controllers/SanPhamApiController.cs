using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Web_TraSua.ViewModels;
using WebTraSua.ViewModels;

[Route("api/sanpham")]
[ApiController]
public class SanPhamApiController : ControllerBase
{
    private readonly string _connStr;
    public SanPhamApiController(IConfiguration cfg)
    {
        _connStr = cfg.GetConnectionString("DefaultConnection");
    }

    [HttpGet("ct-theo-size")]
    public async Task<ActionResult<SanPhamChiTietBySizeDto>> GetCTBySize([FromQuery] int maSP, [FromQuery] int maSize)
    {
        using var conn = new SqlConnection(_connStr);
        await conn.OpenAsync();

        using var multi = await conn.QueryMultipleAsync(
            "usp_GetSanPhamChiTiet_BySize",
            new { MaSP = maSP, MaSize = maSize },
            commandType: System.Data.CommandType.StoredProcedure
        );

        var sp = await multi.ReadFirstOrDefaultAsync<(int MaSP, string TenSP, int MaSize, string TenSize, decimal Gia)>();
        if (sp.MaSP == 0) return NotFound();

        var nl = (await multi.ReadAsync<NguyenLieuTheoSizeDto>()).ToList();

        var dto = new SanPhamChiTietBySizeDto
        {
            MaSP = sp.MaSP,
            TenSP = sp.TenSP,
            MaSize = sp.MaSize,
            TenSize = sp.TenSize,
            Gia = sp.Gia,
            NguyenLieus = nl
        };
        return dto;
    }

    [HttpGet("check-ml")]
    public async Task<ActionResult<object>> CheckMl([FromQuery] int maSP)
    {
        using var conn = new SqlConnection(_connStr);
        var rows = await conn.QueryAsync(
            "usp_CheckTheTichSanPham",
            new { MaSP = maSP },
            commandType: System.Data.CommandType.StoredProcedure
        );
        return Ok(rows);
    }
}
