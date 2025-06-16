using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Model;

namespace CoreAdminWeb.Services
{
    public interface ICoSoSanXuatGiongChiTietService : IBaseService<CoSoSanXuatGiongChiTietModel>
    {
        Task<RequestHttpResponse<List<CoSoSanXuatGiongChiTietModel>>> CreateAsync(List<CoSoSanXuatGiongChiTietModel> model);
        Task<RequestHttpResponse<bool>> UpdateAsync(List<CoSoSanXuatGiongChiTietModel> model);
        Task<RequestHttpResponse<bool>> DeleteAsync(List<CoSoSanXuatGiongChiTietModel> model);
    }
}
