using CoreAdminWeb.Enums;
using CoreAdminWeb.Model.Base;
using CoreAdminWeb.Model.CoSoBuonBan;
using CoreAdminWeb.Model.QuanLyCoSoSanXuatPhanBon;

namespace CoreAdminWeb.Model  
{
    public class ViPhamSanXuatKinhDoanhPhanBonModel : BaseModel<int>
    {
        public TrangThaiXuLy? status { set; get; }  = TrangThaiXuLy.ChuaXuLy;
        public DateTime? ngay_phat_hien { set; get; }
        public DateTime? ngay_xu_ly{ set; get; }
        public string? don_vi_kiem_tra{ set; get; }
        public string? dia_diem_vi_pham{ set; get; }
        public LoaiToChuc? loai_to_chuc { set; get; }
        public QuanLyCoSoSanXuatPhanBonModel? co_so_san_xuat_phan_bon { set; get; }
        public CoSoDuDieuKienBuonBanPhanBonModel? co_so_du_dieu_kien_buon_ban_phan_bon { set; get; }
        public string? doi_tuong_vi_pham{ set; get; }
        public string? noi_dung_vi_pham{ set; get; }
        public string? hinh_thuc_xu_ly{ set; get; }

    }
    public class ViPhamSanXuatKinhDoanhPhanBonCRUDModel : BaseDetailModel
    {
        public TrangThaiXuLy? status { set; get; } = TrangThaiXuLy.ChuaXuLy;
        public DateTime? ngay_phat_hien { set; get; }
        public DateTime? ngay_xu_ly{ set; get; }
        public string? don_vi_kiem_tra{ set; get; }
        public string? dia_diem_vi_pham{ set; get; }
        public LoaiToChuc? loai_to_chuc { set; get; }
        public int? co_so_san_xuat_phan_bon { set; get; }
        public int? co_so_du_dieu_kien_buon_ban_phan_bon { set; get; }
        public string? doi_tuong_vi_pham{ set; get; }
        public string? noi_dung_vi_pham{ set; get; }
        public string? hinh_thuc_xu_ly{ set; get; }
    }
}