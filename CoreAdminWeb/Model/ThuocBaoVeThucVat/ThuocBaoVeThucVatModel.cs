using CoreAdminWeb.Model.Base;
using CoreAdminWeb.Model.LoaiThuocBaoVeThucVat;
using CoreAdminWeb.Model.NhomThuocBaoVeThucVat;

namespace CoreAdminWeb.Model.ThuocBaoVeThucVat  
{
    public class ThuocBaoVeThucVatModel : BaseModel<int>
    {
        public NhomThuocBaoVeThucVatModel? nhom_thuoc_bvtv { set; get; }
        public LoaiThuocBaoVeThucVatModel? loai_thuoc_bvtv { set; get; }
        public DonViTinhModel? don_vi_tinh { set; get; }
        public string? ten_thuong_pham { set; get; }
        public string? doi_tuong_phong_tru { set; get; }
        public string? hoat_chat_ky_thuat { set; get; }
    }
    public class ThuocBaoVeThucVatCRUDModel : BaseDetailModel
    {
        public int? nhom_thuoc_bvtv { set; get; }
        public int? loai_thuoc_bvtv { set; get; }
        public int? don_vi_tinh { set; get; }
        public string? ten_thuong_pham { set; get; }
        public string? doi_tuong_phong_tru { set; get; }
        public string? hoat_chat_ky_thuat { set; get; }
        public new string status { set; get; } = Status.active.ToString();
    }
}