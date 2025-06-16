using CoreAdminWeb.Enums;
using CoreAdminWeb.Model.Base;

namespace CoreAdminWeb.Model.TonDuPhanBonTrongSanPham
{
    public class ChiTieuTonDuPhanBonModel : BaseModel<int>
    {
        public new TrangThaiBanGhiChiTiet status { get; set; } = TrangThaiBanGhiChiTiet.New;
        public TonDuPhanBonTrongSanPhamModel? ton_du_phan_bon_trong_san_pham { get; set; }
        public string? chi_tieu_ton_du { get; set; }
        public string? ham_luong_ket_qua { get; set; }
        public string? gioi_han_cho_phep { get; set; }
    }

    public class ChiTieuTonDuPhanBonCRUDModel : BaseDetailModel
    {
        public new string status { set; get; } = TrangThaiBanGhiChiTiet.New.ToString();
        public int? ton_du_phan_bon_trong_san_pham { get; set; }
        public string? chi_tieu_ton_du { get; set; }
        public string? ham_luong_ket_qua { get; set; }
        public string? gioi_han_cho_phep { get; set; }
    }
}
