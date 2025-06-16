using CoreAdminWeb.Model.Base;
using CoreAdminWeb.Enums;

namespace CoreAdminWeb.Model
{
    public class CoSoSanXuatGiongModel : BaseModel<int>
    {
        public QuyMoEnum quy_mo { get; set; } = QuyMoEnum.QuyMoNho;
        public string? so_giay_phep_hoat_dong { get; set; }
        public DateTime? ngay_cap { get; set; }
        public DateTime? ngay_het_han { get; set; }
        public string? co_quan_cap_phep { get; set; }
        public string? dia_chi { get; set; }
        public string? cong_nghe_san_xuat { get; set; }
        public TinhModel? province { get; set; }
        public XaPhuongModel? ward { get; set; }
    }
    public class CoSoSanXuatGiongCRUDModel : BaseDetailModel
    {
        public new string status { get; set; } = Status.active.ToString();
        public int? quy_mo { get; set; }
        public string? so_giay_phep_hoat_dong { get; set; }
        public DateTime? ngay_cap { get; set; }
        public DateTime? ngay_het_han { get; set; }
        public string? co_quan_cap_phep { get; set; }
        public string? dia_chi { get; set; }
        public string? cong_nghe_san_xuat { get; set; }
        public int? province { get; set; }
        public int? ward { get; set; }
    }
}
