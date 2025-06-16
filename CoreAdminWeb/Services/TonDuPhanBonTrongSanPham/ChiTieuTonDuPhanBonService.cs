using CoreAdminWeb.Enums;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.Model.TonDuPhanBonTrongSanPham;
using CoreAdminWeb.RequestHttp;
using Newtonsoft.Json.Linq;
using System.Dynamic;
using System.Net;
using System.Text.Json;

namespace CoreAdminWeb.Services.TonDuPhanBonTrongSanPham
{
    public class ChiTieuTonDuPhanBonService : IChiTieuTonDuPhanBonService
    {
        private readonly string _collection = "ChiTieuTonDuPhanBon";
        private const string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name"
            + ",ton_du_phan_bon_trong_san_pham.id";

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
        private static ChiTieuTonDuPhanBonCRUDModel MapToCRUDModel(ChiTieuTonDuPhanBonModel model)
        {
            return new()
            {
                chi_tieu_ton_du = model.chi_tieu_ton_du,
                ham_luong_ket_qua = model.ham_luong_ket_qua,
                gioi_han_cho_phep = model.gioi_han_cho_phep,
                ton_du_phan_bon_trong_san_pham = model.ton_du_phan_bon_trong_san_pham?.id,
                status = model.status.ToString(),
                description = model.description,
                sort = model.sort
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<ChiTieuTonDuPhanBonModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<ChiTieuTonDuPhanBonModel>>>(url);

                return response.IsSuccess
                    ? new RequestHttpResponse<List<ChiTieuTonDuPhanBonModel>> { Data = response.Data?.Data }
                    : new RequestHttpResponse<List<ChiTieuTonDuPhanBonModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<ChiTieuTonDuPhanBonModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<ChiTieuTonDuPhanBonModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<ChiTieuTonDuPhanBonModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<ChiTieuTonDuPhanBonModel>>($"items/{_collection}/{id}?fields={Fields}");

                return response.IsSuccess
                    ? new RequestHttpResponse<ChiTieuTonDuPhanBonModel> { Data = response.Data?.Data }
                    : new RequestHttpResponse<ChiTieuTonDuPhanBonModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<ChiTieuTonDuPhanBonModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<ChiTieuTonDuPhanBonModel>> CreateAsync(ChiTieuTonDuPhanBonModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<ChiTieuTonDuPhanBonModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = MapToCRUDModel(model);
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<ChiTieuTonDuPhanBonModel>>($"items/{_collection}?fields={Fields}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<ChiTieuTonDuPhanBonModel> { Errors = response.Errors };
                }

                return response.Data ?? new RequestHttpResponse<ChiTieuTonDuPhanBonModel>();
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<ChiTieuTonDuPhanBonModel>(ex);
            }
        }

        public async Task<RequestHttpResponse<List<ChiTieuTonDuPhanBonModel>>> CreateAsync(List<ChiTieuTonDuPhanBonModel> model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<List<ChiTieuTonDuPhanBonModel>>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = model.Select(c => MapToCRUDModel(c)).ToList();
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<List<ChiTieuTonDuPhanBonModel>>>($"items/{_collection}?fields={Fields}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<List<ChiTieuTonDuPhanBonModel>> { Errors = response.Errors };
                }

                return response.Data ?? new RequestHttpResponse<List<ChiTieuTonDuPhanBonModel>>();
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<ChiTieuTonDuPhanBonModel>>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(ChiTieuTonDuPhanBonModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<ChiTieuTonDuPhanBonCRUDModel>>($"items/{_collection}/{model.id}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> UpdateAsync(List<ChiTieuTonDuPhanBonModel> model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<List<ChiTieuTonDuPhanBonModel>>>($"items/{_collection}?fields={Fields}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(ChiTieuTonDuPhanBonModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<ChiTieuTonDuPhanBonCRUDModel>>($"items/{_collection}/{model.id}", new { deleted = true, status = TrangThaiBanGhiChiTiet.Removed.ToString() });

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(List<ChiTieuTonDuPhanBonModel> model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<List<ChiTieuTonDuPhanBonModel>>>($"items/{_collection}?fields={Fields}", model.Select(c => new { id = c.id, deleted = true, status = TrangThaiBanGhiChiTiet.Removed.ToString() }));

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
