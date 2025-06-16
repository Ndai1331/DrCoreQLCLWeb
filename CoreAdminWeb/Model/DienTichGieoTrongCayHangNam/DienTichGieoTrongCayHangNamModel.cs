using CoreAdminWeb.Model.Base;

namespace CoreAdminWeb.Model.DienTichGieoTrongCayHangNam
{
    public class DienTichGieoTrongCayHangNamModel : BaseModel<int>
    {
        public TinhModel? province { get; set; }
        public XaPhuongModel? ward { get; set; }
        public string? dia_diem_gieo_trong { get; set; }
        public DateTime? ngay_du_lieu { get; set; }
        public decimal? ke_hoach_nam { get; set; }
        public decimal? dien_tich_trong_moi { get; set; }
        public decimal? tong_dien_tich { get; set; }
        public string? vung_sinh_thai { get; set; }
        public LoaiHinhCanhTacModel? loai_hinh_canh_tac { get; set; }
        public string? he_thong_tuoi_tieu { get; set; }
    }
    public class DienTichGieoTrongCayHangNamCRUDModel : BaseDetailModel
    {
        public new string status { get; set; } = Status.active.ToString();
        public int? province { get; set; }
        public int? ward { get; set; }
        public string? dia_diem_gieo_trong { get; set; }
        public DateTime? ngay_du_lieu { get; set; }
        public decimal? ke_hoach_nam { get; set; }
        public decimal? dien_tich_trong_moi { get; set; }
        public decimal? tong_dien_tich { get; set; }
        public string? vung_sinh_thai { get; set; }
        public int? loai_hinh_canh_tac { get; set; }
        public string? he_thong_tuoi_tieu { get; set; }
    }
}
