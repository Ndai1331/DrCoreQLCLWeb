using CoreAdminWeb.Model.Base;

namespace CoreAdminWeb.Model
{
    public class CoSoSanXuatGiongChiTietModel : BaseModel<int>
    {
        public CoSoSanXuatGiongModel? co_so_san_xuat_giong { get; set; }
        public CayGiongCayTrongModel? cay_giong_cay_trong { get; set; }
        public decimal? san_luong_du_kien { get; set; } = 0;

    }
    public class CoSoSanXuatGiongChiTietCRUDModel : BaseDetailModel
    {
        public new string status { get; set; } = Status.active.ToString();
        public int? co_so_san_xuat_giong { get; set; }
        public int? cay_giong_cay_trong { get; set; }
        public decimal? san_luong_du_kien { get; set; } = 0;
    }
}
