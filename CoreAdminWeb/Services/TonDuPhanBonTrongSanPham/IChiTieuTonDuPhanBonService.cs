using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.Model.TonDuPhanBonTrongSanPham;
using CoreAdminWeb.Services.BaseServices;

namespace CoreAdminWeb.Services.TonDuPhanBonTrongSanPham
{
    public interface IChiTieuTonDuPhanBonService: IBaseService<ChiTieuTonDuPhanBonModel>
    {
        Task<RequestHttpResponse<List<ChiTieuTonDuPhanBonModel>>> CreateAsync(List<ChiTieuTonDuPhanBonModel> model);
        Task<RequestHttpResponse<bool>> UpdateAsync(List<ChiTieuTonDuPhanBonModel> model);
        Task<RequestHttpResponse<bool>> DeleteAsync(List<ChiTieuTonDuPhanBonModel> model);
    }
}
