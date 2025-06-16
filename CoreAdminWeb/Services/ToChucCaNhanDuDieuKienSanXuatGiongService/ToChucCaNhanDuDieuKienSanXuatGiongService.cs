using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.RequestHttp;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.Model;
using System.Net;

namespace CoreAdminWeb.Services
{
    public class ToChucCaNhanDuDieuKienSanXuatGiongService : IBaseService<ToChucCaNhanDuDieuKienSanXuatGiongModel>
    {
        private readonly string _collection = "ToChucCaNhanDuDieuKienSanXuatGiong";
        private const string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name"
                + ",co_so_san_xuat_giong.id,co_so_san_xuat_giong.code,co_so_san_xuat_giong.name,co_so_san_xuat_giong.province.name,co_so_san_xuat_giong.ward.name,co_so_san_xuat_giong.ward.id";

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
        private static ToChucCaNhanDuDieuKienSanXuatGiongCRUDModel MapToCRUDModel(ToChucCaNhanDuDieuKienSanXuatGiongModel model)
        {
            return new()
            {
                status = model.status.ToString(),
                sort = model.sort,
                name = model.name,
                code = model.code,
                description = model.description,
                co_so_san_xuat_giong = model.co_so_san_xuat_giong?.id,
                tieu_chuan_cong_bo = model.tieu_chuan_cong_bo,
                cong_bo_hop_quy = model.cong_bo_hop_quy,
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<ToChucCaNhanDuDieuKienSanXuatGiongModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<ToChucCaNhanDuDieuKienSanXuatGiongModel>>>(url);

                return response.IsSuccess
                    ? new RequestHttpResponse<List<ToChucCaNhanDuDieuKienSanXuatGiongModel>> { Data = response.Data?.Data }
                    : new RequestHttpResponse<List<ToChucCaNhanDuDieuKienSanXuatGiongModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<ToChucCaNhanDuDieuKienSanXuatGiongModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<ToChucCaNhanDuDieuKienSanXuatGiongModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<ToChucCaNhanDuDieuKienSanXuatGiongModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<ToChucCaNhanDuDieuKienSanXuatGiongModel>>($"items/{_collection}/{id}?fields={Fields}");

                return response.IsSuccess
                    ? new RequestHttpResponse<ToChucCaNhanDuDieuKienSanXuatGiongModel> { Data = response.Data?.Data }
                    : new RequestHttpResponse<ToChucCaNhanDuDieuKienSanXuatGiongModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<ToChucCaNhanDuDieuKienSanXuatGiongModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<ToChucCaNhanDuDieuKienSanXuatGiongModel>> CreateAsync(ToChucCaNhanDuDieuKienSanXuatGiongModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<ToChucCaNhanDuDieuKienSanXuatGiongModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = MapToCRUDModel(model);
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<ToChucCaNhanDuDieuKienSanXuatGiongModel>>($"items/{_collection}?fields={Fields}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<ToChucCaNhanDuDieuKienSanXuatGiongModel> { Errors = response.Errors };
                }

                return response.Data ?? new RequestHttpResponse<ToChucCaNhanDuDieuKienSanXuatGiongModel>();
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<ToChucCaNhanDuDieuKienSanXuatGiongModel>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(ToChucCaNhanDuDieuKienSanXuatGiongModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<ToChucCaNhanDuDieuKienSanXuatGiongCRUDModel>>($"items/{_collection}/{model.id}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(ToChucCaNhanDuDieuKienSanXuatGiongModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<ToChucCaNhanDuDieuKienSanXuatGiongCRUDModel>>($"items/{_collection}/{model.id}", new { deleted = true });

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
