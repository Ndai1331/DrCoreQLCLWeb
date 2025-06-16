using CoreAdminWeb.Model;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.RequestHttp;
using CoreAdminWeb.Services.BaseServices;
using System.Net;

namespace CoreAdminWeb.Services
{
    public class ThiTruongXuatKhauService : IBaseService<ThiTruongXuatKhauModel>
    {
        private readonly string _collection = "ThiTruongXuatKhau";
        private const string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name"
            + ",country.id,country.code,country.name,san_pham_san_xuat.id,san_pham_san_xuat.code,san_pham_san_xuat.name";

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
        private static ThiTruongXuatKhauCRUDModel MapToCRUDModel(ThiTruongXuatKhauModel model)
        {
            return new()
            {
                code = model.code,
                name = model.name,
                description = model.description,
                sort = model.sort,
                status = model.status.ToString(),
                country = model.country?.id,
                ngay_ghi_nhan = model.ngay_ghi_nhan,
                khu_vuc_thi_truong = model.khu_vuc_thi_truong,
                doanh_nghiep_xuat_khau = model.doanh_nghiep_xuat_khau,
                ma_don_vi = model.ma_don_vi,
                so_giay_phep_xuat_khau = model.so_giay_phep_xuat_khau,
                dia_chi = model.dia_chi,
                san_pham_san_xuat = model.san_pham_san_xuat?.id,
                san_luong_xuat_khau = model.san_luong_xuat_khau,
                gia_tri = model.gia_tri,
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<ThiTruongXuatKhauModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<ThiTruongXuatKhauModel>>>(url);

                return response.IsSuccess
                    ? new RequestHttpResponse<List<ThiTruongXuatKhauModel>> { Data = response.Data?.Data }
                    : new RequestHttpResponse<List<ThiTruongXuatKhauModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<ThiTruongXuatKhauModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<ThiTruongXuatKhauModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<ThiTruongXuatKhauModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<ThiTruongXuatKhauModel>>($"items/{_collection}/{id}?fields={Fields}");

                return response.IsSuccess
                    ? new RequestHttpResponse<ThiTruongXuatKhauModel> { Data = response.Data?.Data }
                    : new RequestHttpResponse<ThiTruongXuatKhauModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<ThiTruongXuatKhauModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<ThiTruongXuatKhauModel>> CreateAsync(ThiTruongXuatKhauModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<ThiTruongXuatKhauModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = MapToCRUDModel(model);
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<ThiTruongXuatKhauCRUDModel>>($"items/{_collection}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<ThiTruongXuatKhauModel> { Errors = response.Errors };
                }

                return new RequestHttpResponse<ThiTruongXuatKhauModel>
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
                return CreateErrorResponse<ThiTruongXuatKhauModel>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(ThiTruongXuatKhauModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<ThiTruongXuatKhauCRUDModel>>($"items/{_collection}/{model.id}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(ThiTruongXuatKhauModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<ThiTruongXuatKhauCRUDModel>>($"items/{_collection}/{model.id}", new { deleted = true });

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
