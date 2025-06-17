using CoreAdminWeb.Model;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.RequestHttp;
using CoreAdminWeb.Services.BaseServices;

namespace CoreAdminWeb.Services
{
   
    public class SanPhamSanXuatService : IBaseService<SanPhamSanXuatModel>
    {
        private readonly string _collection = "QLCLSanPhamSanXuat";
        private readonly string Fields = "*,loai_sp.id,loai_sp.name,loai_sp.english_name, user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name";
        
        public async Task<RequestHttpResponse<List<SanPhamSanXuatModel>>> GetAllAsync(string query)
        {
            var response = new RequestHttpResponse<List<SanPhamSanXuatModel>>();
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var res = await RequestClient.GetAPIAsync<RequestHttpResponse<List<SanPhamSanXuatModel>>>(url);
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

        public async Task<RequestHttpResponse<SanPhamSanXuatModel>> GetByIdAsync(string id)
        {
            var response = new RequestHttpResponse<SanPhamSanXuatModel>();
            try
            {
                var result = await RequestClient.GetAPIAsync<RequestHttpResponse<SanPhamSanXuatModel>>($"items/{_collection}/{id}?fields={Fields}");
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
        
        public async Task<RequestHttpResponse<SanPhamSanXuatModel>> CreateAsync(SanPhamSanXuatModel model)
        {
            var response = new RequestHttpResponse<SanPhamSanXuatModel>();
            try
            {
                SanPhamSanXuatCRUDModel createModel = new SanPhamSanXuatCRUDModel(){
                    code = model.code,
                    name = model.name,
                    loai_sp = model.loai_sp?.id,
                    tieu_chuan_chat_luong = model.tieu_chuan_chat_luong,
                    tieu_chuan_kiem_dich = model.tieu_chuan_kiem_dich,
                    english_name = model.english_name,
                    description = model.description,
                    status = model.status.ToString(),
                    sort = model.sort,
                };
                
                var result = await RequestClient.PostAPIAsync<RequestHttpResponse<SanPhamSanXuatCRUDModel>>("items/" + _collection, createModel);    
                if (result.IsSuccess)
                {
                    response.Data = new SanPhamSanXuatModel(){
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
        
        public async Task<RequestHttpResponse<bool>> UpdateAsync(SanPhamSanXuatModel model)
        {
            var response = new RequestHttpResponse<bool>(){Data =false};
            try
            {
                SanPhamSanXuatCRUDModel updateModel = new SanPhamSanXuatCRUDModel(){
                    code = model.code,
                    name = model.name,
                    english_name = model.english_name,
                    loai_sp = model.loai_sp?.id,
                    tieu_chuan_chat_luong = model.tieu_chuan_chat_luong,
                    tieu_chuan_kiem_dich = model.tieu_chuan_kiem_dich,
                    description = model.description,
                    status = model.status.ToString(),
                    sort = model.sort,
                };
                var result = await RequestClient.PatchAPIAsync<RequestHttpResponse<SanPhamSanXuatCRUDModel>>("items/" + _collection + "/" + model.id, updateModel);    
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
        
        public async Task<RequestHttpResponse<bool>> DeleteAsync(SanPhamSanXuatModel model)
        {
            var response = new RequestHttpResponse<bool>();
            try
            {
                var result = await RequestClient.PatchAPIAsync<RequestHttpResponse<SanPhamSanXuatCRUDModel>>("items/" + _collection + "/" + model.id, new { deleted = true });    
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