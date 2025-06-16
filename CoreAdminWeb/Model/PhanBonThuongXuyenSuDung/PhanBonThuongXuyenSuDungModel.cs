using CoreAdminWeb.Enums;
using CoreAdminWeb.Model.Base;
using CoreAdminWeb.Model.PhanBon;

namespace CoreAdminWeb.Model
{
    public class PhanBonThuongXuyenSuDungModel : BaseModel<int>
    {
        public TinhModel? province { get; set; }
        public XaPhuongModel? ward{ get; set; }
        public PhanBonModel? phan_bon{ get; set; }
        public string? khu_vuc_su_dung { get; set; }
        public string? lieu_luong_trung_binh { get; set; }
        public string? thoi_diem_bon_phan { get; set; }
        public string? hinh_thuc_bon_phan { get; set; }        
        public string? loai_cay_trong_ap_dung { get; set; }

    }

    public class PhanBonThuongXuyenSuDungCRUDModel : BaseDetailModel
    {
        public new string status { set; get; } = Status.active.ToString();
        public int? province { get; set; }
        public int? ward{ get; set; }
        public int? phan_bon{ get; set; }
        public string? khu_vuc_su_dung { get; set; }
        public string? loai_cay_trong_ap_dung { get; set; }
        public string? lieu_luong_trung_binh { get; set; }
        public string? thoi_diem_bon_phan { get; set; }
        public string? hinh_thuc_bon_phan { get; set; }
    }
}
