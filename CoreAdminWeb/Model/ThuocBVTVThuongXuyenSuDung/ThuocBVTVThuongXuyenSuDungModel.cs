using CoreAdminWeb.Enums;
using CoreAdminWeb.Model.Base;
using CoreAdminWeb.Model.ThuocBaoVeThucVat;

namespace CoreAdminWeb.Model
{
    public class ThuocBVTVThuongXuyenSuDungModel : BaseModel<int>
    {
        public TinhModel? province { get; set; }
        public XaPhuongModel? ward{ get; set; }
        public ThuocBaoVeThucVatModel? thuoc_bvtv{ get; set; }
        public string? khu_vuc_su_dung { get; set; }
        public string? lieu_luong_trung_binh { get; set; }
        public string? thoi_diem_bon_phan { get; set; }
        public string? hinh_thuc_bon_phan { get; set; }        
        public string? loai_cay_trong_ap_dung { get; set; }

    }

    public class ThuocBVTVThuongXuyenSuDungCRUDModel : BaseDetailModel
    {
        public new string status { set; get; } = Status.active.ToString();
        public int? province { get; set; }
        public int? ward{ get; set; }
        public int? thuoc_bvtv{ get; set; }
        public string? khu_vuc_su_dung { get; set; }
        public string? loai_cay_trong_ap_dung { get; set; }
        public string? lieu_luong_trung_binh { get; set; }
        public string? thoi_diem_bon_phan { get; set; }
        public string? hinh_thuc_bon_phan { get; set; }
    }
}
