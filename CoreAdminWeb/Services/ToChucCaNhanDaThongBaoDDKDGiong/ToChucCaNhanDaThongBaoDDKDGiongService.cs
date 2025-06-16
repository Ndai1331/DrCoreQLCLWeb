using CoreAdminWeb.Model;
using CoreAdminWeb.Model.ToChucCaNhanDaThongBaoDDKDGiong;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.RequestHttp;
using CoreAdminWeb.Services.BaseServices;
using System.Net;

namespace CoreAdminWeb.Services.ToChucCaNhanDaThongBaoDDKDGiong
{
    public class ToChucCaNhanDaThongBaoDDKDGiongService : IBaseService<ToChucCaNhanDaThongBaoDDKDGiongModel>
    {
        private readonly string _collection = "ToChucCaNhanDaThongBaoDDKDGiong";
        private const string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name"
            + ",co_so_san_xuat_giong.id,co_so_san_xuat_giong.code,co_so_san_xuat_giong.name"
            + ",co_so_san_xuat_giong.dia_chi,co_so_san_xuat_giong.province.name,co_so_san_xuat_giong.ward.name";

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
        private static ToChucCaNhanDaThongBaoDDKDGiongCRUDModel MapToCRUDModel(ToChucCaNhanDaThongBaoDDKDGiongModel model)
        {
            return new()
            {
                co_so_san_xuat_giong = model.co_so_san_xuat_giong?.id,
                so_giay_phep_kinh_doanh = model.so_giay_phep_kinh_doanh
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<ToChucCaNhanDaThongBaoDDKDGiongModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<ToChucCaNhanDaThongBaoDDKDGiongModel>>>(url);

                return response.IsSuccess
                    ? new RequestHttpResponse<List<ToChucCaNhanDaThongBaoDDKDGiongModel>> { Data = response.Data?.Data }
                    : new RequestHttpResponse<List<ToChucCaNhanDaThongBaoDDKDGiongModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<ToChucCaNhanDaThongBaoDDKDGiongModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<ToChucCaNhanDaThongBaoDDKDGiongModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<ToChucCaNhanDaThongBaoDDKDGiongModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<ToChucCaNhanDaThongBaoDDKDGiongModel>>($"items/{_collection}/{id}?fields={Fields}");

                return response.IsSuccess
                    ? new RequestHttpResponse<ToChucCaNhanDaThongBaoDDKDGiongModel> { Data = response.Data?.Data }
                    : new RequestHttpResponse<ToChucCaNhanDaThongBaoDDKDGiongModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<ToChucCaNhanDaThongBaoDDKDGiongModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<ToChucCaNhanDaThongBaoDDKDGiongModel>> CreateAsync(ToChucCaNhanDaThongBaoDDKDGiongModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<ToChucCaNhanDaThongBaoDDKDGiongModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = MapToCRUDModel(model);
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<ToChucCaNhanDaThongBaoDDKDGiongCRUDModel>>($"items/{_collection}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<ToChucCaNhanDaThongBaoDDKDGiongModel> { Errors = response.Errors };
                }

                return new RequestHttpResponse<ToChucCaNhanDaThongBaoDDKDGiongModel>
                {
                    Data = new()
                    {
                        code = response.Data?.Data?.code,
                        name = response.Data?.Data?.name
                    }
                };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<ToChucCaNhanDaThongBaoDDKDGiongModel>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(ToChucCaNhanDaThongBaoDDKDGiongModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<ToChucCaNhanDaThongBaoDDKDGiongCRUDModel>>($"items/{_collection}/{model.id}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(ToChucCaNhanDaThongBaoDDKDGiongModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<ToChucCaNhanDaThongBaoDDKDGiongCRUDModel>>($"items/{_collection}/{model.id}", new { deleted = true });

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
