using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneStore.Data;
using PhoneStore.ViewModels;

namespace PhoneStore.Controllers;

public class BaoCaoController : Controller
{
    private readonly ApplicationDbContext _context;
    public BaoCaoController(ApplicationDbContext context) => _context = context;

    public async Task<IActionResult> Index(DateTime? tuNgay, DateTime? denNgay)
    {
        var start = (tuNgay ?? new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1)).Date;
        var end = (denNgay ?? DateTime.Today).Date.AddDays(1);
        var orders = _context.DonHangs.AsNoTracking().Where(x => x.Ngaydat >= start && x.Ngaydat < end && x.Trangthai != 4);
        ViewBag.TuNgay = start.ToString("yyyy-MM-dd");
        ViewBag.DenNgay = end.AddDays(-1).ToString("yyyy-MM-dd");
        ViewBag.SoDon = await orders.CountAsync();
        ViewBag.DoanhThu = await orders.Where(x => x.Trangthai == 3).SumAsync(x => (decimal?)x.Tongtien) ?? 0m;
        ViewBag.DonChoXuLy = await orders.CountAsync(x => x.Trangthai == 0 || x.Trangthai == 1 || x.Trangthai == 2);
        var banChay = await _context.ChiTietDonHangs.AsNoTracking()
            .Where(x => x.Donhang.Ngaydat >= start && x.Donhang.Ngaydat < end && x.Donhang.Trangthai == 3)
            .GroupBy(x => x.Bienthe.Sanpham.Tensanpham)
            .Select(g => new BaoCaoSanPhamViewModel { TenSanPham = g.Key, SoLuong = g.Sum(x => x.Soluong), DoanhThu = g.Sum(x => x.Soluong * x.Dongia) })
            .OrderByDescending(x => x.SoLuong).Take(10).ToListAsync();
        return View(banChay);
    }
}
