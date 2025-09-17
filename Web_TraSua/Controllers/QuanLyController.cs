using Microsoft.AspNetCore.Mvc;

namespace Web_TraSua.Controllers
{
    public class QuanLyController : Controller
    {
        public IActionResult Index()
        {
            var hoTen = TempData["AdminName"]?.ToString() ?? "Quản lý";
            ViewBag.HoTen = hoTen;
            return View();
        }
    }
}
