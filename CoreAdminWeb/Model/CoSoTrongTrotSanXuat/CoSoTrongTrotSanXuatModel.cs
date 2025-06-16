using CoreAdminWeb.Model.Base;

namespace CoreAdminWeb.Model.CoSoTrongTrotSanXuat
{
    public class CoSoTrongTrotSanXuatModel : BaseModel<int>
    {
        public TinhModel? province { get; set; }
        public XaPhuongModel? ward { get; set; }
        public string? dia_chi { get; set; }
        public string? dien_thoai { get; set; }
        public string? email { get; set; }
        public string? so_cccd { get; set; }
        public string? nguoi_dai_dien { get; set; }
        public string? so_giay_phep_kinh_doanh { get; set; }
        public string? so_gcn_du_dieu_kien { get; set; }
        public DateTime? ngay_cap { get; set; }
        public string? co_quan_cap_phep { get; set; }
        public decimal? dien_tich_san_xuat { get; set; }
        public decimal? nang_suat_du_kien { get; set; }
        public string? cong_nghe_canh_tac { get; set; }
    }
    public class CoSoTrongTrotSanXuatCRUDModel : BaseDetailModel
    {
        public new string status { get; set; } = Status.active.ToString();
        public int? province { get; set; }
        public int? ward { get; set; }
        public string? dia_chi { get; set; }
        public string? dien_thoai { get; set; }
        public string? email { get; set; }
        public string? so_cccd { get; set; }
        public string? nguoi_dai_dien { get; set; }
        public string? so_giay_phep_kinh_doanh { get; set; }
        public string? so_gcn_du_dieu_kien { get; set; }
        public DateTime? ngay_cap { get; set; }
        public string? co_quan_cap_phep { get; set; }
        public decimal? dien_tich_san_xuat { get; set; }
        public decimal? nang_suat_du_kien { get; set; }
        public string? cong_nghe_canh_tac { get; set; }
    }
}
