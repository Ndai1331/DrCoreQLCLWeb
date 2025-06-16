using CoreAdminWeb.Model.Base;
using CoreAdminWeb.Model.ThuocBaoVeThucVat;
using CoreAdminWeb.Model.XuatNhapKhauThuocBaoVeThucVat;

namespace CoreAdminWeb.Model.XuatNhapKhauPhanBon
{
    public class XuatNhapKhauThuocBVTVChiTietModel : BaseModel<int>
    {
        public XuatNhapKhauThuocBVTVModel? xnk_thuoc_bvtv { get; set; }
        public ThuocBaoVeThucVatModel? thuoc_bvtv { get; set; }
        public string? thanh_phan_ty_le { get; set; }
        public string? nuoc_xuat_nhap { get; set; }
        public string? dvt { get; set; }
        public int so_luong { get; set; } = 0;
    }
    public class XuatNhapKhauThuocBVTVChiTietCRUDModel : BaseDetailModel
    {
        public new string status { set; get; } = Status.active.ToString();

        public int? xnk_thuoc_bvtv { get; set; }
        public int? thuoc_bvtv { get; set; }
        public string? thanh_phan_ty_le { get; set; }
        public string? nuoc_xuat_nhap { get; set; }
        public string? dvt { get; set; }
        public int so_luong { get; set; } = 0;
    }
}
