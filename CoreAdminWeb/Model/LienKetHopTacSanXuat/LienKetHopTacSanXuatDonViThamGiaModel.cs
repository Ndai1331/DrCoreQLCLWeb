using CoreAdminWeb.Model.Base;
using CoreAdminWeb.Model.CoSoTrongTrotSanXuat;

namespace CoreAdminWeb.Model.LienKetHopTacSanXuat
{
    public class LienKetHopTacSanXuatDonViThamGiaModel : BaseModel<int>
    {
        public LienKetHopTacSanXuatModel? lien_ket_hop_tac_san_xuat { get; set; }
        public CoSoTrongTrotSanXuatModel? co_so_trong_trot_san_xuat { get; set; }
        public string? vai_tro { get; set; }
        public decimal? dien_tich { get; set; }
        public decimal? san_luong { get; set; }
    }
    public class LienKetHopTacSanXuatDonViThamGiaCRUDModel : BaseDetailModel
    {
        public new string status { get; set; } = Status.active.ToString();
        public int? lien_ket_hop_tac_san_xuat { get; set; }
        public int? co_so_trong_trot_san_xuat { get; set; }
        public string? vai_tro { get; set; }
        public decimal? dien_tich { get; set; }
        public decimal? san_luong { get; set; }
    }
}
