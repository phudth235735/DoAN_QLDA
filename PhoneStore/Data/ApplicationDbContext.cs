using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PhoneStore.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace PhoneStore.Data;

public partial class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BienTheSanpham> BienTheSanphams { get; set; }

    public virtual DbSet<BinhLuan> BinhLuans { get; set; }

    public virtual DbSet<ChiNhanh> ChiNhanhs { get; set; }

    public virtual DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }

    public virtual DbSet<DanhMuc> DanhMucs { get; set; }

    public virtual DbSet<DonHang> DonHangs { get; set; }

    public virtual DbSet<HangSanXuat> HangSanXuats { get; set; }

    public virtual DbSet<KhuyenMaiSanPham> KhuyenMaiSanPhams { get; set; }

    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }

    public virtual DbSet<NhuCau> NhuCaus { get; set; }

    public virtual DbSet<SanPham> SanPhams { get; set; }

    public virtual DbSet<TinCongNghe> TinCongNghes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<BienTheSanpham>(entity =>
        {
            entity.HasKey(e => e.Bientheid).HasName("PK__BienTheS__C2F483718EA38D20");

            entity.Property(e => e.Kichhoat).HasDefaultValue(true);

            entity.HasOne(d => d.Sanpham).WithMany(p => p.BienTheSanphams).HasConstraintName("FK_bienthe_sanpham");
        });

        modelBuilder.Entity<BinhLuan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BinhLuan__3213E83F33BD98D4");

            entity.Property(e => e.Kichhoat).HasDefaultValue(true);
            entity.Property(e => e.Sosao).HasDefaultValue(5);

            entity.HasOne(d => d.Sanpham).WithMany(p => p.BinhLuans).HasConstraintName("FK_binhluan_sanpham");
        });

        modelBuilder.Entity<ChiNhanh>(entity =>
        {
            entity.HasKey(e => e.Chinhanhid).HasName("PK__ChiNhanh__8C810A3013F0549A");
        });

        modelBuilder.Entity<ChiTietDonHang>(entity =>
        {
            entity.HasKey(e => e.Chitietid).HasName("PK__ChiTietD__B6A8B592E72C4E25");

            entity.HasOne(d => d.Bienthe).WithMany(p => p.ChiTietDonHangs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_chitiet_bienthe");

            entity.HasOne(d => d.Donhang).WithMany(p => p.ChiTietDonHangs).HasConstraintName("FK_chitiet_donhang");
        });

        modelBuilder.Entity<DanhMuc>(entity =>
        {
            entity.HasKey(e => e.Danhmucid).HasName("PK__DanhMuc__BBADBA2503E6470C");
        });

        modelBuilder.Entity<DonHang>(entity =>
        {
            entity.HasKey(e => e.Donhangid).HasName("PK__DonHang__7E8E0BD3B6DB7782");

            entity.Property(e => e.Ngaydat).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Chinhanh).WithMany(p => p.DonHangs)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_donhang_chinhanh");

            entity.HasOne(d => d.Nguoidung).WithMany(p => p.DonHangs)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_donhang_nguoidung");
        });

        modelBuilder.Entity<HangSanXuat>(entity =>
        {
            entity.HasKey(e => e.Hangid).HasName("PK__HangSanX__77BAD8694B4159A1");
        });

        modelBuilder.Entity<KhuyenMaiSanPham>(entity =>
        {
            entity.HasKey(e => e.Kmid).HasName("PK__KhuyenMa__252A40CC02711271");

            entity.HasOne(d => d.Sanpham).WithMany(p => p.KhuyenMaiSanPhams).HasConstraintName("FK_khuyenmai_sanpham");
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.Nguoidungid).HasName("PK__NguoiDun__22456E556A8744FD");

            entity.Property(e => e.Hangthanhvien).HasDefaultValue("Smember");
            entity.Property(e => e.Kichhoat).HasDefaultValue(true);
        });

        modelBuilder.Entity<NhuCau>(entity =>
        {
            entity.HasKey(e => e.Nhucauid).HasName("PK__NhuCau__F73498F9850D304E");
        });

        modelBuilder.Entity<SanPham>(entity =>
        {
            entity.HasKey(e => e.Sanphamid).HasName("PK__SanPham__8EB3A43A154FEEF8");

            entity.Property(e => e.Kichhoat).HasDefaultValue(true);
            entity.Property(e => e.Ngaytao).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Tragop0).HasDefaultValue(true);

            entity.HasOne(d => d.Danhmuc).WithMany(p => p.SanPhams)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_sanpham_danhmuc");

            entity.HasOne(d => d.Hang).WithMany(p => p.SanPhams)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_sanpham_hangsanxuat");

            entity.HasOne(d => d.Nhucau).WithMany(p => p.SanPhams).HasConstraintName("FK_sanpham_nhucau");
        });

        modelBuilder.Entity<TinCongNghe>(entity =>
        {
            entity.HasKey(e => e.Tinid).HasName("PK__TinCongN__E4A274C7D63C62CA");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    
}
