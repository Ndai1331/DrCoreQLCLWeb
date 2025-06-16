using CoreAdminWeb.Enums;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.Model.TonDuThuocBVTVTrongSanPham;
using CoreAdminWeb.RequestHttp;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text.Json;

namespace CoreAdminWeb.Services.TonDuThuocBVTVTrongSanPham
{
    public class ChiTieuTonDuThuocBVTVService : IChiTieuTonDuThuocBVTVService
    {
        private readonly string _collection = "ChiTieuTonDuThuocBVTV";
        private const string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name"
            + ",ton_du_thuoc_bvtv_trong_san_pham.id";

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
        private static ChiTieuTonDuThuocBVTVCRUDModel MapToCRUDModel(ChiTieuTonDuThuocBVTVModel model)
        {
            return new()
            {
                chi_tieu_ton_du = model.chi_tieu_ton_du,
                ham_luong_ket_qua = model.ham_luong_ket_qua,
                gioi_han_cho_phep = model.gioi_han_cho_phep,
                ton_du_thuoc_bvtv_trong_san_pham = model.ton_du_thuoc_bvtv_trong_san_pham?.id,
                status = model.status.ToString(),
                description = model.description,
                sort = model.sort
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<ChiTieuTonDuThuocBVTVModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<ChiTieuTonDuThuocBVTVModel>>>(url);

                return response.IsSuccess
                    ? new RequestHttpResponse<List<ChiTieuTonDuThuocBVTVModel>> { Data = response.Data?.Data }
                    : new RequestHttpResponse<List<ChiTieuTonDuThuocBVTVModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<ChiTieuTonDuThuocBVTVModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<ChiTieuTonDuThuocBVTVModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<ChiTieuTonDuThuocBVTVModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<ChiTieuTonDuThuocBVTVModel>>($"items/{_collection}/{id}?fields={Fields}");

                return response.IsSuccess
                    ? new RequestHttpResponse<ChiTieuTonDuThuocBVTVModel> { Data = response.Data?.Data }
                    : new RequestHttpResponse<ChiTieuTonDuThuocBVTVModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<ChiTieuTonDuThuocBVTVModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<ChiTieuTonDuThuocBVTVModel>> CreateAsync(ChiTieuTonDuThuocBVTVModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<ChiTieuTonDuThuocBVTVModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = MapToCRUDModel(model);
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<ChiTieuTonDuThuocBVTVModel>>($"items/{_collection}?fields={Fields}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<ChiTieuTonDuThuocBVTVModel> { Errors = response.Errors };
                }

                return response.Data ?? new RequestHttpResponse<ChiTieuTonDuThuocBVTVModel>();
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<ChiTieuTonDuThuocBVTVModel>(ex);
            }
        }

        public async Task<RequestHttpResponse<List<ChiTieuTonDuThuocBVTVModel>>> CreateAsync(List<ChiTieuTonDuThuocBVTVModel> model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<List<ChiTieuTonDuThuocBVTVModel>>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = model.Select(c => MapToCRUDModel(c)).ToList();
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<List<ChiTieuTonDuThuocBVTVModel>>>($"items/{_collection}?fields={Fields}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<List<ChiTieuTonDuThuocBVTVModel>> { Errors = response.Errors };
                }

                return response.Data ?? new RequestHttpResponse<List<ChiTieuTonDuThuocBVTVModel>>();
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<ChiTieuTonDuThuocBVTVModel>>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(ChiTieuTonDuThuocBVTVModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<ChiTieuTonDuThuocBVTVCRUDModel>>($"items/{_collection}/{model.id}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> UpdateAsync(List<ChiTieuTonDuThuocBVTVModel> model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<List<ChiTieuTonDuThuocBVTVModel>>>($"items/{_collection}?fields={Fields}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(ChiTieuTonDuThuocBVTVModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<ChiTieuTonDuThuocBVTVCRUDModel>>($"items/{_collection}/{model.id}", new { deleted = true, status = TrangThaiBanGhiChiTiet.Removed.ToString() });

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(List<ChiTieuTonDuThuocBVTVModel> model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<List<ChiTieuTonDuThuocBVTVModel>>>($"items/{_collection}?fields={Fields}", model.Select(c => new { id = c.id, deleted = true, status = TrangThaiBanGhiChiTiet.Removed.ToString() }));

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
