using CoreAdminWeb.Enums;
using CoreAdminWeb.Model.Base;

namespace CoreAdminWeb.Model  
{
    public class CoSoViPhamTrongLinhVucCheBienModel : BaseModel<int>
    {
        public TrangThaiXuLy? status { set; get; }  = TrangThaiXuLy.ChuaXuLy;
        public DateTime? ngay_phat_hien { set; get; }
        public DateTime? ngay_xu_ly{ set; get; }
        public string? don_vi_kiem_tra_xu_ly { set; get; }
        public string? dia_diem_vi_pham{ set; get; }
        public CoSoSanXuatCheBienModel? co_so_vi_pham { set; get; }
        public string? doi_tuong_vi_pham{ set; get; }
        public string? noi_dung_vi_pham{ set; get; }
        public string? hinh_thuc_xu_ly{ set; get; }

    }
    public class CoSoViPhamTrongLinhVucCheBienCRUDModel : BaseDetailModel
    {
        public TrangThaiXuLy? status { set; get; } = TrangThaiXuLy.ChuaXuLy;
        public DateTime? ngay_phat_hien { set; get; }
        public DateTime? ngay_xu_ly{ set; get; }
        public string? don_vi_kiem_tra_xu_ly { set; get; }
        public string? dia_diem_vi_pham{ set; get; }
        public int? co_so_vi_pham { set; get; }
        public string? doi_tuong_vi_pham{ set; get; }
        public string? noi_dung_vi_pham{ set; get; }
        public string? hinh_thuc_xu_ly{ set; get; }
    }
}