using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.Model.TonDuThuocBVTVTrongSanPham;
using CoreAdminWeb.Services.BaseServices;

namespace CoreAdminWeb.Services.TonDuThuocBVTVTrongSanPham
{
    public interface IChiTieuTonDuThuocBVTVService: IBaseService<ChiTieuTonDuThuocBVTVModel>
    {
        Task<RequestHttpResponse<List<ChiTieuTonDuThuocBVTVModel>>> CreateAsync(List<ChiTieuTonDuThuocBVTVModel> model);
        Task<RequestHttpResponse<bool>> UpdateAsync(List<ChiTieuTonDuThuocBVTVModel> model);
        Task<RequestHttpResponse<bool>> DeleteAsync(List<ChiTieuTonDuThuocBVTVModel> model);
    }
}
