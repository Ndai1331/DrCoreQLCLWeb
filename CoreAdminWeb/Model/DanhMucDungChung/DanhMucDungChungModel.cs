using CoreAdminWeb.Enums;
using CoreAdminWeb.Model.Base;

namespace CoreAdminWeb.Model
{

    public class QLCLLoaiSanPhamModel : BaseModel<int>
    {
    }
    public class QLCLLoaiSanPhamCRUDModel : BaseDetailModel
    {
        public new string status { set; get; } = Status.active.ToString();
    }



    public class QLCLLoaiHinhCoSoModel : BaseModel<int>
    {
    }
    public class QLCLLoaiHinhCoSoCRUDModel : BaseDetailModel
    {
        public new string status { set; get; } = Status.active.ToString();
    }

    public class QLCLNguyenLieuCheBienModel : BaseModel<int>
    {
    }
    public class QLCLNguyenLieuCheBienCRUDModel : BaseDetailModel
    {
        public new string status { set; get; } = Status.active.ToString();
    }


    public class QLCLPhamViHoatDongModel : BaseModel<int>
    {
        public TinhModel? province { get; set; }
        public XaPhuongModel? ward { get; set; }
        public string? khu_vuc_hoat_dong { get; set; }
        public string? thong_tin_vung_nguyen_lieu { get; set; }
        public string? doi_tac_tieu_thu { get; set; }
        public bool? pham_vi_noi_dia { get; set; }
        public bool? pham_vi_xuat_khau { get; set; }
    }
    public class QLCLPhamViHoatDongCRUDModel : BaseDetailModel
    {
        public new string status { set; get; } = Status.active.ToString();
        public int? province { get; set; }
        public int? ward { get; set; }
        public string? khu_vuc_hoat_dong { get; set; }
        public string? thong_tin_vung_nguyen_lieu { get; set; }
        public string? doi_tac_tieu_thu { get; set; }
        public bool? pham_vi_noi_dia { get; set; }
        public bool? pham_vi_xuat_khau { get; set; }
    }



    

    public class CountryModel : BaseModel<int>
    {
    }
    public class CountryCRUDModel : BaseDetailModel
    {
        public new string status { set; get; } = Status.active.ToString();
    }
    public class LoaiHinhKinhDoanhModel : BaseModel<int>
    {
    }
    public class LoaiHinhKinhDoanhCRUDModel : BaseDetailModel
    {
        public new string status { set; get; } = Status.active.ToString();
    }

    public class LoaiHinhCanhTacModel : BaseModel<int>
    {
    }
    public class LoaiHinhCanhTacCRUDModel : BaseDetailModel
    {
        public new string status { set; get; } = Status.active.ToString();

    }
    public class LoaiHinhSanXuatModel : BaseModel<int>
    {
    }
    public class LoaiHinhSanXuatCRUDModel : BaseDetailModel
    {
        public new string status { set; get; } = Status.active.ToString();

    }

    public class NhomViSinhVatGayHaiModel : BaseModel<int>
    {
    }
    public class NhomViSinhVatGayHaiCRUDModel : BaseDetailModel
    {
        public new string status { set; get; } = Status.active.ToString();
    }

    public class ViSinhVatGayHaiModel : BaseModel<int>
    {
        public NhomViSinhVatGayHaiModel? nhom_vi_sinh_vat_gay_hai { set; get; }
        public string? loai_benh_gay_ra { set; get; }
    }
    public class ViSinhVatGayHaiCRUDModel : BaseDetailModel
    {
        public int? nhom_vi_sinh_vat_gay_hai { set; get; }
        public string? loai_benh_gay_ra { set; get; }
        public new string status { set; get; } = Status.active.ToString();
    }

    public class DonViTinhModel : BaseModel<int>
    {
    }
    public class DonViTinhCRUDModel : BaseDetailModel
    {
        public new string status { set; get; } = Status.active.ToString();

    }

    public class TinhModel : BaseModel<int>
    {
    }
    public class TinhCRUDModel : BaseDetailModel
    {
        public new string status { set; get; } = Status.active.ToString();

    }
    public class XaPhuongModel : BaseModel<int>
    {
        public int? ProvinceId { get; set; }
    }
    public class XaPhuongCRUDModel : BaseDetailModel
    {
        public new string status { set; get; } = Status.active.ToString();
    }


    public class HinhThucCanhTacModel : BaseModel<int>
    {
    }
    public class HinhThucCanhTacCRUDModel : BaseDetailModel
    {
        public new string status { set; get; } = Status.active.ToString();

    }


    public class LoaiSanPhamSanXuatModel : BaseModel<int>
    {
        public string? english_name { set; get; }
    }
    public class LoaiSanPhamSanXuatCRUDModel : BaseDetailModel
    {
        public string? english_name { set; get; }
        public new string status { set; get; } = Status.active.ToString();
    }

    public class SanPhamSanXuatModel : BaseModel<int>
    {
        public LoaiSanPhamSanXuatModel? loai_san_pham_san_xuat_id { set; get; }
        public string? english_name { set; get; }
        public string? tieu_chuan_chat_luong { set; get; }
        public string? tieu_chuan_kiem_dich { set; get; }
    }
    public class SanPhamSanXuatCRUDModel : BaseDetailModel
    {
        public int? loai_san_pham_san_xuat_id { set; get; }
        public string? english_name { set; get; }
        public string? tieu_chuan_chat_luong { set; get; }
        public string? tieu_chuan_kiem_dich { set; get; }
        public new string status { set; get; } = Status.active.ToString();
    }




    public class LoaiCayTrongModel : BaseModel<int>
    {
    }
    public class LoaiCayTrongCRUDModel : BaseDetailModel
    {
        public new string status { set; get; } = Status.active.ToString();
    }

    public class CayGiongCayTrongModel : BaseModel<int>
    {
        public LoaiCayTrongModel? loai_cay_trong { set; get; }
        public int? loai_cay_trong_id { set; get; }

        public NhomCayTrong nhom_cay_trong { set; get; }
        public NguonGoc nguon_goc { set; get; }
        public string? co_quan_phat_trien_giong { set; get; }
        public decimal? nang_suat_trung_binh { set; get; }
        public string? chat_luong { set; get; }
        public string? kha_nang_chong_chiu_sau_benh { set; get; }
        public string? khu_vuc_thich_nghi { set; get; }
    }
    public class CayGiongCayTrongCRUDModel : BaseDetailModel
    {
        public int? loai_cay_trong { set; get; }
        public NhomCayTrong nhom_cay_trong { set; get; }
        public NguonGoc nguon_goc { set; get; }
        public string? co_quan_phat_trien_giong { set; get; }
        public decimal? nang_suat_trung_binh { set; get; }
        public string? chat_luong { set; get; }
        public string? kha_nang_chong_chiu_sau_benh { set; get; }
        public string? khu_vuc_thich_nghi { set; get; }
        public new string status { set; get; } = Status.active.ToString();
    }


    public class HinhThucLienKetModel : BaseModel<int>
    {
    }
    public class HinhThucLienKetCRUDModel : BaseDetailModel
    {
        public new string status { set; get; } = Status.active.ToString();
    }


    public class LoaiHinhThienTaiModel : BaseModel<int>
    {
    }
    public class LoaiHinhThienTaiCRUDModel : BaseDetailModel
    {
        public new string status { set; get; } = Status.active.ToString();
    }
}