using CoreAdminWeb.Enums;
using CoreAdminWeb.Model.Base;
using CoreAdminWeb.Model;

namespace CoreAdminWeb.Model.CayTrongDuocBaoHo
{
    public class CayTrongDuocBaoHoModel : BaseModel<int>
    {
        public string? to_chuc_ca_nhan { get; set; }
        public string? dia_chi { get; set; }
        public string? ma_so_bao_ho { get; set; }
        public string? pham_vi_bao_ho { get; set; }
        public DateTime? ngay_cap { get; set; }
        public DateTime? ngay_het_han { get; set; }
        public TinhModel? province { get; set; }
        public XaPhuongModel? ward { get; set; }
        public CayGiongCayTrongModel? cay_giong_cay_trong { get; set; }
    }

    public class CayTrongDuocBaoHoCRUDModel : BaseDetailModel
    {
        public new string status { get; set; } = Status.active.ToString();
        public string? to_chuc_ca_nhan { get; set; }
        public string? dia_chi { get; set; }
        public string? ma_so_bao_ho { get; set; }
        public string? pham_vi_bao_ho { get; set; }
        public DateTime? ngay_cap { get; set; }
        public DateTime? ngay_het_han { get; set; }
        public int? province { get; set; }
        public int? ward { get; set; }
        public int? cay_giong_cay_trong { get; set; }
    }
}
