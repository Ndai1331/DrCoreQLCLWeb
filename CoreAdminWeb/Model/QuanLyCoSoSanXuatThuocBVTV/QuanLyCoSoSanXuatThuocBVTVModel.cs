using CoreAdminWeb.Enums;
using CoreAdminWeb.Model.Base;

namespace CoreAdminWeb.Model
{
    /// <summary>
    /// Model representing a fertilizer production facility
    /// </summary>
    public class QuanLyCoSoSanXuatThuocBVTVModel : BaseModel<int>
    {
        public TinhModel? province { get; set; }

        public XaPhuongModel? ward{ get; set; }

        public QLCLLoaiHinhKinhDoanhModel? loai_hinh_kinh_doanh { get; set; }
        public LoaiHinhSanXuat? loai_hinh_san_xuat { get; set; }
        public string dia_chi { get; set; } = string.Empty;

        public string dien_thoai { get; set; } = string.Empty;

        public string email { get; set; } = string.Empty;

        public string so_cccd { get; set; } = string.Empty;

        public string nguoi_dai_dien { get; set; } = string.Empty;

        public string pham_vi_phan_phoi { get; set; } = string.Empty;

        public decimal? cong_suat_thiet_ke { get; set; }

        public string so_gcn_du_dieu_kien { get; set; } = string.Empty;

        public DateTime? ngay_cap { get; set; }

        public string co_quan_cap_phep { get; set; } = string.Empty;

        public string chung_nhan_tieu_chuan_chat_luong { get; set; } = string.Empty;

        public string nguon_nguyen_lieu_dau_vao { get; set; } = string.Empty;
    }

    /// <summary>
    /// Model for CRUD operations on fertilizer production facilities
    /// </summary>
    public class QuanLyCoSoSanXuatThuocBVTVCRUDModel : BaseDetailModel
    {
        public new string status { set; get; } = Status.active.ToString();
        public int? province { get; set; }
        public int? ward { get; set; }
        public int? loai_hinh_kinh_doanh { get; set; }
        public LoaiHinhSanXuat? loai_hinh_san_xuat { get; set; }
        public string dia_chi { get; set; } = string.Empty;
        public string dien_thoai { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string so_cccd { get; set; } = string.Empty;
        public string nguoi_dai_dien { get; set; } = string.Empty;
        public string pham_vi_phan_phoi { get; set; } = string.Empty;
        public decimal? cong_suat_thiet_ke { get; set; }
        public string so_gcn_du_dieu_kien { get; set; } = string.Empty;
        public DateTime? ngay_cap { get; set; }
        public string co_quan_cap_phep { get; set; } = string.Empty;
        public string chung_nhan_tieu_chuan_chat_luong { get; set; } = string.Empty;
        public string nguon_nguyen_lieu_dau_vao { get; set; } = string.Empty;
    }

}
