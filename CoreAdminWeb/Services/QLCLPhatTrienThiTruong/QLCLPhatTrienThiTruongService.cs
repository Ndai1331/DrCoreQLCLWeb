using CoreAdminWeb.Model;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.RequestHttp;
using CoreAdminWeb.Services.BaseServices;
using System.Net;

namespace CoreAdminWeb.Services
{
    public class QLCLPhatTrienThiTruongService : IBaseService<QLCLPhatTrienThiTruongModel>
    {
        private readonly string _collection = "QLCLPhatTrienThiTruong";
        private const string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name"
            + ",province.id,province,province.name,ward.id,ward,ward.name"
            +",chi_tiets.id,chi_tiets.sort,chi_tiets.deleted,chi_tiets.san_pham.id,chi_tiets.san_pham.name,chi_tiets.so_luong,chi_tiets.description";

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
        private static QLCLPhatTrienThiTruongCRUDModel MapToCRUDModel(QLCLPhatTrienThiTruongModel model)
        {
            return new()
            {
                code = model.code,
                name = model.name,
                description = model.description,
                sort = model.sort,
                status = model.status.ToString(),
                province = model.province?.id,
                ward = model.ward?.id,
                dia_chi = model.dia_chi,
                ma_so_thue = model.ma_so_thue,
                so_giay_phep_dkkd = model.so_giay_phep_dkkd,
                ngay_cap = model.ngay_cap,
                co_quan_cap = model.co_quan_cap,
                hinh_thuc_ban_hang = model.hinh_thuc_ban_hang,
                thi_truong = model.thi_truong,
                quy_mo = model.quy_mo,
                doanh_thu_du_kien = model.doanh_thu_du_kien
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<QLCLPhatTrienThiTruongModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<QLCLPhatTrienThiTruongModel>>>(url);

                return response.IsSuccess
                    ? new RequestHttpResponse<List<QLCLPhatTrienThiTruongModel>> { Data = response.Data?.Data, Meta = response.Data?.Meta }
                    : new RequestHttpResponse<List<QLCLPhatTrienThiTruongModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<QLCLPhatTrienThiTruongModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<QLCLPhatTrienThiTruongModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<QLCLPhatTrienThiTruongModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<QLCLPhatTrienThiTruongModel>>($"items/{_collection}/{id}?fields={Fields}");

                return response.IsSuccess
                    ? new RequestHttpResponse<QLCLPhatTrienThiTruongModel> { Data = response.Data?.Data, Meta = response.Data?.Meta }
                    : new RequestHttpResponse<QLCLPhatTrienThiTruongModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<QLCLPhatTrienThiTruongModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<QLCLPhatTrienThiTruongModel>> CreateAsync(QLCLPhatTrienThiTruongModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<QLCLPhatTrienThiTruongModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = MapToCRUDModel(model);
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<QLCLPhatTrienThiTruongCRUDResponseModel>>($"items/{_collection}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<QLCLPhatTrienThiTruongModel> { Errors = response.Errors };
                }

                return new RequestHttpResponse<QLCLPhatTrienThiTruongModel>
                {
                    Data = new()
                    {
                        id = response.Data?.Data?.id ?? 0
                    }
                };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<QLCLPhatTrienThiTruongModel>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(QLCLPhatTrienThiTruongModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<QLCLPhatTrienThiTruongCRUDModel>>($"items/{_collection}/{model.id}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(QLCLPhatTrienThiTruongModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<QLCLPhatTrienThiTruongCRUDModel>>($"items/{_collection}/{model.id}", new { deleted = true });

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
