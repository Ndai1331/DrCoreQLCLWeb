using CoreAdminWeb.Model.Base;

namespace CoreAdminWeb.Model.CoSoTrongTrotSanXuat
{
    public class CoSoTrongTrotSanXuatChiTietCayTrongModel : BaseModel<int>
    {
        public CoSoTrongTrotSanXuatModel? co_so_trong_trot_san_xuat { get; set; }
        public CayGiongCayTrongModel? cay_giong_cay_trong { get; set; }
        public decimal? dien_tich { get; set; }
        public decimal? nang_suat_binh_quan { get; set; }
        public decimal? san_luong_binh_quan { get; set; }
    }
    public class CoSoTrongTrotSanXuatChiTietCayTrongCRUDModel : BaseDetailModel
    {
        public new string status { get; set; } = Status.active.ToString();
        public int? co_so_trong_trot_san_xuat { get; set; }
        public int? cay_giong_cay_trong { get; set; }
        public decimal? dien_tich { get; set; }
        public decimal? nang_suat_binh_quan { get; set; }
        public decimal? san_luong_binh_quan { get; set; }
    }
}
