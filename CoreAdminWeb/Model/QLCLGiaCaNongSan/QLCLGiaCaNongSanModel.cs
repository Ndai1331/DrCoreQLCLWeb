using CoreAdminWeb.Model.Base;

namespace CoreAdminWeb.Model
{
    public class QLCLGiaCaNongSanModel : BaseModel<int>
    {
        public DateTime? ngay_ghi_nhan { get; set; }
        public QLCLSanPhamSanXuatModel? san_pham_san_xuat { get; set; }
        public TinhModel? province { get; set; }
        public XaPhuongModel? ward { get; set; }
        public DonViTinhModel? don_vi_tinh { get; set; }
        public string? nha_cung_cap { get; set; }
        public decimal? gia_mua_vao { get; set; }
        public decimal? gia_ban_ra { get; set; }
    }
    public class QLCLGiaCaNongSanCRUDModel : BaseDetailModel
    {
        public new string status { get; set; } = Status.active.ToString();
        public DateTime? ngay_ghi_nhan { get; set; }
        public int? san_pham_san_xuat { get; set; }
        public int? province { get; set; }
        public int? ward { get; set; }
        public int? don_vi_tinh { get; set; }
        public string? nha_cung_cap { get; set; }
        public decimal? gia_mua_vao { get; set; }
        public decimal? gia_ban_ra { get; set; }
    }
}
