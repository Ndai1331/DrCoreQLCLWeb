using CoreAdminWeb.Model.Base;

namespace CoreAdminWeb.Model.DienTichGieoTrongCayHangNam
{
    public class DienTichGieoTrongCayHangNamCoCauChuLucModel : BaseModel<int>
    {
        public DienTichGieoTrongCayHangNamModel? dien_tich_gieo_trong_cay_hang_nam { get; set; }
        public CayGiongCayTrongModel? cay_giong_cay_trong { get; set; }
    }
    public class DienTichGieoTrongCayHangNamCoCauChuLucCRUDModel : BaseDetailModel
    {
        public new string status { get; set; } = Status.active.ToString();
        public int? dien_tich_gieo_trong_cay_hang_nam { get; set; }
        public int? cay_giong_cay_trong { get; set; }
    }
}
