using CoreAdminWeb.Model.CoSoTrongTrotSanXuat;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.Services.BaseServices;

namespace CoreAdminWeb.Services.CoSoTrongTrotSanXuat
{
    public interface ICoSoTrongTrotSanXuatChiTietCayTrongService : IBaseService<CoSoTrongTrotSanXuatChiTietCayTrongModel>
    {
        Task<RequestHttpResponse<List<CoSoTrongTrotSanXuatChiTietCayTrongModel>>> CreateAsync(List<CoSoTrongTrotSanXuatChiTietCayTrongModel> model);
        Task<RequestHttpResponse<bool>> UpdateAsync(List<CoSoTrongTrotSanXuatChiTietCayTrongModel> model);
        Task<RequestHttpResponse<bool>> DeleteAsync(List<CoSoTrongTrotSanXuatChiTietCayTrongModel> model);
    }
}
