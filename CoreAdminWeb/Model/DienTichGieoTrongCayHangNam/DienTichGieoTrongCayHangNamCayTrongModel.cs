using CoreAdminWeb.Enums;
using CoreAdminWeb.Model.Base;

namespace CoreAdminWeb.Model.DienTichGieoTrongCayHangNam
{
    public class DienTichGieoTrongCayHangNamCayTrongModel : BaseModel<int>
    {
        public DienTichGieoTrongCayHangNamModel? dien_tich_gieo_trong_cay_hang_nam { get; set; }
        public CayGiongCayTrongModel? cay_giong_cay_trong { get; set; }
        public string? mua_vu { get; set; }
        public decimal? dien_tich_ke_hoach { get; set; }
        public decimal? dien_tich_gieo_trong { get; set; }
        public decimal? nang_suat { get; set; }
        public decimal? san_luong { get; set; }
    }
    public class DienTichGieoTrongCayHangNamCayTrongCRUDModel : BaseDetailModel
    {
        public new string status { get; set; } = Status.active.ToString();
        public int? dien_tich_gieo_trong_cay_hang_nam { get; set; }
        public int? cay_giong_cay_trong { get; set; }
        public string? mua_vu { get; set; }
        public decimal? dien_tich_ke_hoach { get; set; }
        public decimal? dien_tich_gieo_trong { get; set; }
        public decimal? nang_suat { get; set; }
        public decimal? san_luong { get; set; }
    }
}
