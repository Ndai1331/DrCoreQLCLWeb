using CoreAdminWeb.Model;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.RequestHttp;
using CoreAdminWeb.Services.BaseServices;

namespace CoreAdminWeb.Services
{
   
    public class ViSinhVatGayHaiService : IBaseService<ViSinhVatGayHaiModel>
    {
        private readonly string _collection = "ViSinhVatGayHai";
        private readonly string Fields = "*,nhom_vi_sinh_vat_gay_hai.id,nhom_vi_sinh_vat_gay_hai.name, user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name";
        
        public async Task<RequestHttpResponse<List<ViSinhVatGayHaiModel>>> GetAllAsync(string query)
        {
            var response = new RequestHttpResponse<List<ViSinhVatGayHaiModel>>();
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var res = await RequestClient.GetAPIAsync<RequestHttpResponse<List<ViSinhVatGayHaiModel>>>(url);
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

        public async Task<RequestHttpResponse<ViSinhVatGayHaiModel>> GetByIdAsync(string id)
        {
            var response = new RequestHttpResponse<ViSinhVatGayHaiModel>();
            try
            {
                var result = await RequestClient.GetAPIAsync<RequestHttpResponse<ViSinhVatGayHaiModel>>($"items/{_collection}/{id}?fields={Fields}");
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
        
        public async Task<RequestHttpResponse<ViSinhVatGayHaiModel>> CreateAsync(ViSinhVatGayHaiModel model)
        {
            var response = new RequestHttpResponse<ViSinhVatGayHaiModel>();
            try
            {
                ViSinhVatGayHaiCRUDModel createModel = new ViSinhVatGayHaiCRUDModel(){
                    code = model.code,
                    name = model.name,
                    nhom_vi_sinh_vat_gay_hai = model.nhom_vi_sinh_vat_gay_hai?.id,
                    loai_benh_gay_ra = model.loai_benh_gay_ra,
                    description = model.description,
                    status = model.status.ToString(),
                    sort = model.sort,
                };
                
                var result = await RequestClient.PostAPIAsync<RequestHttpResponse<ViSinhVatGayHaiCRUDModel>>("items/" + _collection, createModel);    
                if (result.IsSuccess)
                {
                    response.Data = new ViSinhVatGayHaiModel(){
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
        
        public async Task<RequestHttpResponse<bool>> UpdateAsync(ViSinhVatGayHaiModel model)
        {
            var response = new RequestHttpResponse<bool>(){Data =false};
            try
            {
                ViSinhVatGayHaiCRUDModel updateModel = new ViSinhVatGayHaiCRUDModel(){
                    code = model.code,
                    name = model.name,
                    nhom_vi_sinh_vat_gay_hai = model.nhom_vi_sinh_vat_gay_hai?.id,
                    loai_benh_gay_ra = model.loai_benh_gay_ra,
                    description = model.description,
                    status = model.status.ToString(),
                    sort = model.sort,
                };
                var result = await RequestClient.PatchAPIAsync<RequestHttpResponse<ViSinhVatGayHaiCRUDModel>>("items/" + _collection + "/" + model.id, updateModel);    
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
        
        public async Task<RequestHttpResponse<bool>> DeleteAsync(ViSinhVatGayHaiModel model)
        {
            var response = new RequestHttpResponse<bool>();
            try
            {
                var result = await RequestClient.PatchAPIAsync<RequestHttpResponse<ViSinhVatGayHaiCRUDModel>>("items/" + _collection + "/" + model.id, new { deleted = true });    
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