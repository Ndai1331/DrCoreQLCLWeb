using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.Model.SanXuatUngDungCongNgheCao;
using CoreAdminWeb.Services.BaseServices;

namespace CoreAdminWeb.Services.SanXuatUngDungCongNgheCao
{
    public interface ISanXuatUngDungCongNgheCaoLoaiCayTrongService : IBaseService<SanXuatUngDungCongNgheCaoLoaiCayTrongModel>
    {
        Task<RequestHttpResponse<List<SanXuatUngDungCongNgheCaoLoaiCayTrongModel>>> CreateAsync(List<SanXuatUngDungCongNgheCaoLoaiCayTrongModel> model);
        Task<RequestHttpResponse<bool>> UpdateAsync(List<SanXuatUngDungCongNgheCaoLoaiCayTrongModel> model);
        Task<RequestHttpResponse<bool>> DeleteAsync(List<SanXuatUngDungCongNgheCaoLoaiCayTrongModel> model);
    }
}
