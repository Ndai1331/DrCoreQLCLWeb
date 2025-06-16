using CoreAdminWeb.Enums;
using CoreAdminWeb.Model.Base;
using CoreAdminWeb.Model.LoaiPhanBon;
using CoreAdminWeb.Model.QuanLyCoSoSanXuatPhanBon;

namespace CoreAdminWeb.Model.PhanBon
{
    public class PhanBonModel : BaseModel<int>
    {
        public string? thanh_phan_ham_luong { get; set; }
        public string? nguon_goc { get; set; }
        public string? ghi_chu { get; set; }
        public LoaiPhanBonModel? loai_phan_bon { get; set; }
        public QuanLyCoSoSanXuatPhanBonModel? co_so_san_xuat_phan_bon { get; set; }
        public DonViTinhModel? don_vi_tinh { get; set; }
    }

    public class PhanBonCRUDModel : BaseDetailModel
    {
        public new string status { set; get; } = Status.active.ToString();
        public string? thanh_phan_ham_luong { get; set; }
        public string? nguon_goc { get; set; } = NguonGocPhanBon.NhapKhau.ToString();
        public string? ghi_chu { get; set; }
        public int? loai_phan_bon { get; set; }
        public int? co_so_san_xuat_phan_bon { get; set; }
        public int? don_vi_tinh { get; set; }
    }
}
