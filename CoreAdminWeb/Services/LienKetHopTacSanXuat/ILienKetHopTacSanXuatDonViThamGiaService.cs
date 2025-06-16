using CoreAdminWeb.Model.LienKetHopTacSanXuat;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.Services.BaseServices;

namespace CoreAdminWeb.Services.LienKetHopTacSanXuat
{
    public interface ILienKetHopTacSanXuatDonViThamGiaService : IBaseService<LienKetHopTacSanXuatDonViThamGiaModel>
    {
        Task<RequestHttpResponse<List<LienKetHopTacSanXuatDonViThamGiaModel>>> CreateAsync(List<LienKetHopTacSanXuatDonViThamGiaModel> model);
        Task<RequestHttpResponse<bool>> UpdateAsync(List<LienKetHopTacSanXuatDonViThamGiaModel> model);
        Task<RequestHttpResponse<bool>> DeleteAsync(List<LienKetHopTacSanXuatDonViThamGiaModel> model);
    }
}
