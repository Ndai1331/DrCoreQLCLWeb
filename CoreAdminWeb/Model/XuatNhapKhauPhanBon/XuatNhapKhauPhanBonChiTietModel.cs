using CoreAdminWeb.Model.Base;
using CoreAdminWeb.Model.PhanBon;

namespace CoreAdminWeb.Model.XuatNhapKhauPhanBon
{
    public class XuatNhapKhauPhanBonChiTietModel : BaseModel<int>
    {
        public XuatNhapKhauPhanBonModel? xnk_phan_bon { get; set; }
        public PhanBonModel? phan_bon { get; set; }
        public string? thanh_phan_ty_le { get; set; }
        public string? nuoc_xuat_nhap { get; set; }
        public string? dvt { get; set; }
        public int so_luong { get; set; } = 0;
    }
    public class XuatNhapKhauPhanBonChiTietCRUDModel : BaseDetailModel
    {
        public new string status { set; get; } = Status.active.ToString();

        public int? xnk_phan_bon { get; set; }
        public int? phan_bon { get; set; }
        public string? thanh_phan_ty_le { get; set; }
        public string? nuoc_xuat_nhap { get; set; }
        public string? dvt { get; set; }
        public int so_luong { get; set; } = 0;
    }
}
