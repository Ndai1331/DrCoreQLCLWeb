using CoreAdminWeb.Enums;
using CoreAdminWeb.Model.Base;
using CoreAdminWeb.Model.CoSoTrongTrotSanXuat;

namespace CoreAdminWeb.Model.CoSoChungNhanATTP
{
    public class CoSoChungNhanATTPModel : BaseModel<int>
    {
        public new TrangThaiGiayPhep status { get; set; } = TrangThaiGiayPhep.DangHoatDong;
        public HinhThucCap hinh_thuc_cap { get; set; } = HinhThucCap.CapMoi;
        public LoaiGiayChungNhan loai_gcn { get; set; } = LoaiGiayChungNhan.AnToanThucPham;
        public TinhModel? province { get; set; }
        public XaPhuongModel? ward { get; set; }
        public string? so_gcn { get; set; }
        public DateTime? ngay_cap { get; set; }
        public DateTime? ngay_het_han { get; set; }
        public string? co_quan_cap_phep { get; set; }
        public CoSoTrongTrotSanXuatModel? co_so_trong_trot_san_xuat { get; set; }
    }
    public class CoSoChungNhanATTPCRUDModel : BaseDetailModel
    {
        public new string status { get; set; } = TrangThaiGiayPhep.DangHoatDong.ToString();
        public string hinh_thuc_cap { get; set; } = HinhThucCap.CapMoi.ToString();
        public string loai_gcn { get; set; } = LoaiGiayChungNhan.AnToanThucPham.ToString();
        public int? province { get; set; }
        public int? ward { get; set; }
        public string? so_gcn { get; set; }
        public DateTime? ngay_cap { get; set; }
        public DateTime? ngay_het_han { get; set; }
        public string? co_quan_cap_phep { get; set; }
        public int? co_so_trong_trot_san_xuat { get; set; }
    }
}
