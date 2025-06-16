using CoreAdminWeb.Model.Base;

namespace CoreAdminWeb.Model.DienTichGieoTrongCayLauNam
{
    public class DienTichGieoTrongCayLauNamCayTrongModel : BaseModel<int>
    {
        public DienTichGieoTrongCayLauNamModel? dien_tich_gieo_trong_cay_lau_nam { get; set; }
        public CayGiongCayTrongModel? cay_giong_cay_trong { get; set; }
        public decimal? dien_tich_ke_hoach { get; set; }
        public decimal? dien_tich_gieo_trong { get; set; }
        public decimal? nang_suat { get; set; }
        public decimal? san_luong { get; set; }
    }
    public class DienTichGieoTrongCayLauNamCayTrongCRUDModel : BaseDetailModel
    {
        public new string status { get; set; } = Status.active.ToString();
        public int? dien_tich_gieo_trong_cay_lau_nam { get; set; }
        public int? cay_giong_cay_trong { get; set; }
        public decimal? dien_tich_ke_hoach { get; set; }
        public decimal? dien_tich_gieo_trong { get; set; }
        public decimal? nang_suat { get; set; }
        public decimal? san_luong { get; set; }
    }
}
