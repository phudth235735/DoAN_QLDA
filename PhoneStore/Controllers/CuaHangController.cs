using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneStore.Data;
using PhoneStore.Models;

namespace PhoneStore.Controllers;

public class CuaHangController : Controller
{
    private readonly ApplicationDbContext _context;
    public CuaHangController(ApplicationDbContext context) => _context = context;

    public async Task<IActionResult> Index(string? tuKhoa, int? hangId, int? danhMucId,
        string? ram, decimal? giaTu, decimal? giaDen, string? sapXep)
    {
        var query = _context.SanPhams
            .AsNoTracking()
            .Include(s => s.Hang)
            .Include(s => s.Danhmuc)
            .Include(s => s.BienTheSanphams)
            .Where(s => s.Kichhoat);

        if (!string.IsNullOrWhiteSpace(tuKhoa))
            query = query.Where(s => s.Tensanpham.Contains(tuKhoa) || (s.Chip != null && s.Chip.Contains(tuKhoa)));
        if (hangId.HasValue) query = query.Where(s => s.Hangid == hangId.Value);
        if (danhMucId.HasValue) query = query.Where(s => s.Danhmucid == danhMucId.Value);
        if (!string.IsNullOrWhiteSpace(ram)) query = query.Where(s => s.Ram != null && s.Ram.Contains(ram));
        if (giaTu.HasValue || giaDen.HasValue)
            query = query.Where(s => s.BienTheSanphams.Any(b =>
                (!giaTu.HasValue || (b.Giakhuyenmai ?? b.Giaban) >= giaTu.Value) &&
                (!giaDen.HasValue || (b.Giakhuyenmai ?? b.Giaban) <= giaDen.Value)));

        query = sapXep switch
        {
            "gia-tang" => query.OrderBy(s => s.BienTheSanphams.Min(b => (decimal?)(b.Giakhuyenmai ?? b.Giaban))),
            "gia-giam" => query.OrderByDescending(s => s.BienTheSanphams.Max(b => (decimal?)(b.Giakhuyenmai ?? b.Giaban))),
            "xem-nhieu" => query.OrderByDescending(s => s.Luotxem),
            _ => query.OrderByDescending(s => s.Noibat).ThenByDescending(s => s.Ngaytao)
        };

        ViewBag.TuKhoa = tuKhoa;
        ViewBag.HangId = hangId;
        ViewBag.DanhMucId = danhMucId;
        ViewBag.Ram = ram;
        ViewBag.GiaTu = giaTu;
        ViewBag.GiaDen = giaDen;
        ViewBag.SapXep = sapXep;
        ViewBag.Hangs = await _context.HangSanXuats.Where(x => x.Kichhoat).OrderBy(x => x.Thutu).ToListAsync();
        ViewBag.DanhMucs = await _context.DanhMucs.Where(x => x.Kichhoat).OrderBy(x => x.Thutu).ToListAsync();
        return View(await query.ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var sp = await _context.SanPhams
            .Include(s => s.Hang).Include(s => s.Danhmuc)
            .Include(s => s.BienTheSanphams.Where(b => b.Kichhoat && b.Soluongton >= 0))
            .Include(s => s.BinhLuans.Where(b => b.Kichhoat).OrderByDescending(b => b.Ngaybinhluan))
            .FirstOrDefaultAsync(s => s.Sanphamid == id && s.Kichhoat);
        if (sp == null) return NotFound();
        sp.Luotxem++;
        await _context.SaveChangesAsync();
        return View(sp);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DanhGia(int sanphamid, string nguoiDang, int soSao, string noiDung)
    {
        if (soSao < 1 || soSao > 5 || string.IsNullOrWhiteSpace(noiDung) || string.IsNullOrWhiteSpace(nguoiDang))
        {
            TempData["Loi"] = "Vui lòng nhập tên, nội dung và chọn số sao từ 1 đến 5.";
            return RedirectToAction(nameof(Details), new { id = sanphamid });
        }
        if (!await _context.SanPhams.AnyAsync(s => s.Sanphamid == sanphamid && s.Kichhoat)) return NotFound();
        _context.BinhLuans.Add(new BinhLuan
        {
            Sanphamid = sanphamid, Nguoidang = nguoiDang.Trim(), Sosao = soSao,
            Noidung = noiDung.Trim(), Ngaybinhluan = DateTime.Now, Kichhoat = true
        });
        await _context.SaveChangesAsync();
        TempData["ThongBao"] = "Cảm ơn bạn đã đánh giá sản phẩm!";
        return RedirectToAction(nameof(Details), new { id = sanphamid });
    }
}
