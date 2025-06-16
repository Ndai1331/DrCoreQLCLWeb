using CoreAdminWeb.Model;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.RequestHttp;
using CoreAdminWeb.Services.BaseServices;

namespace CoreAdminWeb.Services
{
   
    public class LoaiHinhSanXuatService : IBaseService<LoaiHinhSanXuatModel>
    {
        private readonly string _collection = "LoaiHinhSanXuat";
        private readonly string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name";
        
        public async Task<RequestHttpResponse<List<LoaiHinhSanXuatModel>>> GetAllAsync(string query)
        {
            var response = new RequestHttpResponse<List<LoaiHinhSanXuatModel>>();
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var res = await RequestClient.GetAPIAsync<RequestHttpResponse<List<LoaiHinhSanXuatModel>>>(url);
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

        public async Task<RequestHttpResponse<LoaiHinhSanXuatModel>> GetByIdAsync(string id)
        {
            var response = new RequestHttpResponse<LoaiHinhSanXuatModel>();
            try
            {
                var result = await RequestClient.GetAPIAsync<RequestHttpResponse<LoaiHinhSanXuatModel>>($"items/{_collection}/{id}?fields={Fields}");
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
        
        public async Task<RequestHttpResponse<LoaiHinhSanXuatModel>> CreateAsync(LoaiHinhSanXuatModel model)
        {
            var response = new RequestHttpResponse<LoaiHinhSanXuatModel>();
            try
            {
                LoaiHinhSanXuatCRUDModel createModel = new LoaiHinhSanXuatCRUDModel(){
                    code = model.code,
                    name = model.name,
                    description = model.description,
                    status = model.status.ToString(),
                    sort = model.sort,
                };
                
                var result = await RequestClient.PostAPIAsync<RequestHttpResponse<LoaiHinhSanXuatCRUDModel>>("items/" + _collection, createModel);    
                if (result.IsSuccess)
                {
                    response.Data = new LoaiHinhSanXuatModel(){
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
        
        public async Task<RequestHttpResponse<bool>> UpdateAsync(LoaiHinhSanXuatModel model)
        {
            var response = new RequestHttpResponse<bool>(){Data =false};
            try
            {
                LoaiHinhSanXuatCRUDModel updateModel = new LoaiHinhSanXuatCRUDModel(){
                    code = model.code,
                    name = model.name,
                    description = model.description,
                    status = model.status.ToString(),
                    sort = model.sort,
                };
                var result = await RequestClient.PatchAPIAsync<RequestHttpResponse<LoaiHinhSanXuatCRUDModel>>("items/" + _collection + "/" + model.id, updateModel);    
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
        
        public async Task<RequestHttpResponse<bool>> DeleteAsync(LoaiHinhSanXuatModel model)
        {
            var response = new RequestHttpResponse<bool>();
            try
            {
                var result = await RequestClient.PatchAPIAsync<RequestHttpResponse<LoaiHinhSanXuatCRUDModel>>("items/" + _collection + "/" + model.id, new { deleted = true });    
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