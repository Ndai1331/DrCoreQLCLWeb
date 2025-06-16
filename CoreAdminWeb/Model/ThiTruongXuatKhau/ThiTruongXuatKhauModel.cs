using CoreAdminWeb.Model.Base;
using CoreAdminWeb.Model.User;

namespace CoreAdminWeb.Model
{
    public class ThiTruongXuatKhauModel : BaseModel<int>
    {
        public CountryModel? country { get; set; }
        public DateTime? ngay_ghi_nhan { get; set; }
        public string? khu_vuc_thi_truong { get; set; }
        public string? doanh_nghiep_xuat_khau { get; set; }
        public string? ma_don_vi { get; set; }
        public string? so_giay_phep_xuat_khau { get; set; }
        public string? dia_chi { get; set; }
    }
    public class ThiTruongXuatKhauCRUDModel : BaseDetailModel
    {
        public new string status { get; set; } = Status.active.ToString();
        public int? country { get; set; }
        public DateTime? ngay_ghi_nhan { get; set; }
        public string? khu_vuc_thi_truong { get; set; }
        public string? doanh_nghiep_xuat_khau { get; set; }
        public string? ma_don_vi { get; set; }
        public string? so_giay_phep_xuat_khau { get; set; }
        public string? dia_chi { get; set; }
    }


    public class ThiTruongXuatKhauChiTietModel 
    {
        public int id { get; set; }
        public UserModel? user_created { get; set; } = new();
        public DateTime date_created { get; set; } = DateTime.Now;
        public UserModel? user_updated { get; set; } = new();
        public DateTime? date_updated { get; set; } = DateTime.Now;
        public ThiTruongXuatKhauModel? thi_truong_xuat_khau { get; set; }
        public SanPhamSanXuatModel? san_pham { get; set; }
        public decimal? san_luong_tan { get; set; }
        public decimal? gia_tri { get; set; }
    }
    public class ThiTruongXuatKhauChiTietCRUDModel 
    {
        public int? thi_truong_xuat_khau { get; set; }
        public int? san_pham { get; set; }
        public decimal? san_luong_tan { get; set; }
        public decimal? gia_tri { get; set; }
        public bool? deleted { get; set; }
    }
}
