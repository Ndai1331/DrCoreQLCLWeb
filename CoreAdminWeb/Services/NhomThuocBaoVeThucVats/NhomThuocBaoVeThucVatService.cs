using CoreAdminWeb.Model.NhomThuocBaoVeThucVat;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.RequestHttp;
using CoreAdminWeb.Services.BaseServices;

namespace CoreAdminWeb.Services.NhomThuocBaoVeThucVats
{
   
    public class NhomThuocBaoVeThucVatService : IBaseService<NhomThuocBaoVeThucVatModel>
    {
        private readonly string _collection = "NhomThuocBaoVeThucVat";
        private readonly string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name";
        public async Task<RequestHttpResponse<List<NhomThuocBaoVeThucVatModel>>> GetAllAsync(string query)
        {
            var response = new RequestHttpResponse<List<NhomThuocBaoVeThucVatModel>>();
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var res = await RequestClient.GetAPIAsync<RequestHttpResponse<List<NhomThuocBaoVeThucVatModel>>>(url);
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

        public async Task<RequestHttpResponse<NhomThuocBaoVeThucVatModel>> GetByIdAsync(string id)
        {
            var response = new RequestHttpResponse<NhomThuocBaoVeThucVatModel>();
            try
            {
                var result = await RequestClient.GetAPIAsync<RequestHttpResponse<NhomThuocBaoVeThucVatModel>>($"items/{_collection}/{id}?fields={Fields}");
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
        
        public async Task<RequestHttpResponse<NhomThuocBaoVeThucVatModel>> CreateAsync(NhomThuocBaoVeThucVatModel model)
        {
            var response = new RequestHttpResponse<NhomThuocBaoVeThucVatModel>();
            try
            {
                NhomThuocBaoVeThucVatCRUDModel createModel = new NhomThuocBaoVeThucVatCRUDModel(){
                    code = model.code,
                    name = model.name,
                    description = model.description,
                    status = model.status.ToString(),
                    sort = model.sort,
                };
                
                var result = await RequestClient.PostAPIAsync<RequestHttpResponse<NhomThuocBaoVeThucVatCRUDModel>>("items/" + _collection, createModel);    
                if (result.IsSuccess)
                {
                    response.Data = new NhomThuocBaoVeThucVatModel(){
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
        
        public async Task<RequestHttpResponse<bool>> UpdateAsync(NhomThuocBaoVeThucVatModel model)
        {
            var response = new RequestHttpResponse<bool>(){Data =false};
            try
            {
                NhomThuocBaoVeThucVatCRUDModel updateModel = new NhomThuocBaoVeThucVatCRUDModel(){
                    code = model.code,
                    name = model.name,
                    description = model.description,
                    status = model.status.ToString(),
                    sort = model.sort,
                };
                var result = await RequestClient.PatchAPIAsync<RequestHttpResponse<NhomThuocBaoVeThucVatCRUDModel>>("items/" + _collection + "/" + model.id, updateModel);    
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
        
        public async Task<RequestHttpResponse<bool>> DeleteAsync(NhomThuocBaoVeThucVatModel model)
        {
            var response = new RequestHttpResponse<bool>();
            try
            {
                var result = await RequestClient.PatchAPIAsync<RequestHttpResponse<NhomThuocBaoVeThucVatCRUDModel>>("items/" + _collection + "/" + model.id, new { deleted = true });    
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