using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneStore.Data;
using PhoneStore.Models;
using PhoneStore.ViewModels;

namespace PhoneStore.Controllers;

public class ThanhToanController : Controller
{
    private readonly ApplicationDbContext _context;
    public ThanhToanController(ApplicationDbContext context) => _context = context;

    [HttpGet]
    public IActionResult Index()
    {
        if (!GioHangController.DocGioHang(HttpContext).Any()) return RedirectToAction("Index", "GioHang");
        return View(new ThanhToanViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ThanhToanViewModel model)
    {
        var cart = GioHangController.DocGioHang(HttpContext);
        if (!cart.Any()) ModelState.AddModelError(string.Empty, "Giỏ hàng đang trống.");
        if (model.PhuongThucThanhToan != "COD" && model.PhuongThucThanhToan != "ChuyenKhoan")
            ModelState.AddModelError(nameof(model.PhuongThucThanhToan), "Phương thức thanh toán không hợp lệ.");

        var ids = cart.Select(x => x.Bientheid).ToList();
        var variants = await _context.BienTheSanphams.Include(x => x.Sanpham)
            .Where(x => ids.Contains(x.Bientheid) && x.Kichhoat && x.Sanpham.Kichhoat).ToListAsync();
        if (variants.Count != cart.Count) ModelState.AddModelError(string.Empty, "Có sản phẩm không còn khả dụng. Vui lòng kiểm tra lại giỏ hàng.");
        foreach (var item in cart)
        {
            var variant = variants.FirstOrDefault(x => x.Bientheid == item.Bientheid);
            if (variant == null || item.SoLuong > variant.Soluongton)
                ModelState.AddModelError(string.Empty, $"Sản phẩm {item.TenSanPham} không đủ tồn kho.");
        }
        if (!ModelState.IsValid) return View(model);

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            decimal tongTien = 0;
            var donHang = new DonHang
            {
                Madonhang = "DH" + DateTime.Now.ToString("yyMMddHHmmss") + Random.Shared.Next(10, 99),
                Tennguoinhan = model.TenNguoiNhan.Trim(), Sdtnguoinhan = model.SdtNguoiNhan.Trim(),
                Diachigiaohang = model.DiaChiGiaoHang.Trim(), Phuongthucthanhtoan = model.PhuongThucThanhToan,
                Trangthai = 0, Ngaydat = DateTime.Now, Ghichu = model.GhiChu
            };
            _context.DonHangs.Add(donHang);
            await _context.SaveChangesAsync();

            foreach (var item in cart)
            {
                var bt = variants.Single(x => x.Bientheid == item.Bientheid);
                var gia = bt.Giakhuyenmai ?? bt.Giaban;
                bt.Soluongton -= item.SoLuong;
                _context.ChiTietDonHangs.Add(new ChiTietDonHang
                {
                    Donhangid = donHang.Donhangid, Bientheid = bt.Bientheid,
                    Soluong = item.SoLuong, Dongia = gia
                });
                tongTien += gia * item.SoLuong;
            }
            donHang.Tongtien = tongTien;
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            GioHangController.XoaGioHang(HttpContext);
            return RedirectToAction(nameof(ThanhCong), new { maDon = donHang.Madonhang });
        }
        catch
        {
            await transaction.RollbackAsync();
            ModelState.AddModelError(string.Empty, "Không thể tạo đơn hàng. Vui lòng thử lại.");
            return View(model);
        }
    }

    public async Task<IActionResult> ThanhCong(string maDon)
    {
        var don = await _context.DonHangs.AsNoTracking().FirstOrDefaultAsync(x => x.Madonhang == maDon);
        if (don == null) return NotFound();
        return View(don);
    }
}
