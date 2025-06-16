using CoreAdminWeb.Model.DuBaoDichBenh;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.Services.BaseServices;

namespace CoreAdminWeb.Services.DuBaoDichBenh
{
    public interface IDuBaoDichBenhChiTietService : IBaseService<DuBaoDichBenhChiTietModel>
    {
        Task<RequestHttpResponse<List<DuBaoDichBenhChiTietModel>>> CreateAsync(List<DuBaoDichBenhChiTietModel> model);
        Task<RequestHttpResponse<bool>> UpdateAsync(List<DuBaoDichBenhChiTietModel> model);
        Task<RequestHttpResponse<bool>> DeleteAsync(List<DuBaoDichBenhChiTietModel> model);
    }
}
