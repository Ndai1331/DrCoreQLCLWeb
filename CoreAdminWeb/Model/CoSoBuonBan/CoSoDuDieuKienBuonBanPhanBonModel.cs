using CoreAdminWeb.Model.Base;

namespace CoreAdminWeb.Model.CoSoBuonBan
{
    public class CoSoDuDieuKienBuonBanPhanBonModel : BaseModel<int>
    {
        public string? dia_chi { get; set; }
        public string? dien_thoai { get; set; }
        public string? email { get; set; }
        public string? so_cccd { get; set; }
        public string? nguoi_dai_dien { get; set; }
        public string? so_giay_phep_kinh_doanh { get; set; }
        public DateTime? ngay_cap { get; set; }
        public string? co_quan_cap_phep { get; set; }
        public int? nhom_co_so { get; set; }

        public TinhModel? province { get; set; }
        public XaPhuongModel? ward { get; set; }
        public LoaiHinhKinhDoanhModel? loai_hinh_kinh_doanh { get; set; }
    }

    public class CoSoDuDieuKienBuonBanPhanBonCRUDModel : BaseDetailModel
    {
        public new string status { set; get; } = Status.active.ToString();
        public string? dia_chi { get; set; }
        public string? dien_thoai { get; set; }
        public string? email { get; set; }
        public string? so_cccd { get; set; }
        public string? nguoi_dai_dien { get; set; }
        public string? so_giay_phep_kinh_doanh { get; set; }
        public DateTime? ngay_cap { get; set; }
        public string? co_quan_cap_phep { get; set; }
        public int? nhom_co_so { get; set; }

        public int province { get; set; }
        public int ward { get; set; }
        public int? loai_hinh_kinh_doanh { get; set; }
    }
}
