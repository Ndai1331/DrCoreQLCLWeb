using CoreAdminWeb.Enums;
using CoreAdminWeb.Model.DienTichGieoTrongCayLauNam;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.RequestHttp;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text.Json;

namespace CoreAdminWeb.Services.DienTichGieoTrongCayLauNam
{
    public class DienTichGieoTrongCayLauNamCayTrongService : IDienTichGieoTrongCayLauNamCayTrongService
    {
        private readonly string _collection = "DienTichGieoTrongCayLauNamCayTrong";
        private const string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name"
            + ",dien_tich_gieo_trong_cay_lau_nam.id"
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
        private static DienTichGieoTrongCayLauNamCayTrongCRUDModel MapToCRUDModel(DienTichGieoTrongCayLauNamCayTrongModel model)
        {
            return new()
            {
                status = model.status.ToString(),
                description = model.description,
                sort = model.sort,
                dien_tich_gieo_trong_cay_lau_nam = model.dien_tich_gieo_trong_cay_lau_nam?.id,
                cay_giong_cay_trong = model.cay_giong_cay_trong?.id,
                dien_tich_ke_hoach = model.dien_tich_ke_hoach,
                dien_tich_gieo_trong = model.dien_tich_gieo_trong,
                nang_suat = model.nang_suat,
                san_luong = model.san_luong
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<DienTichGieoTrongCayLauNamCayTrongModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<DienTichGieoTrongCayLauNamCayTrongModel>>>(url);

                return response.IsSuccess
                    ? new RequestHttpResponse<List<DienTichGieoTrongCayLauNamCayTrongModel>> { Data = response.Data?.Data }
                    : new RequestHttpResponse<List<DienTichGieoTrongCayLauNamCayTrongModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<DienTichGieoTrongCayLauNamCayTrongModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<DienTichGieoTrongCayLauNamCayTrongModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<DienTichGieoTrongCayLauNamCayTrongModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<DienTichGieoTrongCayLauNamCayTrongModel>>($"items/{_collection}/{id}?fields={Fields}");

                return response.IsSuccess
                    ? new RequestHttpResponse<DienTichGieoTrongCayLauNamCayTrongModel> { Data = response.Data?.Data }
                    : new RequestHttpResponse<DienTichGieoTrongCayLauNamCayTrongModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<DienTichGieoTrongCayLauNamCayTrongModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<DienTichGieoTrongCayLauNamCayTrongModel>> CreateAsync(DienTichGieoTrongCayLauNamCayTrongModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<DienTichGieoTrongCayLauNamCayTrongModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = MapToCRUDModel(model);
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<DienTichGieoTrongCayLauNamCayTrongModel>>($"items/{_collection}?fields={Fields}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<DienTichGieoTrongCayLauNamCayTrongModel> { Errors = response.Errors };
                }

                return response.Data ?? new RequestHttpResponse<DienTichGieoTrongCayLauNamCayTrongModel>();
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<DienTichGieoTrongCayLauNamCayTrongModel>(ex);
            }
        }

        public async Task<RequestHttpResponse<List<DienTichGieoTrongCayLauNamCayTrongModel>>> CreateAsync(List<DienTichGieoTrongCayLauNamCayTrongModel> model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<List<DienTichGieoTrongCayLauNamCayTrongModel>>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = model.Select(c => MapToCRUDModel(c)).ToList();
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<List<DienTichGieoTrongCayLauNamCayTrongModel>>>($"items/{_collection}?fields={Fields}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<List<DienTichGieoTrongCayLauNamCayTrongModel>> { Errors = response.Errors };
                }

                return response.Data ?? new RequestHttpResponse<List<DienTichGieoTrongCayLauNamCayTrongModel>>();
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<DienTichGieoTrongCayLauNamCayTrongModel>>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(DienTichGieoTrongCayLauNamCayTrongModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<DienTichGieoTrongCayLauNamCayTrongCRUDModel>>($"items/{_collection}/{model.id}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> UpdateAsync(List<DienTichGieoTrongCayLauNamCayTrongModel> model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<List<DienTichGieoTrongCayLauNamCayTrongModel>>>($"items/{_collection}?fields={Fields}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(DienTichGieoTrongCayLauNamCayTrongModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<DienTichGieoTrongCayLauNamCayTrongCRUDModel>>($"items/{_collection}/{model.id}", new { deleted = true });

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(List<DienTichGieoTrongCayLauNamCayTrongModel> model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<List<DienTichGieoTrongCayLauNamCayTrongModel>>>($"items/{_collection}?fields={Fields}", model.Select(c => new { id = c.id, deleted = true }));

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
