using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebTraSua.Data;
using WebTraSua.Models;

namespace WebTraSua.Controllers
{
    public class ThongKeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ThongKeController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> DoanhThuTheoNgay(DateTime? ngayBatDau, DateTime? ngayKetThuc)
        {
            DateTime fromDate = ngayBatDau ?? DateTime.Today.AddDays(-6);
            DateTime toDate = ngayKetThuc ?? DateTime.Today;
            if (fromDate > toDate)
                (fromDate, toDate) = (toDate, fromDate);
            var data = await _context.HoaDons
                .Where(h => h.NgayLap.Date >= fromDate && h.NgayLap.Date <= toDate && h.TrangThai == "HoanTat")
                .GroupBy(h => h.NgayLap.Date)
                .Select(g => new DoanhThuNgay
                {
                    Ngay = g.Key,
                    DoanhThu = g.Sum(x => x.TongTien)
                })
                .ToListAsync();
            var allDates = Enumerable.Range(0, (toDate - fromDate).Days + 1)
                .Select(i => fromDate.AddDays(i))
                .ToList();
            var resultList = allDates.Select(d =>
                data.FirstOrDefault(x => x.Ngay == d) ?? new DoanhThuNgay { Ngay = d, DoanhThu = 0 }
            ).ToList();

            var vm = new ThongKeDoanhThuTheoNgayViewModel
            {
                NgayBatDau = fromDate,
                NgayKetThuc = toDate,
                DoanhThuTheoNgay = resultList
            };
            return View(vm);
        }
        public async Task<IActionResult> DoanhThuTheoThang(int? nam)
        {
            int year = nam ?? DateTime.Today.Year;

            var data = await _context.HoaDons
                .Where(h => h.NgayLap.Year == year && h.TrangThai == "HoanTat")
                .GroupBy(h => h.NgayLap.Month)
                .Select(g => new DoanhThuThang
                {
                    Thang = g.Key,
                    DoanhThu = g.Sum(x => x.TongTien)
                })
                .ToListAsync();
            var resultList = Enumerable.Range(1, 12)
                .Select(m => data.FirstOrDefault(x => x.Thang == m) ?? new DoanhThuThang { Thang = m, DoanhThu = 0 })
                .ToList();
            var vm = new ThongKeDoanhThuTheoThangViewModel
            {
                Nam = year,
                DoanhThuTheoThang = resultList
            };
            return View(vm);
        }
    }
}
    