using CoreAdminWeb.Model.LoaiPhanBon;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.RequestHttp;
using CoreAdminWeb.Services.BaseServices;

namespace CoreAdminWeb.Services.LoaiPhanBons
{
    public class LoaiPhanBonService : IBaseService<LoaiPhanBonModel>
    {
        private readonly string _collection = "LoaiPhanBon";
        private readonly string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name";
        public async Task<RequestHttpResponse<LoaiPhanBonModel>> CreateAsync(LoaiPhanBonModel model)
        {
            var response = new RequestHttpResponse<LoaiPhanBonModel>();
            try
            {
                LoaiPhanBonCRUDModel createModel = new LoaiPhanBonCRUDModel()
                {
                    code = model.code,
                    name = model.name,
                    description = model.description,
                    status = model.status.ToString(),
                    sort = model.sort,
                };

                var result = await RequestClient.PostAPIAsync<RequestHttpResponse<LoaiPhanBonCRUDModel>>("items/" + _collection, createModel);
                if (result.IsSuccess)
                {
                    response.Data = new LoaiPhanBonModel()
                    {
                        code = result.Data?.Data?.code,
                        name = result.Data?.Data?.name
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

        public async Task<RequestHttpResponse<bool>> DeleteAsync(LoaiPhanBonModel model)
        {
            var response = new RequestHttpResponse<bool>();
            try
            {
                var result = await RequestClient.PatchAPIAsync<RequestHttpResponse<LoaiPhanBonCRUDModel>>("items/" + _collection + "/" + model.id, new { deleted = true });
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

        public async Task<RequestHttpResponse<List<LoaiPhanBonModel>>> GetAllAsync(string query)
        {
            var response = new RequestHttpResponse<List<LoaiPhanBonModel>>();
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var res = await RequestClient.GetAPIAsync<RequestHttpResponse<List<LoaiPhanBonModel>>>(url);
                if (res.IsSuccess)
                {
                    response.Data = res.Data?.Data;
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

        public async Task<RequestHttpResponse<LoaiPhanBonModel>> GetByIdAsync(string id)
        {
            var response = new RequestHttpResponse<LoaiPhanBonModel>();
            try
            {
                var result = await RequestClient.GetAPIAsync<RequestHttpResponse<LoaiPhanBonModel>>($"items/{_collection}/{id}?fields={Fields}");
                if (result.IsSuccess)
                {
                    response.Data = result.Data?.Data;
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

        public async Task<RequestHttpResponse<bool>> UpdateAsync(LoaiPhanBonModel model)
        {
            var response = new RequestHttpResponse<bool>() { Data = false };
            try
            {
                LoaiPhanBonCRUDModel updateModel = new LoaiPhanBonCRUDModel()
                {
                    code = model.code,
                    name = model.name,
                    description = model.description,
                    status = model.status.ToString(),
                    sort = model.sort,
                };
                var result = await RequestClient.PatchAPIAsync<RequestHttpResponse<LoaiPhanBonCRUDModel>>("items/" + _collection + "/" + model.id, updateModel);
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
