using CoreAdminWeb.Model;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.RequestHttp;
using CoreAdminWeb.Services.BaseServices;

namespace CoreAdminWeb.Services
{
   
    public class CayGiongCayTrongService : IBaseService<CayGiongCayTrongModel>
    {
        private readonly string _collection = "CayGiongCayTrong";
        private readonly string Fields = "*,loai_cay_trong.id,loai_cay_trong.name, user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name";
        
        public async Task<RequestHttpResponse<List<CayGiongCayTrongModel>>> GetAllAsync(string query)
        {
            var response = new RequestHttpResponse<List<CayGiongCayTrongModel>>();
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var res = await RequestClient.GetAPIAsync<RequestHttpResponse<List<CayGiongCayTrongModel>>>(url);
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

        public async Task<RequestHttpResponse<CayGiongCayTrongModel>> GetByIdAsync(string id)
        {
            var response = new RequestHttpResponse<CayGiongCayTrongModel>();
            try
            {
                var result = await RequestClient.GetAPIAsync<RequestHttpResponse<CayGiongCayTrongModel>>($"items/{_collection}/{id}?fields={Fields}");
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
        
        public async Task<RequestHttpResponse<CayGiongCayTrongModel>> CreateAsync(CayGiongCayTrongModel model)
        {
            var response = new RequestHttpResponse<CayGiongCayTrongModel>();
            try
            {
                CayGiongCayTrongCRUDModel createModel = new CayGiongCayTrongCRUDModel(){
                    code = model.code,
                    name = model.name,
                    loai_cay_trong = model.loai_cay_trong?.id,
                    nhom_cay_trong = model.nhom_cay_trong,
                    nguon_goc = model.nguon_goc,
                    co_quan_phat_trien_giong = model.co_quan_phat_trien_giong,
                    nang_suat_trung_binh = model.nang_suat_trung_binh,
                    chat_luong = model.chat_luong,
                    kha_nang_chong_chiu_sau_benh = model.kha_nang_chong_chiu_sau_benh,
                    khu_vuc_thich_nghi = model.khu_vuc_thich_nghi,
                    description = model.description,
                    status = model.status.ToString(),
                    sort = model.sort,
                };
                
                var result = await RequestClient.PostAPIAsync<RequestHttpResponse<CayGiongCayTrongCRUDModel>>("items/" + _collection, createModel);    
                if (result.IsSuccess)
                {
                    response.Data = new CayGiongCayTrongModel(){
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
        
        public async Task<RequestHttpResponse<bool>> UpdateAsync(CayGiongCayTrongModel model)
        {
            var response = new RequestHttpResponse<bool>(){Data =false};
            try
            {
                CayGiongCayTrongCRUDModel updateModel = new CayGiongCayTrongCRUDModel(){
                    code = model.code,
                    name = model.name,
                    loai_cay_trong = model.loai_cay_trong?.id,
                    nhom_cay_trong = model.nhom_cay_trong,
                    nguon_goc = model.nguon_goc,
                    co_quan_phat_trien_giong = model.co_quan_phat_trien_giong,
                    nang_suat_trung_binh = model.nang_suat_trung_binh,
                    chat_luong = model.chat_luong,
                    kha_nang_chong_chiu_sau_benh = model.kha_nang_chong_chiu_sau_benh,
                    khu_vuc_thich_nghi = model.khu_vuc_thich_nghi,
                    description = model.description,
                    status = model.status.ToString(),
                    sort = model.sort,
                };
                var result = await RequestClient.PatchAPIAsync<RequestHttpResponse<CayGiongCayTrongCRUDModel>>("items/" + _collection + "/" + model.id, updateModel);    
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
        
        public async Task<RequestHttpResponse<bool>> DeleteAsync(CayGiongCayTrongModel model)
        {
            var response = new RequestHttpResponse<bool>();
            try
            {
                var result = await RequestClient.PatchAPIAsync<RequestHttpResponse<CayGiongCayTrongCRUDModel>>("items/" + _collection + "/" + model.id, new { deleted = true });    
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