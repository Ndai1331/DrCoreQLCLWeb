using CoreAdminWeb.Model.Base;

namespace CoreAdminWeb.Model
{
    public class QLCLGiaCaVatTuNNModel : BaseModel<int>
    {
        public DateTime? ngay_ghi_nhan { get; set; }
        public QLCLVatTuNongNghiepModel? vat_tu_nong_nghiep { get; set; }
        public TinhModel? province { get; set; }
        public XaPhuongModel? ward { get; set; }
        public DonViTinhModel? don_vi_tinh { get; set; }
        public string? nha_cung_cap { get; set; }
        public decimal? gia_mua_vao { get; set; }
        public decimal? gia_ban_ra { get; set; }
    }
    public class QLCLGiaCaVatTuNNCRUDModel : BaseDetailModel
    {
        public new string status { get; set; } = Status.active.ToString();
        public DateTime? ngay_ghi_nhan { get; set; }
        public int? vat_tu_nong_nghiep { get; set; }
        public int? province { get; set; }
        public int? ward { get; set; }
        public int? don_vi_tinh { get; set; }
        public string? nha_cung_cap { get; set; }
        public decimal? gia_mua_vao { get; set; }
        public decimal? gia_ban_ra { get; set; }
    }
}
