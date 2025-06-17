using CoreAdminWeb.Model.Base;

namespace CoreAdminWeb.Model
{
    public class CoSoSanXuatCheBienModel : BaseModel<int>
    {
        public TinhModel? province { get; set; }
        public XaPhuongModel? ward { get; set; }
        public string? dia_chi { get; set; }
        public string? dien_thoai { get; set; }
        public string? email { get; set; }
        public string? so_cccd { get; set; }
        public string? dai_dien { get; set; }
        public decimal? cong_suat_tan_nam { get; set; }
        public string? so_giay_phep { get; set; }
        public QLCLLoaiHinhKinhDoanhModel? loai_hinh_kinh_doanh { get; set; }
        public DateTime? ngay_cap { get; set; }
        public DateTime? thoi_han_den { get; set; }
        public string? co_quan_cap_phep { get; set; }
        public string? san_pham { get; set; }
        public string? thi_truong { get; set; }
        public string? quy_trinh_san_xuat { get; set; }
        public string? chung_nhan_tieu_chuan { get; set; }
        public Enums.KetQuaKiemTraDinhKy? ket_qua_kiem_tra { get; set; } = Enums.KetQuaKiemTraDinhKy.Dat;
    }
    public class CoSoSanXuatCheBienCRUDModel : BaseDetailModel
    {
        public new string status { get; set; } = Status.active.ToString();
        public int? province { get; set; }
        public int? ward { get; set; }
        public string? dia_chi { get; set; }
        public string? dien_thoai { get; set; }
        public string? email { get; set; }
        public string? so_cccd { get; set; }
        public string? dai_dien { get; set; }
        public decimal? cong_suat_tan_nam { get; set; }
        public string? so_giay_phep { get; set; }
        public int? loai_hinh_kinh_doanh { get; set; }
        public DateTime? ngay_cap { get; set; }
        public DateTime? thoi_han_den { get; set; }
        public string? co_quan_cap_phep { get; set; }
        public string? san_pham { get; set; }
        public string? thi_truong { get; set; }
        public string? quy_trinh_san_xuat { get; set; }
        public string? chung_nhan_tieu_chuan { get; set; }
        public Enums.KetQuaKiemTraDinhKy? ket_qua_kiem_tra { get; set; } = Enums.KetQuaKiemTraDinhKy.Dat;
    }
}
