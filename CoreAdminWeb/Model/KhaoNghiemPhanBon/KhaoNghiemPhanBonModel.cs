using CoreAdminWeb.Model.Base;
using CoreAdminWeb.Model.PhanBon;
using CoreAdminWeb.Model.QuanLyCoSoSanXuatPhanBon;

namespace CoreAdminWeb.Model.KhaoNghiemPhanBon
{
    public class KhaoNghiemPhanBonModel : BaseModel<int>
    {
        public string? dia_diem_thuc_hien { get; set; }
        public string? loai_hinh_khao_nghiem { get; set; }
        public string? muc_tieu { get; set; }
        public string? thanh_phan_cong_thuc { get; set; }
        public string? chi_tieu_ky_thuat { get; set; }
        public string? quy_mo_khao_nghiem { get; set; }
        public string? tieu_chuan_thu_nghiem { get; set; }
        public DateTime? ngay_bat_dau { get; set; }
        public DateTime? ngay_ket_thuc { get; set; }
        public PhanBonModel? phan_bon { get; set; }
        public QuanLyCoSoSanXuatPhanBonModel? co_so_san_xuat_phan_bon { get; set; }
    }

    public class KhaoNghiemPhanBonCRUDModel : BaseDetailModel
    {
        public new string status { set; get; } = Status.active.ToString();
        public string? dia_diem_thuc_hien { get; set; }
        public string? loai_hinh_khao_nghiem { get; set; }
        public string? muc_tieu { get; set; }
        public string? thanh_phan_cong_thuc { get; set; }
        public string? chi_tieu_ky_thuat { get; set; }
        public string? quy_mo_khao_nghiem { get; set; }
        public string? tieu_chuan_thu_nghiem { get; set; }
        public DateTime? ngay_bat_dau { get; set; }
        public DateTime? ngay_ket_thuc { get; set; }
        public int? phan_bon { get; set; }
        public int? co_so_san_xuat_phan_bon { get; set; }
    }
}
