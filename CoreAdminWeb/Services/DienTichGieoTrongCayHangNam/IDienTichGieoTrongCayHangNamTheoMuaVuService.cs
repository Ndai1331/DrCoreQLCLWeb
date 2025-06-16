using CoreAdminWeb.Model.DienTichGieoTrongCayHangNam;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.Services.BaseServices;

namespace CoreAdminWeb.Services.DienTichGieoTrongCayHangNam
{
    public interface IDienTichGieoTrongCayHangNamTheoMuaVuService : IBaseService<DienTichGieoTrongCayHangNamTheoMuaVuModel>
    {
        Task<RequestHttpResponse<List<DienTichGieoTrongCayHangNamTheoMuaVuModel>>> CreateAsync(List<DienTichGieoTrongCayHangNamTheoMuaVuModel> model);
        Task<RequestHttpResponse<bool>> UpdateAsync(List<DienTichGieoTrongCayHangNamTheoMuaVuModel> model);
        Task<RequestHttpResponse<bool>> DeleteAsync(List<DienTichGieoTrongCayHangNamTheoMuaVuModel> model);
    }
}
