using CoreAdminWeb.Enums;
using CoreAdminWeb.Model.Base;

namespace CoreAdminWeb.Model
{
    public class ToChucCaNhanDuDieuKienSanXuatGiongModel : BaseModel<int>
    {
        public CoSoSanXuatGiongModel? co_so_san_xuat_giong { get; set; }
        public string? tieu_chuan_cong_bo { get; set; }
        public string? cong_bo_hop_quy { get; set; }


    }

    public class ToChucCaNhanDuDieuKienSanXuatGiongCRUDModel : BaseDetailModel
    {
        public new string status { set; get; } = Status.active.ToString();
        public int? co_so_san_xuat_giong { get; set; }
        public string? tieu_chuan_cong_bo { get; set; }
        public string? cong_bo_hop_quy { get; set; }
    }
}
