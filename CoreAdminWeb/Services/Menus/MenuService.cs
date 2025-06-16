using CoreAdminWeb.Model.Menus;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.RequestHttp;
namespace CoreAdminWeb.Services.Menus
{
    public interface IMenuService
    {
        Task<RequestHttpResponse<List<MenuResponse>>> GetMenusAsync(int external_system_id);
    }

    public class MenuService : IMenuService
    {
        public MenuService()
        {
        }

        public async Task<RequestHttpResponse<List<MenuResponse>>> GetMenusAsync(int external_system_id = 2)
        {
            string url = $"items/Menu?fields=id,status,sort,code,name,parent_code, parent_id, icon, external_system_id&sort=sort&filter[external_system_id][_eq]={external_system_id}";
            var response = new RequestHttpResponse<List<MenuResponse>>();
            try
            {
                var result = await RequestClient.GetAPIAsync<RequestHttpResponse<List<MenuResponse>>>(url);
                if (result.IsSuccess)
                {
                    response.Data = result.Data.Data;
                }else{
                    response.Errors = result.Errors;
                }
            }
            catch (Exception ex)
            {
                response.Errors = new List<ErrorResponse> { new ErrorResponse { Message = ex.Message } };
            }
            return response;
        }
    }
} 