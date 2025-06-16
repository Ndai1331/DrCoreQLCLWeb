using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.Model.TonDuThuocBVTVTrongSanPham;
using CoreAdminWeb.RequestHttp;
using CoreAdminWeb.Services.BaseServices;
using System.Net;

namespace CoreAdminWeb.Services.TonDuThuocBVTVTrongSanPham
{
    public class TonDuThuocBVTVTrongSanPhamService : IBaseService<TonDuThuocBVTVTrongSanPhamModel>
    {
        private readonly string _collection = "TonDuThuocBVTVTrongSanPham";
        private const string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name"
            + ",province.id,province.name"
            + ",ward.id,ward.name";

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
        private static TonDuThuocBVTVTrongSanPhamCRUDModel MapToCRUDModel(TonDuThuocBVTVTrongSanPhamModel model)
        {
            return new()
            {
                bien_phap_xu_ly = model.bien_phap_xu_ly,
                dia_diem_lay_mau = model.dia_diem_lay_mau,
                don_vi_kiem_dinh = model.don_vi_kiem_dinh,
                ket_qua_phan_tich = model.ket_qua_phan_tich,
                ma_co_so = model.ma_co_so,
                ten_co_so = model.ten_co_so,
                ngay_lay_mau = model.ngay_lay_mau,
                phuong_phap_lay_mau = model.phuong_phap_lay_mau,
                province = model.province?.id,
                ward = model.ward?.id,
                so_luong_mau = model.so_luong_mau,
                ten_mau_kiem_dinh = model.ten_mau_kiem_dinh,
                status = model.status.ToString(),
                sort = model.sort
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<TonDuThuocBVTVTrongSanPhamModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<TonDuThuocBVTVTrongSanPhamModel>>>(url);

                return response.IsSuccess
                    ? new RequestHttpResponse<List<TonDuThuocBVTVTrongSanPhamModel>> { Data = response.Data?.Data }
                    : new RequestHttpResponse<List<TonDuThuocBVTVTrongSanPhamModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<TonDuThuocBVTVTrongSanPhamModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<TonDuThuocBVTVTrongSanPhamModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<TonDuThuocBVTVTrongSanPhamModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<TonDuThuocBVTVTrongSanPhamModel>>($"items/{_collection}/{id}?fields={Fields}");

                return response.IsSuccess
                    ? new RequestHttpResponse<TonDuThuocBVTVTrongSanPhamModel> { Data = response.Data?.Data }
                    : new RequestHttpResponse<TonDuThuocBVTVTrongSanPhamModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<TonDuThuocBVTVTrongSanPhamModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<TonDuThuocBVTVTrongSanPhamModel>> CreateAsync(TonDuThuocBVTVTrongSanPhamModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<TonDuThuocBVTVTrongSanPhamModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = MapToCRUDModel(model);
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<TonDuThuocBVTVTrongSanPhamModel>>($"items/{_collection}?fields={Fields}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<TonDuThuocBVTVTrongSanPhamModel> { Errors = response.Errors };
                }

                return response.Data ?? new RequestHttpResponse<TonDuThuocBVTVTrongSanPhamModel>();
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<TonDuThuocBVTVTrongSanPhamModel>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(TonDuThuocBVTVTrongSanPhamModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<TonDuThuocBVTVTrongSanPhamCRUDModel>>($"items/{_collection}/{model.id}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(TonDuThuocBVTVTrongSanPhamModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<TonDuThuocBVTVTrongSanPhamCRUDModel>>($"items/{_collection}/{model.id}", new { deleted = true });

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
