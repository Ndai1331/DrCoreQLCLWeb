using CoreAdminWeb.Model.Base;

namespace CoreAdminWeb.Model.CoSoBiDichBenh
{
    public class CoSoBiDichBenhChiTietModel : BaseModel<int>
    {
        public CoSoBiDichBenhModel? co_so_bi_dich_benh { get; set; }
        public ViSinhVatGayHaiModel? vi_sinh_vat_gay_hai { get; set; }
        public decimal? dien_tich { get; set; }
        public decimal? muc_nhe { get; set; }
        public decimal? muc_trung_binh { get; set; }
        public decimal? muc_nang { get; set; }
        public decimal? mat_trang { get; set; }
        public string? muc_do_anh_huong { get; set; }
    }
    public class CoSoBiDichBenhChiTietCRUDModel : BaseDetailModel
    {
        public new string status { get; set; } = Status.active.ToString();
        public int? co_so_bi_dich_benh { get; set; }
        public int? vi_sinh_vat_gay_hai { get; set; }
        public decimal? dien_tich { get; set; }
        public decimal? muc_nhe { get; set; }
        public decimal? muc_trung_binh { get; set; }
        public decimal? muc_nang { get; set; }
        public decimal? mat_trang { get; set; }
        public string? muc_do_anh_huong { get; set; }
    }
}
