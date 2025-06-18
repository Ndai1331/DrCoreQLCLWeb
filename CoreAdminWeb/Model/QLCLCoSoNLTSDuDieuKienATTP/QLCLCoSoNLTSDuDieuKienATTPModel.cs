using CoreAdminWeb.Model.Base;

namespace CoreAdminWeb.Model
{
    public class QLCLCoSoNLTSDuDieuKienATTPModel : BaseModel<int>
    {
        public TinhModel? province { get; set; }
        public XaPhuongModel? ward { get; set; }
        public string? dia_chi { get; set; }
        public string? dien_thoai { get; set; }
        public string? dai_dien { get; set; }
        public string? so_giay_chung_nhan { get; set; }
        public QLCLLoaiHinhKinhDoanhModel? loai_hinh_kinh_doanh { get; set; }
        public DateTime? ngay_cap { get; set; }
        public DateTime? ngay_het_hieu_luc { get; set; }
        public DateTime? ngay_tham_dinh { get; set; }
        public string? co_quan_cap { get; set; }
        public string? xu_ly_ket_qua { get; set; }
        public string? he_thong_quan_ly_chat_luong { get; set; }
        public Enums.KetQuaKiemTraDinhKy? ket_qua_tham_dinh { get; set; } = Enums.KetQuaKiemTraDinhKy.Dat;
    }
    public class QLCLCoSoNLTSDuDieuKienATTPCRUDModel : BaseDetailModel
    {
        public new string status { get; set; } = Status.active.ToString();
        public int? province { get; set; }
        public int? ward { get; set; }
        public int? loai_hinh_kinh_doanh { get; set; }
        public string? dia_chi { get; set; }
        public string? dien_thoai { get; set; }
        public string? dai_dien { get; set; }
        public string? so_giay_chung_nhan { get; set; }
        public DateTime? ngay_cap { get; set; }
        public DateTime? ngay_het_hieu_luc { get; set; }
        public DateTime? ngay_tham_dinh { get; set; }
        public string? co_quan_cap { get; set; }
        public string? xu_ly_ket_qua { get; set; }
        public string? he_thong_quan_ly_chat_luong { get; set; }
        public Enums.KetQuaKiemTraDinhKy? ket_qua_tham_dinh { get; set; } = Enums.KetQuaKiemTraDinhKy.Dat;
    }


    public class QLCLCoSoNLTSDuDieuKienATTPCRUDResponseModel
    {
        public int id { get; set; }
    }
}
