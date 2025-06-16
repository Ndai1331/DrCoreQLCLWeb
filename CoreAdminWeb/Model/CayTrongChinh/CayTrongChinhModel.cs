using CoreAdminWeb.Enums;
using CoreAdminWeb.Model.Base;
using CoreAdminWeb.Model;

namespace CoreAdminWeb.Model.CayTrongChinh
{
    public class CayTrongChinhModel : BaseModel<int>
    {
        public decimal? dien_tich { get; set; }
        public decimal? nang_suat_trung_binh { get; set; }
        public decimal? san_luong { get; set; }
        public string? khu_vuc_phan_bo { get; set; }
        public decimal? gia_tri_kinh_te { get; set; }
        public CayGiongCayTrongModel? cay_giong_cay_trong { get; set; }
    }

    public class CayTrongChinhCRUDModel : BaseDetailModel
    {
        public new string status { get; set; } = Status.active.ToString();
        public decimal? dien_tich { get; set; }
        public decimal? nang_suat_trung_binh { get; set; }
        public decimal? san_luong { get; set; }
        public string? khu_vuc_phan_bo { get; set; }
        public decimal? gia_tri_kinh_te { get; set; }
        public int? cay_giong_cay_trong { get; set; }
    }
}
