using CoreAdminWeb.Model.Base;

namespace CoreAdminWeb.Model.SanXuatUngDungCongNgheCao
{
    public class SanXuatUngDungCongNgheCaoLoaiCayTrongModel : BaseModel<int>
    {
        public SanXuatUngDungCongNgheCaoModel? san_xuat_ung_dung_cong_nghe_cao { get; set; }
        public LoaiCayTrongModel? loai_cay_trong { get; set; }
        public decimal? dien_tich { get; set; }
        public decimal? san_luong { get; set; }
    }
    public class SanXuatUngDungCongNgheCaoLoaiCayTrongCRUDModel : BaseDetailModel
    {
        public new string status { get; set; } = Status.active.ToString();
        public int? san_xuat_ung_dung_cong_nghe_cao { get; set; }
        public int? loai_cay_trong { get; set; }
        public decimal? dien_tich { get; set; }
        public decimal? san_luong { get; set; }
    }
}
