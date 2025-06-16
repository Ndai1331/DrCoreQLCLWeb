using CoreAdminWeb.Enums;
using CoreAdminWeb.Model.Base;
using CoreAdminWeb.Model;

namespace CoreAdminWeb.Model.ToChucCaNhanDaThongBaoDDKDGiong
{
    public class ToChucCaNhanDaThongBaoDDKDGiongModel : BaseModel<int>
    {
        public string? so_giay_phep_kinh_doanh { get; set; }
        public CoSoSanXuatGiongModel? co_so_san_xuat_giong { get; set; }
    }

    public class ToChucCaNhanDaThongBaoDDKDGiongCRUDModel : BaseDetailModel
    {
        public new string status { get; set; } = Status.active.ToString();
        public string? so_giay_phep_kinh_doanh { get; set; }
        public int? co_so_san_xuat_giong { get; set; }
    }
}
