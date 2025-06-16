using CoreAdminWeb.Model.Base;
using CoreAdminWeb.Enums;

namespace CoreAdminWeb.Model
{
    public class TieuChuanQuyChuanModel : BaseModel<int>
    {
        public TieuChuanQuyChuan? phan_loai { get; set; } = TieuChuanQuyChuan.TieuChuan;
        public DateTime? ngay_ban_hanh { get; set; }
        public DateTime? ngay_hieu_luc{ get; set; }
        public DateTime? ngay_het_han { get; set; }
        public string? co_quan_cap { get; set; }
        public string? pham_vi_ap_dung { get; set; }
        public string? noi_dung_ky_thuat_chinh { get; set; }
        public string? huong_dan_ap_dung { get; set; }
    }
    public class TieuChuanQuyChuanCRUDModel : BaseDetailModel
    {
        public new string status { get; set; } = Status.active.ToString();
        public TieuChuanQuyChuan? phan_loai { get; set; } = TieuChuanQuyChuan.TieuChuan;
        public DateTime? ngay_ban_hanh { get; set; }
        public DateTime? ngay_hieu_luc{ get; set; }
        public DateTime? ngay_het_han { get; set; }
        public string? co_quan_cap { get; set; }
        public string? pham_vi_ap_dung { get; set; }
        public string? noi_dung_ky_thuat_chinh { get; set; }
        public string? huong_dan_ap_dung { get; set; }
    }
}
