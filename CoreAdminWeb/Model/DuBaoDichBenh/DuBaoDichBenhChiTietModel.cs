using CoreAdminWeb.Enums;
using CoreAdminWeb.Model.Base;

namespace CoreAdminWeb.Model.DuBaoDichBenh
{
    public class DuBaoDichBenhChiTietModel : BaseModel<int>
    {
        public DuBaoDichBenhModel? du_bao_dich_benh { get; set; }
        public LoaiCayTrongModel? loai_cay_trong { get; set; }
        public KhaNangAnhHuong? kha_nang_anh_huong { get; set; } = KhaNangAnhHuong.MucNhe;
    }
    public class DuBaoDichBenhChiTietCRUDModel : BaseDetailModel
    {
        public new string status { set; get; } = Status.active.ToString();
        public int? du_bao_dich_benh { get; set; }
        public int? loai_cay_trong { get; set; }
        public string? kha_nang_anh_huong { get; set; } = KhaNangAnhHuong.MucNhe.ToString();
    }
}
