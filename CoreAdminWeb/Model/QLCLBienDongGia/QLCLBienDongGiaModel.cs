using CoreAdminWeb.Enums;
using CoreAdminWeb.Model.Base;

namespace CoreAdminWeb.Model
{
    public class QLCLBienDongGiaModel
    {
        public DateTime? ngay_ghi_nhan { get; set; }
        public LoaiBienDongGia? loai { get; set; } //  1: nông sản 2: vật tư nông nghiệp,
        public string? ten_san_pham { get; set; }
        public string? nha_cung_cap { get; set; }
        public string? dia_diem { get; set; }
        public string? don_vi_tinh { get; set; }
        public double? gia_mua_vao { get; set; }
        public double? gia_ban_ra { get; set; }
        public double? bien_dong { get; set; }
    }
}
