using CoreAdminWeb.Enums;
using CoreAdminWeb.Model.DienTichGieoTrongCayHangNam;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.RequestHttp;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text.Json;

namespace CoreAdminWeb.Services.DienTichGieoTrongCayHangNam
{
    public class DienTichGieoTrongCayHangNamCoCauChuLucService : IDienTichGieoTrongCayHangNamCoCauChuLucService
    {
        private readonly string _collection = "DienTichGieoTrongCayHangNamCoCauChuLuc";
        private const string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name"
            + ",dien_tich_gieo_trong_cay_hang_nam.id"
            + ",cay_giong_cay_trong.id,cay_giong_cay_trong.name";

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
        private static DienTichGieoTrongCayHangNamCoCauChuLucCRUDModel MapToCRUDModel(DienTichGieoTrongCayHangNamCoCauChuLucModel model)
        {
            return new()
            {
                status = model.status.ToString(),
                description = model.description,
                sort = model.sort,
                dien_tich_gieo_trong_cay_hang_nam = model.dien_tich_gieo_trong_cay_hang_nam?.id,
                cay_giong_cay_trong = model.cay_giong_cay_trong?.id
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<DienTichGieoTrongCayHangNamCoCauChuLucModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<DienTichGieoTrongCayHangNamCoCauChuLucModel>>>(url);

                return response.IsSuccess
                    ? new RequestHttpResponse<List<DienTichGieoTrongCayHangNamCoCauChuLucModel>> { Data = response.Data?.Data }
                    : new RequestHttpResponse<List<DienTichGieoTrongCayHangNamCoCauChuLucModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<DienTichGieoTrongCayHangNamCoCauChuLucModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<DienTichGieoTrongCayHangNamCoCauChuLucModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<DienTichGieoTrongCayHangNamCoCauChuLucModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<DienTichGieoTrongCayHangNamCoCauChuLucModel>>($"items/{_collection}/{id}?fields={Fields}");

                return response.IsSuccess
                    ? new RequestHttpResponse<DienTichGieoTrongCayHangNamCoCauChuLucModel> { Data = response.Data?.Data }
                    : new RequestHttpResponse<DienTichGieoTrongCayHangNamCoCauChuLucModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<DienTichGieoTrongCayHangNamCoCauChuLucModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<DienTichGieoTrongCayHangNamCoCauChuLucModel>> CreateAsync(DienTichGieoTrongCayHangNamCoCauChuLucModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<DienTichGieoTrongCayHangNamCoCauChuLucModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = MapToCRUDModel(model);
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<DienTichGieoTrongCayHangNamCoCauChuLucModel>>($"items/{_collection}?fields={Fields}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<DienTichGieoTrongCayHangNamCoCauChuLucModel> { Errors = response.Errors };
                }

                return response.Data ?? new RequestHttpResponse<DienTichGieoTrongCayHangNamCoCauChuLucModel>();
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<DienTichGieoTrongCayHangNamCoCauChuLucModel>(ex);
            }
        }

        public async Task<RequestHttpResponse<List<DienTichGieoTrongCayHangNamCoCauChuLucModel>>> CreateAsync(List<DienTichGieoTrongCayHangNamCoCauChuLucModel> model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<List<DienTichGieoTrongCayHangNamCoCauChuLucModel>>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = model.Select(c => MapToCRUDModel(c)).ToList();
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<List<DienTichGieoTrongCayHangNamCoCauChuLucModel>>>($"items/{_collection}?fields={Fields}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<List<DienTichGieoTrongCayHangNamCoCauChuLucModel>> { Errors = response.Errors };
                }

                return response.Data ?? new RequestHttpResponse<List<DienTichGieoTrongCayHangNamCoCauChuLucModel>>();
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<DienTichGieoTrongCayHangNamCoCauChuLucModel>>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(DienTichGieoTrongCayHangNamCoCauChuLucModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<DienTichGieoTrongCayHangNamCoCauChuLucCRUDModel>>($"items/{_collection}/{model.id}", updateModel);

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
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(List<DienTichGieoTrongCayHangNamCoCauChuLucModel> model)
        {
            if (model == null || model.Any(c => c.id == 0) || !model.Any())
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
                var updateModel = model.Select(c =>
                {
                    string jsonStr = JsonSerializer.Serialize(MapToCRUDModel(c));
                    JObject jObject = JObject.Parse(jsonStr);
                    dynamic dynamicObject = jObject;
                    dynamicObject.id = c.id;

                    return dynamicObject;
                }).ToList();
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<List<DienTichGieoTrongCayHangNamCoCauChuLucModel>>>($"items/{_collection}?fields={Fields}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(DienTichGieoTrongCayHangNamCoCauChuLucModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<DienTichGieoTrongCayHangNamCoCauChuLucCRUDModel>>($"items/{_collection}/{model.id}", new { deleted = true });

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(List<DienTichGieoTrongCayHangNamCoCauChuLucModel> model)
        {
            if (model == null || model.Any(c => c.id == 0) || !model.Any())
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<List<DienTichGieoTrongCayHangNamCoCauChuLucModel>>>($"items/{_collection}?fields={Fields}", model.Select(c => new { id = c.id, deleted = true }));

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
