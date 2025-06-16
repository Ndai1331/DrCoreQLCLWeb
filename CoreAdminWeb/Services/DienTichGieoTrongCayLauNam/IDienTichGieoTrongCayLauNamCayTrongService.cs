using CoreAdminWeb.Model.DienTichGieoTrongCayLauNam;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.Services.BaseServices;

namespace CoreAdminWeb.Services.DienTichGieoTrongCayLauNam
{
    public interface IDienTichGieoTrongCayLauNamCayTrongService : IBaseService<DienTichGieoTrongCayLauNamCayTrongModel>
    {
        Task<RequestHttpResponse<List<DienTichGieoTrongCayLauNamCayTrongModel>>> CreateAsync(List<DienTichGieoTrongCayLauNamCayTrongModel> model);
        Task<RequestHttpResponse<bool>> UpdateAsync(List<DienTichGieoTrongCayLauNamCayTrongModel> model);
        Task<RequestHttpResponse<bool>> DeleteAsync(List<DienTichGieoTrongCayLauNamCayTrongModel> model);
    }
}
