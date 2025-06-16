using CoreAdminWeb.Enums;
using CoreAdminWeb.Model.Base;
using CoreAdminWeb.Model;

namespace CoreAdminWeb.Model.CayTrongDuocPhepGieoTrongKhuVuc
{
    public class CayTrongDuocPhepGieoTrongKhuVucModel : BaseModel<int>
    {
        public string? dieu_kien_khi_hau { get; set; }
        public string? dieu_kien_dat_dai { get; set; }
        public string? thoi_vu_gieo_trong_khuyen_nghi { get; set; }
        public string? hieu_qua_kinh_te { get; set; }
        public TinhModel? province { get; set; }
        public XaPhuongModel? ward { get; set; }
        public CayGiongCayTrongModel? cay_giong_cay_trong { get; set; }
    }

    public class CayTrongDuocPhepGieoTrongKhuVucCRUDModel : BaseDetailModel
    {
        public new string status { get; set; } = Status.active.ToString();
        public string? dieu_kien_khi_hau { get; set; }
        public string? dieu_kien_dat_dai { get; set; }
        public string? thoi_vu_gieo_trong_khuyen_nghi { get; set; }
        public string? hieu_qua_kinh_te { get; set; }
        public int? province { get; set; }
        public int? ward { get; set; }
        public int? cay_giong_cay_trong { get; set; }
    }
}
