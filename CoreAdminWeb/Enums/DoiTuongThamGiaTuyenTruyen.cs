using System.ComponentModel;

namespace CoreAdminWeb.Enums
{
    public enum DoiTuongThamGiaTuyenTruyen
    {
        [Description("Hộ nông dân")]
        HoNongDan = 1,
        [Description("Doanh nghiệp")]
        DoanhNghiep,
        [Description("Người tiêu dùng")]
        NguoiTieuDung,
        [Description("Khác")]
        Khac
    }
}
