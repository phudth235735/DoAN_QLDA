namespace PhoneStore.ViewModels;

public class GioHangItem
{
    public int Bientheid { get; set; }
    public int Sanphamid { get; set; }
    public string TenSanPham { get; set; } = string.Empty;
    public string? HinhAnh { get; set; }
    public string DungLuong { get; set; } = string.Empty;
    public string MauSac { get; set; } = string.Empty;
    public decimal DonGia { get; set; }
    public int SoLuong { get; set; }
    public int TonKho { get; set; }
    public decimal ThanhTien => DonGia * SoLuong;
}
