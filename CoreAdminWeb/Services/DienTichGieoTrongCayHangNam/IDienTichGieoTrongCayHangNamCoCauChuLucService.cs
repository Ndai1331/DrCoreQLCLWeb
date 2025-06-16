using CoreAdminWeb.Model.DienTichGieoTrongCayHangNam;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.Services.BaseServices;

namespace CoreAdminWeb.Services.DienTichGieoTrongCayHangNam
{
    public interface IDienTichGieoTrongCayHangNamCoCauChuLucService : IBaseService<DienTichGieoTrongCayHangNamCoCauChuLucModel>
    {
        Task<RequestHttpResponse<List<DienTichGieoTrongCayHangNamCoCauChuLucModel>>> CreateAsync(List<DienTichGieoTrongCayHangNamCoCauChuLucModel> model);
        Task<RequestHttpResponse<bool>> UpdateAsync(List<DienTichGieoTrongCayHangNamCoCauChuLucModel> model);
        Task<RequestHttpResponse<bool>> DeleteAsync(List<DienTichGieoTrongCayHangNamCoCauChuLucModel> model);
    }
}
