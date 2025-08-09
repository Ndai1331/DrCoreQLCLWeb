using System.ComponentModel;

namespace CoreAdminWeb.Enums
{
    public enum LoaiSanPham
    {
        [Description("Sản phẩm nguồn gốc thực vật")]
        SanPhamNguonGocThucVat = 1,
        [Description("Sản phẩm nguồn gốc động vật")]
        SanPhamNguonGocDongVat,
        [Description("Sản phẩm thủy sản")]
        SanPhamThuySan
    }
}
