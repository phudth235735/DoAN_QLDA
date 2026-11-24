using System.ComponentModel.DataAnnotations;

namespace PhoneStore.ViewModels;

public class ThanhToanViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập họ tên người nhận")]
    [StringLength(100)]
    public string TenNguoiNhan { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    [StringLength(20)]
    public string SdtNguoiNhan { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng")]
    [StringLength(255)]
    public string DiaChiGiaoHang { get; set; } = string.Empty;

    [Required]
    public string PhuongThucThanhToan { get; set; } = "COD";

    public string? GhiChu { get; set; }
}
