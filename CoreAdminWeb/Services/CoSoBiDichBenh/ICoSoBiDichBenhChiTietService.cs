using CoreAdminWeb.Model.CoSoBiDichBenh;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.Services.BaseServices;

namespace CoreAdminWeb.Services.CoSoBiDichBenh
{
    public interface ICoSoBiDichBenhChiTietService : IBaseService<CoSoBiDichBenhChiTietModel>
    {
        Task<RequestHttpResponse<List<CoSoBiDichBenhChiTietModel>>> CreateAsync(List<CoSoBiDichBenhChiTietModel> model);
        Task<RequestHttpResponse<bool>> UpdateAsync(List<CoSoBiDichBenhChiTietModel> model);
        Task<RequestHttpResponse<bool>> DeleteAsync(List<CoSoBiDichBenhChiTietModel> model);
    }
}
