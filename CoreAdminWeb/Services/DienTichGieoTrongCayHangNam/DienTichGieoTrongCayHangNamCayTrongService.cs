using CoreAdminWeb.Enums;
using CoreAdminWeb.Model.DienTichGieoTrongCayHangNam;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.RequestHttp;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text.Json;

namespace CoreAdminWeb.Services.DienTichGieoTrongCayHangNam
{
    public class DienTichGieoTrongCayHangNamCayTrongService : IDienTichGieoTrongCayHangNamCayTrongService
    {
        private readonly string _collection = "DienTichGieoTrongCayHangNamCayTrong";
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
        private static DienTichGieoTrongCayHangNamCayTrongCRUDModel MapToCRUDModel(DienTichGieoTrongCayHangNamCayTrongModel model)
        {
            return new()
            {
                status = model.status.ToString(),
                description = model.description,
                sort = model.sort,
                dien_tich_gieo_trong_cay_hang_nam = model.dien_tich_gieo_trong_cay_hang_nam?.id,
                cay_giong_cay_trong = model.cay_giong_cay_trong?.id,
                mua_vu = model.mua_vu,
                dien_tich_ke_hoach = model.dien_tich_ke_hoach,
                dien_tich_gieo_trong = model.dien_tich_gieo_trong,
                nang_suat = model.nang_suat,
                san_luong = model.san_luong
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<DienTichGieoTrongCayHangNamCayTrongModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<DienTichGieoTrongCayHangNamCayTrongModel>>>(url);

                return response.IsSuccess
                    ? new RequestHttpResponse<List<DienTichGieoTrongCayHangNamCayTrongModel>> { Data = response.Data?.Data }
                    : new RequestHttpResponse<List<DienTichGieoTrongCayHangNamCayTrongModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<DienTichGieoTrongCayHangNamCayTrongModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<DienTichGieoTrongCayHangNamCayTrongModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<DienTichGieoTrongCayHangNamCayTrongModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<DienTichGieoTrongCayHangNamCayTrongModel>>($"items/{_collection}/{id}?fields={Fields}");

                return response.IsSuccess
                    ? new RequestHttpResponse<DienTichGieoTrongCayHangNamCayTrongModel> { Data = response.Data?.Data }
                    : new RequestHttpResponse<DienTichGieoTrongCayHangNamCayTrongModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<DienTichGieoTrongCayHangNamCayTrongModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<DienTichGieoTrongCayHangNamCayTrongModel>> CreateAsync(DienTichGieoTrongCayHangNamCayTrongModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<DienTichGieoTrongCayHangNamCayTrongModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = MapToCRUDModel(model);
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<DienTichGieoTrongCayHangNamCayTrongModel>>($"items/{_collection}?fields={Fields}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<DienTichGieoTrongCayHangNamCayTrongModel> { Errors = response.Errors };
                }

                return response.Data ?? new RequestHttpResponse<DienTichGieoTrongCayHangNamCayTrongModel>();
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<DienTichGieoTrongCayHangNamCayTrongModel>(ex);
            }
        }

        public async Task<RequestHttpResponse<List<DienTichGieoTrongCayHangNamCayTrongModel>>> CreateAsync(List<DienTichGieoTrongCayHangNamCayTrongModel> model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<List<DienTichGieoTrongCayHangNamCayTrongModel>>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = model.Select(c => MapToCRUDModel(c)).ToList();
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<List<DienTichGieoTrongCayHangNamCayTrongModel>>>($"items/{_collection}?fields={Fields}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<List<DienTichGieoTrongCayHangNamCayTrongModel>> { Errors = response.Errors };
                }

                return response.Data ?? new RequestHttpResponse<List<DienTichGieoTrongCayHangNamCayTrongModel>>();
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<DienTichGieoTrongCayHangNamCayTrongModel>>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(DienTichGieoTrongCayHangNamCayTrongModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<DienTichGieoTrongCayHangNamCayTrongCRUDModel>>($"items/{_collection}/{model.id}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> UpdateAsync(List<DienTichGieoTrongCayHangNamCayTrongModel> model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<List<DienTichGieoTrongCayHangNamCayTrongModel>>>($"items/{_collection}?fields={Fields}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(DienTichGieoTrongCayHangNamCayTrongModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<DienTichGieoTrongCayHangNamCayTrongCRUDModel>>($"items/{_collection}/{model.id}", new { deleted = true });

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(List<DienTichGieoTrongCayHangNamCayTrongModel> model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<List<DienTichGieoTrongCayHangNamCayTrongModel>>>($"items/{_collection}?fields={Fields}", model.Select(c => new { id = c.id, deleted = true }));

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
