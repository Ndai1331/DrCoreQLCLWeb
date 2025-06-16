using CoreAdminWeb.Enums;
using CoreAdminWeb.Model.Base;

namespace CoreAdminWeb.Model.DuBaoDichBenh
{
    public class DuBaoDichBenhModel : BaseModel<int>
    {
        public DateTime? ngay_du_bao { get; set; }
        public DateTime? tu_ngay { get; set; }
        public DateTime? den_ngay { get; set; }
        public ViSinhVatGayHaiModel? vi_sinh_vat_gay_hai { get; set; }
        public string? du_lieu_khi_hau { get; set; }
        public string? tinh_trang_sinh_truong { get; set; }
        public string? bien_dong_quan_the { get; set; }
        public string? du_bao_ngan_han { get; set; }
        public string? du_bao_dai_han { get; set; }
        public string? bien_phap_phong_tru { get; set; }
        public MucDoNguyCo? muc_do_nguy_co { get; set; } = MucDoNguyCo.Thap;
        public List<DuBaoDichBenhChiTietModel>? chi_tiet { get; set; }
    }
    public class DuBaoDichBenhCRUDModel : BaseDetailModel
    {
        public new string status { set; get; } = Status.active.ToString();
        public DateTime? ngay_du_bao { get; set; }
        public DateTime? tu_ngay { get; set; }
        public DateTime? den_ngay { get; set; }
        public int? vi_sinh_vat_gay_hai { get; set; }
        public string? du_lieu_khi_hau { get; set; }
        public string? tinh_trang_sinh_truong { get; set; }
        public string? bien_dong_quan_the { get; set; }
        public string? du_bao_ngan_han { get; set; }
        public string? du_bao_dai_han { get; set; }
        public string? bien_phap_phong_tru { get; set; }
        public string? muc_do_nguy_co { get; set; } = MucDoNguyCo.Thap.ToString();
    }
}
