using CoreAdminWeb.Enums;
using CoreAdminWeb.Model.Base;

namespace CoreAdminWeb.Model.XuatNhapKhauThuocBaoVeThucVat
{
    public class XuatNhapKhauThuocBVTVModel : BaseModel<int>
    {
        public DateTime ngay_chung_tu { get; set; }

        public QuanLyCoSoSanXuatThuocBVTVModel? co_so_san_xuat_thuoc_bvtv { get; set; }
        public QuanLyCoSoKinhDoanhThuocBVTVModel? co_so_kinh_doanh_thuoc_bvtv { get; set; }
        public string? hinh_thuc { get; set; } = HinhThucXuatNhapKhau.NhapKhau.ToString();
        public string? giay_phep_xnk { get; set; }
        public DateTime? ngay_cap { get; set; }
        public string? co_quan_cap { get; set; }
        public string? so_chung_tu { get; set; }
    }
    public class XuatNhapKhauThuocBVTVCRUDModel : BaseDetailModel
    {
        public new string status { set; get; } = Status.active.ToString();
        public DateTime ngay_chung_tu { get; set; }

        public int? co_so_san_xuat_thuoc_bvtv { get; set; }
        public int? co_so_kinh_doanh_thuoc_bvtv { get; set; }
        public string? hinh_thuc { get; set; } = HinhThucXuatNhapKhau.NhapKhau.ToString();
        public string? giay_phep_xnk { get; set; }
        public DateTime? ngay_cap { get; set; }
        public string? co_quan_cap { get; set; }
        public string? so_chung_tu { get; set; }
    }
}
