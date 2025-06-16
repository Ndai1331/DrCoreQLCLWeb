using CoreAdminWeb.Enums;
using CoreAdminWeb.Model.Base;

namespace CoreAdminWeb.Model.TonDuThuocBVTVTrongSanPham
{
    public class TonDuThuocBVTVTrongSanPhamModel : BaseModel<int>
    {
        public new TrangThaiBanGhi status { get; set; } = TrangThaiBanGhi.ChoLuu;
        public string? ten_mau_kiem_dinh { get; set; }
        public int? so_luong_mau { get; set; }
        public DateTime? ngay_lay_mau { get; set; }
        public string? dia_diem_lay_mau { get; set; }
        public TinhModel? province { get; set; }
        public XaPhuongModel? ward { get; set; }
        public string? phuong_phap_lay_mau { get; set; }
        public string? ma_co_so { get; set; }
        public string? ten_co_so { get; set; }
        public string? don_vi_kiem_dinh { get; set; }
        public string? ket_qua_phan_tich { get; set; }
        public string? bien_phap_xu_ly { get; set; }
    }

    public class TonDuThuocBVTVTrongSanPhamCRUDModel : BaseDetailModel
    {
        public new string status { set; get; } = TrangThaiBanGhi.ChoLuu.ToString();
        public string? ten_mau_kiem_dinh { get; set; }
        public int? so_luong_mau { get; set; }
        public DateTime? ngay_lay_mau { get; set; }
        public string? dia_diem_lay_mau { get; set; }
        public int? province { get; set; }
        public int? ward { get; set; }
        public string? phuong_phap_lay_mau { get; set; }
        public string? ma_co_so { get; set; }
        public string? ten_co_so { get; set; }
        public string? don_vi_kiem_dinh { get; set; }
        public string? ket_qua_phan_tich { get; set; }
        public string? bien_phap_xu_ly { get; set; }
    }
}