using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneStore.Data;
using PhoneStore.ViewModels;

namespace PhoneStore.Controllers;

public class GioHangController : Controller
{
    private const string CartKey = "PHONESTORE_CART";
    private readonly ApplicationDbContext _context;
    public GioHangController(ApplicationDbContext context) => _context = context;

    private List<GioHangItem> GetCart() =>
        JsonSerializer.Deserialize<List<GioHangItem>>(HttpContext.Session.GetString(CartKey) ?? "[]") ?? new();
    private void SaveCart(List<GioHangItem> cart) => HttpContext.Session.SetString(CartKey, JsonSerializer.Serialize(cart));

    public IActionResult Index() => View(GetCart());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Them(int bientheid, int soLuong = 1)
    {
        if (soLuong < 1) soLuong = 1;
        var bt = await _context.BienTheSanphams.Include(x => x.Sanpham)
            .FirstOrDefaultAsync(x => x.Bientheid == bientheid && x.Kichhoat && x.Sanpham.Kichhoat);
        if (bt == null) return NotFound();
        if (bt.Soluongton < 1) { TempData["Loi"] = "Sản phẩm đã hết hàng."; return RedirectToAction("Details", "CuaHang", new { id = bt.Sanphamid }); }

        var cart = GetCart();
        var item = cart.FirstOrDefault(x => x.Bientheid == bientheid);
        if (item == null)
        {
            cart.Add(new GioHangItem { Bientheid = bt.Bientheid, Sanphamid = bt.Sanphamid,
                TenSanPham = bt.Sanpham.Tensanpham, HinhAnh = bt.Hinhanhbienthe ?? bt.Sanpham.Hinhanh,
                DungLuong = bt.Dungluong, MauSac = bt.Mausac,
                DonGia = bt.Giakhuyenmai ?? bt.Giaban, SoLuong = Math.Min(soLuong, bt.Soluongton), TonKho = bt.Soluongton });
        }
        else item.SoLuong = Math.Min(item.SoLuong + soLuong, bt.Soluongton);
        SaveCart(cart);
        TempData["ThongBao"] = "Đã thêm sản phẩm vào giỏ hàng.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CapNhat(int bientheid, int soLuong)
    {
        var cart = GetCart();
        var item = cart.FirstOrDefault(x => x.Bientheid == bientheid);
        if (item != null)
        {
            var bt = await _context.BienTheSanphams.FindAsync(bientheid);
            if (bt == null || soLuong <= 0) cart.Remove(item);
            else item.SoLuong = Math.Min(soLuong, bt.Soluongton);
            SaveCart(cart);
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Xoa(int bientheid)
    {
        var cart = GetCart(); cart.RemoveAll(x => x.Bientheid == bientheid); SaveCart(cart);
        return RedirectToAction(nameof(Index));
    }

    internal static List<GioHangItem> DocGioHang(HttpContext context) =>
        JsonSerializer.Deserialize<List<GioHangItem>>(context.Session.GetString(CartKey) ?? "[]") ?? new();
    internal static void XoaGioHang(HttpContext context) => context.Session.Remove(CartKey);
}
