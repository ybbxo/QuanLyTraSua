using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebTraSua.Data;

namespace WebTraSua.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NguyenLieuController : ControllerBase
    {
        private readonly ApplicationDbContext _ctx;
        public NguyenLieuController(ApplicationDbContext ctx) { _ctx = ctx; }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _ctx.NguyenLieus
            .OrderBy(n => n.MaNL)          
            .Select(n => new { n.MaNL, n.TenNL, n.DonViTinh })
            .ToListAsync();


            return Ok(list);
        }
    }
}
