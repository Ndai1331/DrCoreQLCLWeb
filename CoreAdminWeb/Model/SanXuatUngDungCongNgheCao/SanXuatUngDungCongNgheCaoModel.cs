using CoreAdminWeb.Model.Base;
using CoreAdminWeb.Model.CoSoTrongTrotSanXuat;

namespace CoreAdminWeb.Model.SanXuatUngDungCongNgheCao
{
    public class SanXuatUngDungCongNgheCaoModel : BaseModel<int>
    {
        public string? mo_hinh_du_an { get; set; }
        public CoSoTrongTrotSanXuatModel? co_so_trong_trot_san_xuat { get; set; }
        public TinhModel? province { get; set; }
        public XaPhuongModel? ward { get; set; }
        public string? dia_diem_trien_khai { get; set; }
        public DateTime? thoi_gian_bat_dau { get; set; }
        public DateTime? thoi_gian_ket_thuc { get; set; }
        public string? cong_nghe_canh_tac { get; set; }
        public string? cong_nghe_giong_cay_trong { get; set; }
        public string? muc_tieu { get; set; }

        public List<SanXuatUngDungCongNgheCaoLoaiCayTrongModel>? loai_cay_trong { get; set; }
    }
    public class SanXuatUngDungCongNgheCaoCRUDModel : BaseDetailModel
    {
        public new string status { get; set; } = Status.active.ToString();
        public string? mo_hinh_du_an { get; set; }
        public int? co_so_trong_trot_san_xuat { get; set; }
        public int? province { get; set; }
        public int? ward { get; set; }
        public string? dia_diem_trien_khai { get; set; }
        public DateTime? thoi_gian_bat_dau { get; set; }
        public DateTime? thoi_gian_ket_thuc { get; set; }
        public string? cong_nghe_canh_tac { get; set; }
        public string? cong_nghe_giong_cay_trong { get; set; }
        public string? muc_tieu { get; set; }
    }
}
