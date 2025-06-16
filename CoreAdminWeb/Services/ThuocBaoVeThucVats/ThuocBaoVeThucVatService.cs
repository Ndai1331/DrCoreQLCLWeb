using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.Model.ThuocBaoVeThucVat;
using CoreAdminWeb.RequestHttp;
using CoreAdminWeb.Services.BaseServices;

namespace CoreAdminWeb.Services.ThuocBaoVeThucVats
{
   
    public class ThuocBaoVeThucVatService : IBaseService<ThuocBaoVeThucVatModel>
    {
        private readonly string _collection = "ThuocBVTV";
        private readonly string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name,nhom_thuoc_bvtv.id, nhom_thuoc_bvtv.name,loai_thuoc_bvtv.id,loai_thuoc_bvtv.name,don_vi_tinh.id,don_vi_tinh.name";
        
        public async Task<RequestHttpResponse<List<ThuocBaoVeThucVatModel>>> GetAllAsync(string query)
        {
            var response = new RequestHttpResponse<List<ThuocBaoVeThucVatModel>>();
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";

                var res = await RequestClient.GetAPIAsync<RequestHttpResponse<List<ThuocBaoVeThucVatModel>>>(url);
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

        public async Task<RequestHttpResponse<ThuocBaoVeThucVatModel>> GetByIdAsync(string id)
        {
            var response = new RequestHttpResponse<ThuocBaoVeThucVatModel>();
            try
            {
                var result = await RequestClient.GetAPIAsync<RequestHttpResponse<ThuocBaoVeThucVatModel>>($"items/{_collection}/{id}?fields={Fields}");
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
        
        public async Task<RequestHttpResponse<ThuocBaoVeThucVatModel>> CreateAsync(ThuocBaoVeThucVatModel model)
        {
            var response = new RequestHttpResponse<ThuocBaoVeThucVatModel>();
            try
            {
                ThuocBaoVeThucVatCRUDModel createModel = new ThuocBaoVeThucVatCRUDModel(){
                    code = model.code,
                    name = model.name,
                    description = model.description,
                    status = model.status.ToString(),
                    sort = model.sort,
                    nhom_thuoc_bvtv = model.nhom_thuoc_bvtv?.id,
                    loai_thuoc_bvtv = model.loai_thuoc_bvtv?.id,
                    don_vi_tinh = model.don_vi_tinh?.id,
                    ten_thuong_pham = model.ten_thuong_pham,
                    doi_tuong_phong_tru = model.doi_tuong_phong_tru,
                };
                
                var result = await RequestClient.PostAPIAsync<RequestHttpResponse<ThuocBaoVeThucVatCRUDModel>>("items/" + _collection, createModel);    
                if (result.IsSuccess)
                {
                    response.Data = new ThuocBaoVeThucVatModel(){
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
        
        public async Task<RequestHttpResponse<bool>> UpdateAsync(ThuocBaoVeThucVatModel model)
        {
            var response = new RequestHttpResponse<bool>(){Data =false};
            try
            {
                ThuocBaoVeThucVatCRUDModel updateModel = new ThuocBaoVeThucVatCRUDModel(){
                    code = model.code,
                    name = model.name,
                    description = model.description,
                    status = model.status.ToString(),
                    sort = model.sort,
                    nhom_thuoc_bvtv = model.nhom_thuoc_bvtv?.id,
                    loai_thuoc_bvtv = model.loai_thuoc_bvtv?.id,
                    don_vi_tinh = model.don_vi_tinh?.id,
                    ten_thuong_pham = model.ten_thuong_pham,
                    doi_tuong_phong_tru = model.doi_tuong_phong_tru,
                    hoat_chat_ky_thuat = model.hoat_chat_ky_thuat,
                };
                var result = await RequestClient.PatchAPIAsync<RequestHttpResponse<ThuocBaoVeThucVatCRUDModel>>("items/" + _collection + "/" + model.id, updateModel);    
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
        
        public async Task<RequestHttpResponse<bool>> DeleteAsync(ThuocBaoVeThucVatModel model)
        {
            var response = new RequestHttpResponse<bool>();
            try
            {
                var result = await RequestClient.PatchAPIAsync<RequestHttpResponse<ThuocBaoVeThucVatCRUDModel>>("items/" + _collection + "/" + model.id, new { deleted = true });    
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