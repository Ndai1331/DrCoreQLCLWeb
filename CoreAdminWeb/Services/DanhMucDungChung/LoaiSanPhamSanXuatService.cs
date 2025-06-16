using CoreAdminWeb.Model;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.RequestHttp;
using CoreAdminWeb.Services.BaseServices;

namespace CoreAdminWeb.Services
{
   
    public class LoaiSanPhamSanXuatService : IBaseService<LoaiSanPhamSanXuatModel>
    {
        private readonly string _collection = "LoaiSanPhanSanXuat";
        private readonly string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name";
        
        public async Task<RequestHttpResponse<List<LoaiSanPhamSanXuatModel>>> GetAllAsync(string query)
        {
            var response = new RequestHttpResponse<List<LoaiSanPhamSanXuatModel>>();
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var res = await RequestClient.GetAPIAsync<RequestHttpResponse<List<LoaiSanPhamSanXuatModel>>>(url);
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

        public async Task<RequestHttpResponse<LoaiSanPhamSanXuatModel>> GetByIdAsync(string id)
        {
            var response = new RequestHttpResponse<LoaiSanPhamSanXuatModel>();
            try
            {
                var result = await RequestClient.GetAPIAsync<RequestHttpResponse<LoaiSanPhamSanXuatModel>>($"items/{_collection}/{id}?fields={Fields}");
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
        
        public async Task<RequestHttpResponse<LoaiSanPhamSanXuatModel>> CreateAsync(LoaiSanPhamSanXuatModel model)
        {
            var response = new RequestHttpResponse<LoaiSanPhamSanXuatModel>();
            try
            {
                LoaiSanPhamSanXuatCRUDModel createModel = new LoaiSanPhamSanXuatCRUDModel(){
                    code = model.code,
                    name = model.name,
                    english_name = model.english_name,
                    description = model.description,
                    status = model.status.ToString(),
                    sort = model.sort,
                };
                
                var result = await RequestClient.PostAPIAsync<RequestHttpResponse<LoaiSanPhamSanXuatCRUDModel>>("items/" + _collection, createModel);    
                if (result.IsSuccess)
                {
                    response.Data = new LoaiSanPhamSanXuatModel(){
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
        
        public async Task<RequestHttpResponse<bool>> UpdateAsync(LoaiSanPhamSanXuatModel model)
        {
            var response = new RequestHttpResponse<bool>(){Data =false};
            try
            {
                LoaiSanPhamSanXuatCRUDModel updateModel = new LoaiSanPhamSanXuatCRUDModel(){
                    code = model.code,
                    name = model.name,
                    english_name = model.english_name,
                    description = model.description,
                    status = model.status.ToString(),
                    sort = model.sort,
                };
                var result = await RequestClient.PatchAPIAsync<RequestHttpResponse<LoaiSanPhamSanXuatCRUDModel>>("items/" + _collection + "/" + model.id, updateModel);    
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
        
        public async Task<RequestHttpResponse<bool>> DeleteAsync(LoaiSanPhamSanXuatModel model)
        {
            var response = new RequestHttpResponse<bool>();
            try
            {
                var result = await RequestClient.PatchAPIAsync<RequestHttpResponse<LoaiSanPhamSanXuatCRUDModel>>("items/" + _collection + "/" + model.id, new { deleted = true });    
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