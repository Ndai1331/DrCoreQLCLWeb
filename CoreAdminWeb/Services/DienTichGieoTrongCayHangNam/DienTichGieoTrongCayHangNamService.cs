using CoreAdminWeb.Model.DienTichGieoTrongCayHangNam;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.RequestHttp;
using CoreAdminWeb.Services.BaseServices;
using System.Net;

namespace CoreAdminWeb.Services.DienTichGieoTrongCayHangNam
{
    public class DienTichGieoTrongCayHangNamService : IBaseService<DienTichGieoTrongCayHangNamModel>
    {
        private readonly string _collection = "DienTichGieoTrongCayHangNam";
        private const string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name"
            + ",province.id,province.name"
            + ",ward.id,ward.name"
            + ",loai_hinh_canh_tac.id,loai_hinh_canh_tac.name";

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
        private static DienTichGieoTrongCayHangNamCRUDModel MapToCRUDModel(DienTichGieoTrongCayHangNamModel model)
        {
            return new()
            {
                code = model.code,
                name = model.name,
                status = model.status.ToString(),
                sort = model.sort,
                description = model.description,
                deleted = model.deleted,
                province = model.province?.id,
                ward = model.ward?.id,
                dia_diem_gieo_trong = model.dia_diem_gieo_trong,
                ngay_du_lieu = model.ngay_du_lieu,
                ke_hoach_nam = model.ke_hoach_nam,
                dien_tich_trong_moi = model.dien_tich_trong_moi,
                tong_dien_tich = model.tong_dien_tich,
                vung_sinh_thai = model.vung_sinh_thai,
                loai_hinh_canh_tac = model.loai_hinh_canh_tac?.id,
                he_thong_tuoi_tieu = model.he_thong_tuoi_tieu
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<DienTichGieoTrongCayHangNamModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<DienTichGieoTrongCayHangNamModel>>>(url);

                return response.IsSuccess
                    ? new RequestHttpResponse<List<DienTichGieoTrongCayHangNamModel>> { Data = response.Data?.Data }
                    : new RequestHttpResponse<List<DienTichGieoTrongCayHangNamModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<DienTichGieoTrongCayHangNamModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<DienTichGieoTrongCayHangNamModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<DienTichGieoTrongCayHangNamModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<DienTichGieoTrongCayHangNamModel>>($"items/{_collection}/{id}?fields={Fields}");

                return response.IsSuccess
                    ? new RequestHttpResponse<DienTichGieoTrongCayHangNamModel> { Data = response.Data?.Data }
                    : new RequestHttpResponse<DienTichGieoTrongCayHangNamModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<DienTichGieoTrongCayHangNamModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<DienTichGieoTrongCayHangNamModel>> CreateAsync(DienTichGieoTrongCayHangNamModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<DienTichGieoTrongCayHangNamModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = MapToCRUDModel(model);
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<DienTichGieoTrongCayHangNamModel>>($"items/{_collection}?fields={Fields}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<DienTichGieoTrongCayHangNamModel> { Errors = response.Errors };
                }

                return response.Data ?? new RequestHttpResponse<DienTichGieoTrongCayHangNamModel>();
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<DienTichGieoTrongCayHangNamModel>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(DienTichGieoTrongCayHangNamModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<DienTichGieoTrongCayHangNamCRUDModel>>($"items/{_collection}/{model.id}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(DienTichGieoTrongCayHangNamModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<DienTichGieoTrongCayHangNamCRUDModel>>($"items/{_collection}/{model.id}", new { deleted = true });

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
