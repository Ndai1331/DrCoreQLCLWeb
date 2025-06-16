using CoreAdminWeb.Model.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreAdminWeb.Model
{
    /// <summary>
    /// Model representing a fertilizer production facility
    /// </summary>
    public class QuanLyCoSoKinhDoanhThuocBVTVModel : BaseModel<int>
    {
        public TinhModel? province { get; set; }
        public XaPhuongModel? ward { get; set; }
        public string dia_chi { get; set; } = string.Empty;
        public string dien_thoai { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string so_cccd { get; set; } = string.Empty;
        public string nguoi_dai_dien { get; set; } = string.Empty;
        public string ma_so_thue { get; set; } = string.Empty;
        public string so_giay_phep_kinh_doanh { get; set; } = string.Empty;
        public string so_gcn_du_dieu_kien { get; set; } = string.Empty;
        public DateTime? ngay_cap { get; set; }
        public DateTime? ngay_het_han { get; set; }
        public string co_quan_cap_phep { get; set; } = string.Empty;
        public LoaiHinhKinhDoanhModel? loai_hinh_kinh_doanh { get; set; }
    }

    /// <summary>
    /// Model for CRUD operations on fertilizer production facilities
    /// </summary>
    public class QuanLyCoSoKinhDoanhThuocBVTVCRUDModel : BaseDetailModel
    {
        public string status { get; set; }
        public int? province { get; set; }
        public int? ward { get; set; }
        public int? loai_hinh_kinh_doanh { get; set; }
        public string dia_chi { get; set; } = string.Empty;
        public string dien_thoai { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string so_cccd { get; set; } = string.Empty;
        public string nguoi_dai_dien { get; set; } = string.Empty;
        public string ma_so_thue { get; set; } = string.Empty;
        public string so_giay_phep_kinh_doanh { get; set; } = string.Empty;
        public string so_gcn_du_dieu_kien { get; set; } = string.Empty;
        public DateTime? ngay_cap { get; set; }
        public DateTime? ngay_het_han { get; set; }
        public string co_quan_cap_phep { get; set; } = string.Empty;
    }

}
