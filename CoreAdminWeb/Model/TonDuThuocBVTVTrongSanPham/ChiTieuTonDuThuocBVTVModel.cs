using CoreAdminWeb.Enums;
using CoreAdminWeb.Model.Base;

namespace CoreAdminWeb.Model.TonDuThuocBVTVTrongSanPham;

public class ChiTieuTonDuThuocBVTVModel : BaseModel<int>
{
    public new TrangThaiBanGhiChiTiet status { get; set; } = TrangThaiBanGhiChiTiet.New;
    public TonDuThuocBVTVTrongSanPhamModel? ton_du_thuoc_bvtv_trong_san_pham { get; set; }
    public string? chi_tieu_ton_du { get; set; }
    public string? ham_luong_ket_qua { get; set; }
    public string? gioi_han_cho_phep { get; set; }
}

public class ChiTieuTonDuThuocBVTVCRUDModel : BaseDetailModel
{
    public new string status { set; get; } = TrangThaiBanGhiChiTiet.New.ToString();
    public int? ton_du_thuoc_bvtv_trong_san_pham { get; set; }
    public string? chi_tieu_ton_du { get; set; }
    public string? ham_luong_ket_qua { get; set; }
    public string? gioi_han_cho_phep { get; set; }
}
