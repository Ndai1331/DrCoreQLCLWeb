using CoreAdminWeb.Enums;
using CoreAdminWeb.Model.Base;
using CoreAdminWeb.Model.ThuocBaoVeThucVat;

namespace CoreAdminWeb.Model.XuatNhapKhauPhanBon
{
    public class XuatNhapKhauThuocBVTVAddOrUpdateModel : BaseModel<int>
    {
        public int? parrent_id { get; set; } = null;
        public new string status { set; get; } = Status.active.ToString();
        public DateTime? ngay_chung_tu { get; set; } = DateTime.Now;
        public string? so_chung_tu { get; set; }

        public QuanLyCoSoSanXuatThuocBVTVModel? co_so_san_xuat_thuoc_bvtv { get; set; }
        public QuanLyCoSoKinhDoanhThuocBVTVModel? co_so_kinh_doanh_thuoc_bvtv { get; set; }
        public string? hinh_thuc { get; set; } = HinhThucXuatNhapKhau.NhapKhau.ToString();
        public string? giay_phep_xnk { get; set; }
        public DateTime? ngay_cap { get; set; }
        public string? co_quan_cap { get; set; }
        public ThuocBaoVeThucVatModel? thuoc_bvtv { get; set; }
        public string? thanh_phan_ty_le { get; set; }
        public string? nuoc_xuat_nhap { get; set; }
        public string? dvt { get; set; } = "Tấn";
        public int so_luong { get; set; } = 0;
    }
}
