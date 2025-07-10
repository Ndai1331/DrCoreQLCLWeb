using System.ComponentModel;

namespace CoreAdminWeb.Enums
{
    public enum HinhThucTuyenTruyen
    {
        [Description("Hội nghị")]
        HoiNghi = 1,
        [Description("Tập huấn")]
        TapHuan,
        [Description("Truyền thông")]
        TruyenThong,
        [Description("Tờ rơi")]
        ToRoi,
        [Description("Áp phích")]
        ApPhich,
        [Description("Khác")]
        Khac
    }
}
