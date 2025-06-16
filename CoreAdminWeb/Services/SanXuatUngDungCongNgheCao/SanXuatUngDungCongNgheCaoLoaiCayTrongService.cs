using CoreAdminWeb.Enums;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.Model.SanXuatUngDungCongNgheCao;
using CoreAdminWeb.RequestHttp;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text.Json;

namespace CoreAdminWeb.Services.SanXuatUngDungCongNgheCao
{
    public class SanXuatUngDungCongNgheCaoLoaiCayTrongService : ISanXuatUngDungCongNgheCaoLoaiCayTrongService
    {
        private readonly string _collection = "SanXuatUngDungCongNgheCaoLoaiCayTrong";
        private const string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name"
            + ",san_xuat_ung_dung_cong_nghe_cao.id"
            + ",loai_cay_trong.id,loai_cay_trong.name";

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
        private static SanXuatUngDungCongNgheCaoLoaiCayTrongCRUDModel MapToCRUDModel(SanXuatUngDungCongNgheCaoLoaiCayTrongModel model)
        {
            return new()
            {
                status = model.status.ToString(),
                description = model.description,
                sort = model.sort,
                san_xuat_ung_dung_cong_nghe_cao = model.san_xuat_ung_dung_cong_nghe_cao?.id,
                loai_cay_trong = model.loai_cay_trong?.id,
                dien_tich = model.dien_tich,
                san_luong = model.san_luong
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<SanXuatUngDungCongNgheCaoLoaiCayTrongModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<SanXuatUngDungCongNgheCaoLoaiCayTrongModel>>>(url);

                return response.IsSuccess
                    ? new RequestHttpResponse<List<SanXuatUngDungCongNgheCaoLoaiCayTrongModel>> { Data = response.Data?.Data }
                    : new RequestHttpResponse<List<SanXuatUngDungCongNgheCaoLoaiCayTrongModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<SanXuatUngDungCongNgheCaoLoaiCayTrongModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<SanXuatUngDungCongNgheCaoLoaiCayTrongModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<SanXuatUngDungCongNgheCaoLoaiCayTrongModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<SanXuatUngDungCongNgheCaoLoaiCayTrongModel>>($"items/{_collection}/{id}?fields={Fields}");

                return response.IsSuccess
                    ? new RequestHttpResponse<SanXuatUngDungCongNgheCaoLoaiCayTrongModel> { Data = response.Data?.Data }
                    : new RequestHttpResponse<SanXuatUngDungCongNgheCaoLoaiCayTrongModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<SanXuatUngDungCongNgheCaoLoaiCayTrongModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<SanXuatUngDungCongNgheCaoLoaiCayTrongModel>> CreateAsync(SanXuatUngDungCongNgheCaoLoaiCayTrongModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<SanXuatUngDungCongNgheCaoLoaiCayTrongModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = MapToCRUDModel(model);
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<SanXuatUngDungCongNgheCaoLoaiCayTrongModel>>($"items/{_collection}?fields={Fields}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<SanXuatUngDungCongNgheCaoLoaiCayTrongModel> { Errors = response.Errors };
                }

                return response.Data ?? new RequestHttpResponse<SanXuatUngDungCongNgheCaoLoaiCayTrongModel>();
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<SanXuatUngDungCongNgheCaoLoaiCayTrongModel>(ex);
            }
        }

        public async Task<RequestHttpResponse<List<SanXuatUngDungCongNgheCaoLoaiCayTrongModel>>> CreateAsync(List<SanXuatUngDungCongNgheCaoLoaiCayTrongModel> model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<List<SanXuatUngDungCongNgheCaoLoaiCayTrongModel>>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = model.Select(c => MapToCRUDModel(c)).ToList();
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<List<SanXuatUngDungCongNgheCaoLoaiCayTrongModel>>>($"items/{_collection}?fields={Fields}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<List<SanXuatUngDungCongNgheCaoLoaiCayTrongModel>> { Errors = response.Errors };
                }

                return response.Data ?? new RequestHttpResponse<List<SanXuatUngDungCongNgheCaoLoaiCayTrongModel>>();
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<SanXuatUngDungCongNgheCaoLoaiCayTrongModel>>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(SanXuatUngDungCongNgheCaoLoaiCayTrongModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<SanXuatUngDungCongNgheCaoLoaiCayTrongCRUDModel>>($"items/{_collection}/{model.id}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> UpdateAsync(List<SanXuatUngDungCongNgheCaoLoaiCayTrongModel> model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<List<SanXuatUngDungCongNgheCaoLoaiCayTrongModel>>>($"items/{_collection}?fields={Fields}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(SanXuatUngDungCongNgheCaoLoaiCayTrongModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<SanXuatUngDungCongNgheCaoLoaiCayTrongCRUDModel>>($"items/{_collection}/{model.id}", new { deleted = true });

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(List<SanXuatUngDungCongNgheCaoLoaiCayTrongModel> model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<List<SanXuatUngDungCongNgheCaoLoaiCayTrongModel>>>($"items/{_collection}?fields={Fields}", model.Select(c => new { id = c.id, deleted = true }));

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
