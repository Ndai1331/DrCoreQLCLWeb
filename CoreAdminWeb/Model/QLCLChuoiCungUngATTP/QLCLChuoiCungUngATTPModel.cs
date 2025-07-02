using CoreAdminWeb.Model.Base;

namespace CoreAdminWeb.Model
{
    public class QLCLChuoiCungUngATTPModel : BaseModel<int>
    {
        public TinhModel? province_san_xuat { get; set; }
        public XaPhuongModel? ward_san_xuat { get; set; }
        public string? dia_chi_san_xuat { get; set; }
        public TinhModel? province_kinh_doanh { get; set; }
        public XaPhuongModel? ward_kinh_doanh { get; set; }
        public string? dia_chi_kinh_doanh { get; set; }
        public string? so_xac_nhan { get; set; }
        public DateTime? ngay_chung_nhan { get; set; }
        public string? co_quan_xac_nhan { get; set; }
        public string? san_pham { get; set; }
        public List<QLCLChuoiCungUngATTPCoSoModel>? chi_tiets { get; set; }
    }
    public class QLCLChuoiCungUngATTPCRUDModel : BaseDetailModel
    {
        public new string status { get; set; } = Status.active.ToString();
        public int? province_san_xuat { get; set; }
        public int? ward_san_xuat { get; set; }
        public string? dia_chi_san_xuat { get; set; }
        public int? province_kinh_doanh { get; set; }
        public int? ward_kinh_doanh { get; set; }
        public string? dia_chi_kinh_doanh { get; set; }
        public string? so_xac_nhan { get; set; }
        public DateTime? ngay_chung_nhan { get; set; }
        public string? co_quan_xac_nhan { get; set; }
        public string? san_pham { get; set; }
    }


    public class QLCLChuoiCungUngATTPCRUDResponseModel
    {
        public int id { get; set; }
    }
}
