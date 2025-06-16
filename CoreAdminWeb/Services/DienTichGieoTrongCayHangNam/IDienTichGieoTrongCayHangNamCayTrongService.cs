using CoreAdminWeb.Model.DienTichGieoTrongCayHangNam;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.Services.BaseServices;

namespace CoreAdminWeb.Services.DienTichGieoTrongCayHangNam
{
    public interface IDienTichGieoTrongCayHangNamCayTrongService : IBaseService<DienTichGieoTrongCayHangNamCayTrongModel>
    {
        Task<RequestHttpResponse<List<DienTichGieoTrongCayHangNamCayTrongModel>>> CreateAsync(List<DienTichGieoTrongCayHangNamCayTrongModel> model);
        Task<RequestHttpResponse<bool>> UpdateAsync(List<DienTichGieoTrongCayHangNamCayTrongModel> model);
        Task<RequestHttpResponse<bool>> DeleteAsync(List<DienTichGieoTrongCayHangNamCayTrongModel> model);
    }
}
