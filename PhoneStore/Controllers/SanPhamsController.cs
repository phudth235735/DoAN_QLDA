using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PhoneStore.Data;
using PhoneStore.Models;

namespace PhoneStore.Controllers
{
    public class SanPhamsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SanPhamsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SanPhams
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.SanPhams.Include(s => s.Danhmuc).Include(s => s.Hang).Include(s => s.Nhucau);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: SanPhams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams
                .Include(s => s.Danhmuc)
                .Include(s => s.Hang)
                .Include(s => s.Nhucau)
                .Include(s => s.BienTheSanphams)
                .Include(s => s.KhuyenMaiSanPhams)
                .Include(s => s.BinhLuans)
                .FirstOrDefaultAsync(m => m.Sanphamid == id);
            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }

        // GET: SanPhams/Create
        public IActionResult Create()
        {
            ViewData["Danhmucid"] = new SelectList(_context.DanhMucs, "Danhmucid", "Danhmucid");
            ViewData["Hangid"] = new SelectList(_context.HangSanXuats, "Hangid", "Hangid");
            ViewData["Nhucauid"] = new SelectList(_context.NhuCaus, "Nhucauid", "Nhucauid");
            return View();
        }

        // POST: SanPhams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Sanphamid,Danhmucid,Hangid,Nhucauid,Tensanpham,Manhinh,Chip,Ram,Camera,Pin,Hedieuhanh,Tomtat,Mota,Hinhanh,Hotsale,Tragop0,Trogiathucu,Luotxem,Noibat,Kichhoat,Ngaytao")] SanPham sanPham)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sanPham);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Danhmucid"] = new SelectList(_context.DanhMucs, "Danhmucid", "Danhmucid", sanPham.Danhmucid);
            ViewData["Hangid"] = new SelectList(_context.HangSanXuats, "Hangid", "Hangid", sanPham.Hangid);
            ViewData["Nhucauid"] = new SelectList(_context.NhuCaus, "Nhucauid", "Nhucauid", sanPham.Nhucauid);
            return View(sanPham);
        }

        // GET: SanPhams/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham == null)
            {
                return NotFound();
            }
            ViewData["Danhmucid"] = new SelectList(_context.DanhMucs, "Danhmucid", "Danhmucid", sanPham.Danhmucid);
            ViewData["Hangid"] = new SelectList(_context.HangSanXuats, "Hangid", "Hangid", sanPham.Hangid);
            ViewData["Nhucauid"] = new SelectList(_context.NhuCaus, "Nhucauid", "Nhucauid", sanPham.Nhucauid);
            return View(sanPham);
        }

        // POST: SanPhams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Sanphamid,Danhmucid,Hangid,Nhucauid,Tensanpham,Manhinh,Chip,Ram,Camera,Pin,Hedieuhanh,Tomtat,Mota,Hinhanh,Hotsale,Tragop0,Trogiathucu,Luotxem,Noibat,Kichhoat,Ngaytao")] SanPham sanPham)
        {
            if (id != sanPham.Sanphamid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sanPham);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SanPhamExists(sanPham.Sanphamid))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Danhmucid"] = new SelectList(_context.DanhMucs, "Danhmucid", "Danhmucid", sanPham.Danhmucid);
            ViewData["Hangid"] = new SelectList(_context.HangSanXuats, "Hangid", "Hangid", sanPham.Hangid);
            ViewData["Nhucauid"] = new SelectList(_context.NhuCaus, "Nhucauid", "Nhucauid", sanPham.Nhucauid);
            return View(sanPham);
        }

        // GET: SanPhams/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams
                .Include(s => s.Danhmuc)
                .Include(s => s.Hang)
                .Include(s => s.Nhucau)
                .FirstOrDefaultAsync(m => m.Sanphamid == id);
            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }

        // POST: SanPhams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham != null)
            {
                _context.SanPhams.Remove(sanPham);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SanPhamExists(int id)
        {
            return _context.SanPhams.Any(e => e.Sanphamid == id);
        }
    }
}
