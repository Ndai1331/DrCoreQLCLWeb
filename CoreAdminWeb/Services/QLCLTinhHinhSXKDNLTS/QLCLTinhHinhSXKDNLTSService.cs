using CoreAdminWeb.Model;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.RequestHttp;
using CoreAdminWeb.Services.BaseServices;
using System.Net;

namespace CoreAdminWeb.Services
{
    public class QLCLTinhHinhSXKDNLTSService : IBaseService<QLCLTinhHinhSXKDNLTSModel>
    {
        private readonly string _collection = "QLCLTinhHinhSXKDNLTS";
        private const string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name"
            + ",qlcl_co_so_che_bien_nlts.id,qlcl_co_so_che_bien_nlts.name";

        /// <summary>
        /// Creates a response with error handling
        /// </summary>
        private static RequestHttpResponse<T> CreateErrorResponse<T>(Exception ex)
        {
            return new RequestHttpResponse<T>
            {
                Errors = new List<ErrorResponse> { new() { Message = ex.Message } },
                StatusCode = HttpStatusCode.InternalServerError
            };
        }

        /// <summary>
        /// Maps a model to CRUD model
        /// </summary>
        private static QLCLTinhHinhSXKDNLTSCRUDModel MapToCRUDModel(QLCLTinhHinhSXKDNLTSModel model)
        {
            return new()
            {
                ngay_ghi_nhan = model.ngay_ghi_nhan,
                thoi_gian_bat_dau = model.thoi_gian_bat_dau,
                thoi_gian_ket_thuc = model.thoi_gian_ket_thuc,
                qlcl_co_so_che_bien_nlts = model.qlcl_co_so_che_bien_nlts?.id,
                su_co_an_toan = model.su_co_an_toan,
                bien_phap_xu_ly_chat_thai = model.bien_phap_xu_ly_chat_thai,
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<QLCLTinhHinhSXKDNLTSModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<QLCLTinhHinhSXKDNLTSModel>>>(url);

                return response.IsSuccess
                    ? new RequestHttpResponse<List<QLCLTinhHinhSXKDNLTSModel>> { Data = response.Data?.Data, Meta = response.Data?.Meta }
                    : new RequestHttpResponse<List<QLCLTinhHinhSXKDNLTSModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<QLCLTinhHinhSXKDNLTSModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<QLCLTinhHinhSXKDNLTSModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<QLCLTinhHinhSXKDNLTSModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<QLCLTinhHinhSXKDNLTSModel>>($"items/{_collection}/{id}?fields={Fields}");

                return response.IsSuccess
                    ? new RequestHttpResponse<QLCLTinhHinhSXKDNLTSModel> { Data = response.Data?.Data, Meta = response.Data?.Meta }
                    : new RequestHttpResponse<QLCLTinhHinhSXKDNLTSModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<QLCLTinhHinhSXKDNLTSModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<QLCLTinhHinhSXKDNLTSModel>> CreateAsync(QLCLTinhHinhSXKDNLTSModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<QLCLTinhHinhSXKDNLTSModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = MapToCRUDModel(model);
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<QLCLTinhHinhSXKDNLTSCRUDResponseModel>>($"items/{_collection}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<QLCLTinhHinhSXKDNLTSModel> { Errors = response.Errors };
                }

                return new RequestHttpResponse<QLCLTinhHinhSXKDNLTSModel>
                {
                    Data = new()
                    {
                        id = response.Data?.Data?.id,
                    }
                };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<QLCLTinhHinhSXKDNLTSModel>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(QLCLTinhHinhSXKDNLTSModel model)
        {
            if (model == null || model.id == 0)
            {
                return new RequestHttpResponse<bool>
                {
                    Data = false,
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng chọn bản ghi để cập nhật" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var updateModel = MapToCRUDModel(model);
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<QLCLTinhHinhSXKDNLTSCRUDModel>>($"items/{_collection}/{model.id}", updateModel);

                return new RequestHttpResponse<bool>
                {
                    Data = response.IsSuccess,
                    Errors = response.Errors
                };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<bool>(ex);
            }
        }

        /// <summary>
        /// Deletes a fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> DeleteAsync(QLCLTinhHinhSXKDNLTSModel model)
        {
            if (model == null || model.id == 0)
            {
                return new RequestHttpResponse<bool>
                {
                    Data = false,
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng chọn bản ghi để xoá" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<QLCLTinhHinhSXKDNLTSCRUDModel>>($"items/{_collection}/{model.id}", new { deleted = true });

                return new RequestHttpResponse<bool>
                {
                    Data = response.IsSuccess,
                    Errors = response.Errors
                };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<bool>(ex);
            }
        }
    }
}
