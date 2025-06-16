using CoreAdminWeb.Enums;
using CoreAdminWeb.Model.Base;
using CoreAdminWeb.Model.CoSoBuonBan;
using CoreAdminWeb.Model.QuanLyCoSoSanXuatPhanBon;

namespace CoreAdminWeb.Model.XuatNhapKhauPhanBon
{
    public class XuatNhapKhauPhanBonModel : BaseModel<int>
    {
        public DateTime ngay_chung_tu { get; set; }

        public QuanLyCoSoSanXuatPhanBonModel? co_so_san_xuat_phan_bon { get; set; }
        public CoSoDuDieuKienBuonBanPhanBonModel? co_so_du_dieu_kien_buon_ban_phan_bon { get; set; }
        public string? hinh_thuc { get; set; } = HinhThucXuatNhapKhau.NhapKhau.ToString();
        public string? giay_phep_xnk { get; set; }
        public DateTime? ngay_cap { get; set; }
        public string? co_quan_cap { get; set; }
        public string? so_chung_tu { get; set; }
    }
    public class XuatNhapKhauPhanBonCRUDModel : BaseDetailModel
    {
        public new string status { set; get; } = Status.active.ToString();
        public DateTime ngay_chung_tu { get; set; }

        public int? co_so_san_xuat_phan_bon { get; set; }
        public int? co_so_du_dieu_kien_buon_ban_phan_bon { get; set; }
        public string? hinh_thuc { get; set; } = HinhThucXuatNhapKhau.NhapKhau.ToString();
        public string? giay_phep_xnk { get; set; }
        public DateTime? ngay_cap { get; set; }
        public string? co_quan_cap { get; set; }
        public string? so_chung_tu { get; set; }
    }
}
