using CoreAdminWeb.Model.LoaiThuocBaoVeThucVat;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.RequestHttp;
using CoreAdminWeb.Services.BaseServices;

namespace CoreAdminWeb.Services.LoaiThuocBaoVeThucVats
{
   
    public class LoaiThuocBaoVeThucVatService : IBaseService<LoaiThuocBaoVeThucVatModel>
    {
        private readonly string _collection = "LoaiThuocBaoVeThucVat";
        private readonly string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name";
        
        public async Task<RequestHttpResponse<List<LoaiThuocBaoVeThucVatModel>>> GetAllAsync(string query)
        {
            var response = new RequestHttpResponse<List<LoaiThuocBaoVeThucVatModel>>();
            try
            {
                string url = $"items/{_collection}?fields{Fields}=&{query}";
                var res = await RequestClient.GetAPIAsync<RequestHttpResponse<List<LoaiThuocBaoVeThucVatModel>>>(url);
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

        public async Task<RequestHttpResponse<LoaiThuocBaoVeThucVatModel>> GetByIdAsync(string id)
        {
            var response = new RequestHttpResponse<LoaiThuocBaoVeThucVatModel>();
            try
            {
                var result = await RequestClient.GetAPIAsync<RequestHttpResponse<LoaiThuocBaoVeThucVatModel>>($"items/{_collection}/{id}?fields={Fields}");
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
        
        public async Task<RequestHttpResponse<LoaiThuocBaoVeThucVatModel>> CreateAsync(LoaiThuocBaoVeThucVatModel model)
        {
            var response = new RequestHttpResponse<LoaiThuocBaoVeThucVatModel>();
            try
            {
                LoaiThuocBaoVeThucVatCRUDModel createModel = new LoaiThuocBaoVeThucVatCRUDModel(){
                    code = model.code,
                    name = model.name,
                    description = model.description,
                    status = model.status.ToString(),
                    sort = model.sort,
                };
                
                var result = await RequestClient.PostAPIAsync<RequestHttpResponse<LoaiThuocBaoVeThucVatCRUDModel>>("items/" + _collection, createModel);    
                if (result.IsSuccess)
                {
                    response.Data = new LoaiThuocBaoVeThucVatModel(){
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
        
        public async Task<RequestHttpResponse<bool>> UpdateAsync(LoaiThuocBaoVeThucVatModel model)
        {
            var response = new RequestHttpResponse<bool>(){Data =false};
            try
            {
                LoaiThuocBaoVeThucVatCRUDModel updateModel = new LoaiThuocBaoVeThucVatCRUDModel(){
                    code = model.code,
                    name = model.name,
                    description = model.description,
                    status = model.status.ToString(),
                    sort = model.sort,
                };
                var result = await RequestClient.PatchAPIAsync<RequestHttpResponse<LoaiThuocBaoVeThucVatCRUDModel>>("items/" + _collection + "/" + model.id, updateModel);    
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
        
        public async Task<RequestHttpResponse<bool>> DeleteAsync(LoaiThuocBaoVeThucVatModel model)
        {
            var response = new RequestHttpResponse<bool>();
            try
            {
                var result = await RequestClient.PatchAPIAsync<RequestHttpResponse<LoaiThuocBaoVeThucVatCRUDModel>>("items/" + _collection + "/" + model.id, new { deleted = true });    
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