using CoreAdminWeb.Model;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.RequestHttp;
using CoreAdminWeb.Services.BaseServices;

namespace CoreAdminWeb.Services
{
   
    public class QLCLLoaiHinhKinhDoanhService : IBaseService<QLCLLoaiHinhKinhDoanhModel>
    {
        private readonly string _collection = "QLCLLoaiHinhKinhDoanh";
        private readonly string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name";
        
        public async Task<RequestHttpResponse<List<QLCLLoaiHinhKinhDoanhModel>>> GetAllAsync(string query)
        {
            var response = new RequestHttpResponse<List<QLCLLoaiHinhKinhDoanhModel>>();
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var res = await RequestClient.GetAPIAsync<RequestHttpResponse<List<QLCLLoaiHinhKinhDoanhModel>>>(url);
                if (res.IsSuccess)
                {
                    response.Data = res.Data.Data;
                }
                else
                {
                    response.Errors = res.Errors;
                }
            }
            catch (Exception ex)
            {
                response.Errors = new List<ErrorResponse> { new ErrorResponse { Message = ex.Message } };
            }
            return response;
        }   

        public async Task<RequestHttpResponse<QLCLLoaiHinhKinhDoanhModel>> GetByIdAsync(string id)
        {
            var response = new RequestHttpResponse<QLCLLoaiHinhKinhDoanhModel>();
            try
            {
                var result = await RequestClient.GetAPIAsync<RequestHttpResponse<QLCLLoaiHinhKinhDoanhModel>>($"items/{_collection}/{id}?fields={Fields}");
                if (result.IsSuccess)
                {
                    response.Data = result.Data.Data;
                }
                else if (result?.Errors != null)
                {
                    response.Errors = result.Errors;
                }
            }
            catch (Exception ex)
            {
                response.Errors = new List<ErrorResponse> { new ErrorResponse { Message = ex.Message } };
            }
            return response;
        }
        
        public async Task<RequestHttpResponse<QLCLLoaiHinhKinhDoanhModel>> CreateAsync(QLCLLoaiHinhKinhDoanhModel model)
        {
            var response = new RequestHttpResponse<QLCLLoaiHinhKinhDoanhModel>();
            try
            {
                QLCLLoaiHinhKinhDoanhCRUDModel createModel = new QLCLLoaiHinhKinhDoanhCRUDModel(){
                    code = model.code,
                    name = model.name,
                    description = model.description,
                    status = model.status.ToString(),
                    sort = model.sort,
                };
                
                var result = await RequestClient.PostAPIAsync<RequestHttpResponse<QLCLLoaiHinhKinhDoanhCRUDModel>>("items/" + _collection, createModel);    
                if (result.IsSuccess)
                {
                    response.Data = new QLCLLoaiHinhKinhDoanhModel(){
                        code = result.Data.Data.code,
                        name = result.Data.Data.name
                    };
                }
                else if (result?.Errors != null)
                {
                    response.Errors = result.Errors;
                }
            }
            catch (Exception ex)
            {
                response.Errors = new List<ErrorResponse> { new ErrorResponse { Message = ex.Message } };
            }
            return response;
        }
        
        public async Task<RequestHttpResponse<bool>> UpdateAsync(QLCLLoaiHinhKinhDoanhModel model)
        {
            var response = new RequestHttpResponse<bool>(){Data =false};
            try
            {
                QLCLLoaiHinhKinhDoanhCRUDModel updateModel = new QLCLLoaiHinhKinhDoanhCRUDModel(){
                    code = model.code,
                    name = model.name,
                    description = model.description,
                    status = model.status.ToString(),
                    sort = model.sort,
                };
                var result = await RequestClient.PatchAPIAsync<RequestHttpResponse<QLCLLoaiHinhKinhDoanhCRUDModel>>("items/" + _collection + "/" + model.id, updateModel);    
                if (result?.Data != null)
                {
                    response.Data = true;
                }
                else if (result?.Errors != null)
                {
                    response.Errors = result.Errors;
                }
            }
            catch (Exception ex)
            {
                response.Errors = new List<ErrorResponse> { new ErrorResponse { Message = ex.Message } };
            }
            return response;
        }
        
        public async Task<RequestHttpResponse<bool>> DeleteAsync(QLCLLoaiHinhKinhDoanhModel model)
        {
            var response = new RequestHttpResponse<bool>();
            try
            {
                var result = await RequestClient.PatchAPIAsync<RequestHttpResponse<QLCLLoaiHinhKinhDoanhCRUDModel>>("items/" + _collection + "/" + model.id, new { deleted = true });    
                if (result?.Data != null)
                {
                    response.Data = true;
                }
                else if (result?.Errors != null)
                {
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