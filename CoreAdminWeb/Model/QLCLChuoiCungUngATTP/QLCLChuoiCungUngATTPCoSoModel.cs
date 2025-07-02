using CoreAdminWeb.Model.User;
using CoreAdminWeb.Enums;

namespace CoreAdminWeb.Model
{
    public class QLCLChuoiCungUngATTPCoSoModel 
    {
        public int id { get; set; }
        public UserModel? user_created { get; set; }
        public DateTime date_created { get; set; } = DateTime.Now;
        public UserModel? user_updated { get; set; }
        public DateTime? date_updated { get; set; } = DateTime.Now;
        public QLCLChuoiCungUngATTPModel? chuoi_cung_ung { get; set; }
        public LoaiCoSoChuoiCungUng? loai_co_so { get; set; } = LoaiCoSoChuoiCungUng.CoSoCheBien;
        public QLCLCoSoCheBienNLTSModel? co_so_che_bien_nlts { get; set; }
        public QLCLCoSoNLTSDuDieuKienATTPModel? co_so_nlts_du_dieu_kien_attp { get; set; }
        public bool? deleted { get; set; } = false;
        public int? sort { get; set; } = 0;
    }
    public class QLCLChuoiCungUngATTPCoSoCRUDModel
    {
        public int? chuoi_cung_ung { get; set; }
        public LoaiCoSoChuoiCungUng? loai_co_so { get; set; } = LoaiCoSoChuoiCungUng.CoSoCheBien;
        public int? co_so_che_bien_nlts { get; set; }
        public int? co_so_nlts_du_dieu_kien_attp { get; set; }
        public int? sort { get; set; } = 0;
        public bool? deleted { get; set; } = false;

    }
}
