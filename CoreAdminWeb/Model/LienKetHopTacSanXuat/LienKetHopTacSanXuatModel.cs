using CoreAdminWeb.Model.Base;

namespace CoreAdminWeb.Model.LienKetHopTacSanXuat
{
    public class LienKetHopTacSanXuatModel : BaseModel<int>
    {
        public string? mo_hinh_hop_tac { get; set; }
        public string? don_vi_chu_tri { get; set; }
        public DateTime? thoi_gian_tu { get; set; }
        public DateTime? thoi_gian_den { get; set; }
        public HinhThucLienKetModel? hinh_thuc_lien_ket { get; set; }
        public string? muc_tieu { get; set; }
        public CayGiongCayTrongModel? cay_trong { get; set; }
        public List<LienKetHopTacSanXuatDonViThamGiaModel>? don_vi_tham_gia { get; set; }
    }
    public class LienKetHopTacSanXuatCRUDModel : BaseDetailModel
    {
        public new string status { get; set; } = Status.active.ToString();
        public string? mo_hinh_hop_tac { get; set; }
        public string? don_vi_chu_tri { get; set; }
        public DateTime? thoi_gian_tu { get; set; }
        public DateTime? thoi_gian_den { get; set; }
        public int? hinh_thuc_lien_ket { get; set; }
        public int? cay_trong { get; set; }
        public string? muc_tieu { get; set; }
    }
}
