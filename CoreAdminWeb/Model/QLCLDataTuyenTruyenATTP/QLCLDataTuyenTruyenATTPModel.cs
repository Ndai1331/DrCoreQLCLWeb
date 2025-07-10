using CoreAdminWeb.Enums;
using CoreAdminWeb.Model.Base;

namespace CoreAdminWeb.Model
{
    public class QLCLDataTuyenTruyenATTPModel : BaseModel<int>
    {
        public DateTime? ngay_thuc_hien { get; set; }
        public string? dia_diem { get; set; }
        public TinhModel? province { get; set; }
        public XaPhuongModel? ward { get; set; }
        public string? co_quan_thuc_hien { get; set; }
        public HinhThucTuyenTruyen? hinh_thuc { get; set; } = HinhThucTuyenTruyen.HoiNghi;
        public int? so_luong { get; set; }
        public DonViTinhTuyenTruyen? don_vi_tinh { get; set; } = DonViTinhTuyenTruyen.Buoi;
        public DoiTuongThamGiaTuyenTruyen? doi_tuong_tham_gia { get; set; } = DoiTuongThamGiaTuyenTruyen.HoNongDan;
        public int? so_luong_nguoi_tham_gia { get; set; }
        public string? noi_dung { get; set; }
    }
    public class QLCLDataTuyenTruyenATTPCRUDModel : BaseDetailModel
    {
        public new string status { get; set; } = Status.active.ToString();
        public DateTime? ngay_thuc_hien { get; set; }
        public string? dia_diem { get; set; }
        public int? province { get; set; }
        public int? ward { get; set; }
        public string? co_quan_thuc_hien { get; set; }
        public HinhThucTuyenTruyen? hinh_thuc { get; set; } = HinhThucTuyenTruyen.HoiNghi;
        public int? so_luong { get; set; }
        public DonViTinhTuyenTruyen? don_vi_tinh { get; set; } = DonViTinhTuyenTruyen.Buoi;
        public DoiTuongThamGiaTuyenTruyen? doi_tuong_tham_gia { get; set; } = DoiTuongThamGiaTuyenTruyen.HoNongDan;
        public int? so_luong_nguoi_tham_gia { get; set; }
        public string? noi_dung { get; set; }
    }
}
