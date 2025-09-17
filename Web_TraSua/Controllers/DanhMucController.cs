using Microsoft.AspNetCore.Mvc;
using WebTraSua.Data;
using WebTraSua.Models; // Thêm nếu chưa có

public class DanhMucController : Controller
{
    private readonly ApplicationDbContext _context;

    public DanhMucController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var list = _context.DanhMucSanPhams.ToList();
        return View(list);
    }

    //------------------- CHỈ DÀNH CHO QUẢN LÝ --------------------//

    private bool IsQuanLy()
    {
        return HttpContext.Session.GetString("Role") == "QuanLy";
    }

    [HttpGet]
    public IActionResult QuanLy()
    {
        if (!IsQuanLy()) return View("~/Views/Shared/TrangTrang.cshtml");

        var list = _context.DanhMucSanPhams.ToList();
        return View("Index", list); 

    }

    [HttpGet]
    public IActionResult Create()
    {
        if (!IsQuanLy()) return View("~/Views/Shared/TrangTrang.cshtml");
        return View();
    }

    [HttpPost]
    public IActionResult Create(DanhMucSanPham model)
    {
        if (!IsQuanLy()) return View("~/Views/Shared/TrangTrang.cshtml");

        if (ModelState.IsValid)
        {
            _context.DanhMucSanPhams.Add(model);
            _context.SaveChanges();
            return RedirectToAction("QuanLy");
        }
        return View(model);
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        if (!IsQuanLy()) return View("~/Views/Shared/TrangTrang.cshtml");

        var dm = _context.DanhMucSanPhams.Find(id);
        return View(dm);
    }

    [HttpPost]
    public IActionResult Edit(DanhMucSanPham model)
    {
        if (!IsQuanLy()) return View("~/Views/Shared/TrangTrang.cshtml");

        if (ModelState.IsValid)
        {
            _context.DanhMucSanPhams.Update(model);
            _context.SaveChanges();
            return RedirectToAction("QuanLy");
        }
        return View(model);
    }

    public IActionResult Delete(int id)
    {
        if (!IsQuanLy()) return View("~/Views/Shared/TrangTrang.cshtml");

        var dm = _context.DanhMucSanPhams.Find(id);
        if (dm != null)
        {
            _context.DanhMucSanPhams.Remove(dm);
            _context.SaveChanges();
        }
        return RedirectToAction("QuanLy");
    }
}
