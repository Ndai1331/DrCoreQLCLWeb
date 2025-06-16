using CoreAdminWeb.Model;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.RequestHttp;
using CoreAdminWeb.Services.BaseServices;

namespace CoreAdminWeb.Services
{
   
    public class NhomViSinhVatGayHaiService : IBaseService<NhomViSinhVatGayHaiModel>
    {
        private readonly string _collection = "NhomViSinhVatGayHai";
        private readonly string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name";
        
        public async Task<RequestHttpResponse<List<NhomViSinhVatGayHaiModel>>> GetAllAsync(string query)
        {
            var response = new RequestHttpResponse<List<NhomViSinhVatGayHaiModel>>();
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var res = await RequestClient.GetAPIAsync<RequestHttpResponse<List<NhomViSinhVatGayHaiModel>>>(url);
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

        public async Task<RequestHttpResponse<NhomViSinhVatGayHaiModel>> GetByIdAsync(string id)
        {
            var response = new RequestHttpResponse<NhomViSinhVatGayHaiModel>();
            try
            {
                var result = await RequestClient.GetAPIAsync<RequestHttpResponse<NhomViSinhVatGayHaiModel>>($"items/{_collection}/{id}?fields={Fields}");
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
        
        public async Task<RequestHttpResponse<NhomViSinhVatGayHaiModel>> CreateAsync(NhomViSinhVatGayHaiModel model)
        {
            var response = new RequestHttpResponse<NhomViSinhVatGayHaiModel>();
            try
            {
                NhomViSinhVatGayHaiCRUDModel createModel = new NhomViSinhVatGayHaiCRUDModel(){
                    code = model.code,
                    name = model.name,
                    description = model.description,
                    status = model.status.ToString(),
                    sort = model.sort,
                };
                
                var result = await RequestClient.PostAPIAsync<RequestHttpResponse<NhomViSinhVatGayHaiCRUDModel>>("items/" + _collection, createModel);    
                if (result.IsSuccess)
                {
                    response.Data = new NhomViSinhVatGayHaiModel(){
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
        
        public async Task<RequestHttpResponse<bool>> UpdateAsync(NhomViSinhVatGayHaiModel model)
        {
            var response = new RequestHttpResponse<bool>(){Data =false};
            try
            {
                NhomViSinhVatGayHaiCRUDModel updateModel = new NhomViSinhVatGayHaiCRUDModel(){
                    code = model.code,
                    name = model.name,
                    description = model.description,
                    status = model.status.ToString(),
                    sort = model.sort,
                };
                var result = await RequestClient.PatchAPIAsync<RequestHttpResponse<NhomViSinhVatGayHaiCRUDModel>>("items/" + _collection + "/" + model.id, updateModel);    
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
        
        public async Task<RequestHttpResponse<bool>> DeleteAsync(NhomViSinhVatGayHaiModel model)
        {
            var response = new RequestHttpResponse<bool>();
            try
            {
                var result = await RequestClient.PatchAPIAsync<RequestHttpResponse<NhomViSinhVatGayHaiCRUDModel>>("items/" + _collection + "/" + model.id, new { deleted = true });    
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