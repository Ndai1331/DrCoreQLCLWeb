using CoreAdminWeb.Enums;
using CoreAdminWeb.Model.Base;
using CoreAdminWeb.Model.CoSoTrongTrotSanXuat;

namespace CoreAdminWeb.Model.CoSoBiDichBenh
{
    public class CoSoBiDichBenhModel : BaseModel<int>
    {
        public TinhModel? province { get; set; }
        public XaPhuongModel? ward { get; set; }
        public CoSoTrongTrotSanXuatModel? co_so_trong_trot_san_xuat { get; set; }
        public string? dia_diem_san_xuat { get; set; }
        public DateTime? thoi_diem_ghi_nhan { get; set; }
        public DateTime? thoi_gian_bi_benh_tu { get; set; }
        public DateTime? thoi_gian_bi_benh_den { get; set; }
        public MuaVu? mua_vu { get; set; }
        public string? nguyen_nhan { get; set; }
        public string? bien_phap_phong_tru { get; set; }
        public string? muc_do_thiet_hai { get; set; }
        public string? ho_tro_tu_co_quan_quan_ly { get; set; }
        public CayGiongCayTrongModel? cay_trong { get; set; }
        public List<CoSoBiDichBenhChiTietModel>? chi_tiet_dich_benh { get; set; }
    }
    public class CoSoBiDichBenhCRUDModel : BaseDetailModel
    {
        public new string status { get; set; } = Status.active.ToString();
        public int? province { get; set; }
        public int? ward { get; set; }
        public int? co_so_trong_trot_san_xuat { get; set; }
        public string? dia_diem_san_xuat { get; set; }
        public DateTime? thoi_diem_ghi_nhan { get; set; } = DateTime.Now;
        public DateTime? thoi_gian_bi_benh_tu { get; set; }
        public DateTime? thoi_gian_bi_benh_den { get; set; }
        public MuaVu? mua_vu { get; set; }
        public string? nguyen_nhan { get; set; }
        public string? bien_phap_phong_tru { get; set; }
        public string? muc_do_thiet_hai { get; set; }
        public string? ho_tro_tu_co_quan_quan_ly { get; set; }
        public int? cay_trong { get; set; }
    }
}
