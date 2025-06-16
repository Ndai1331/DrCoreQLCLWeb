using CoreAdminWeb.Model.CoSoBiDichBenh;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.RequestHttp;
using CoreAdminWeb.Services.BaseServices;
using System.Net;

namespace CoreAdminWeb.Services.CoSoBiDichBenh
{
    public class CoSoBiDichBenhService : IBaseService<CoSoBiDichBenhModel>
    {
        private readonly string _collection = "CoSoBiDichBenh";
        private const string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name"
            + ",province.id,province.name"
            + ",ward.id,ward.name"
            + ",co_so_trong_trot_san_xuat.id,co_so_trong_trot_san_xuat.name"
            + ",cay_trong.id,cay_trong.name"
            + ",chi_tiet_dich_benh.vi_sinh_vat_gay_hai.name,chi_tiet_dich_benh.dien_tich,chi_tiet_dich_benh.deleted";

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
        private static CoSoBiDichBenhCRUDModel MapToCRUDModel(CoSoBiDichBenhModel model)
        {
            return new()
            {
                province = model.province?.id,
                ward = model.ward?.id,
                status = model.status.ToString(),
                co_so_trong_trot_san_xuat = model.co_so_trong_trot_san_xuat?.id,
                dia_diem_san_xuat = model.dia_diem_san_xuat,
                thoi_gian_bi_benh_tu = model.thoi_gian_bi_benh_tu,
                thoi_gian_bi_benh_den = model.thoi_gian_bi_benh_den,
                mua_vu = model.mua_vu,
                nguyen_nhan = model.nguyen_nhan,
                bien_phap_phong_tru = model.bien_phap_phong_tru,
                muc_do_thiet_hai = model.muc_do_thiet_hai,
                ho_tro_tu_co_quan_quan_ly = model.ho_tro_tu_co_quan_quan_ly,
                cay_trong = model.cay_trong?.id,
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<CoSoBiDichBenhModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<CoSoBiDichBenhModel>>>(url);

                return response.IsSuccess
                    ? new RequestHttpResponse<List<CoSoBiDichBenhModel>> { Data = response.Data?.Data }
                    : new RequestHttpResponse<List<CoSoBiDichBenhModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<CoSoBiDichBenhModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<CoSoBiDichBenhModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<CoSoBiDichBenhModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<CoSoBiDichBenhModel>>($"items/{_collection}/{id}?fields={Fields}");

                return response.IsSuccess
                    ? new RequestHttpResponse<CoSoBiDichBenhModel> { Data = response.Data?.Data }
                    : new RequestHttpResponse<CoSoBiDichBenhModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<CoSoBiDichBenhModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<CoSoBiDichBenhModel>> CreateAsync(CoSoBiDichBenhModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<CoSoBiDichBenhModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = MapToCRUDModel(model);
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<CoSoBiDichBenhModel>>($"items/{_collection}?fields={Fields}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<CoSoBiDichBenhModel> { Errors = response.Errors };
                }

                return response.Data ?? new RequestHttpResponse<CoSoBiDichBenhModel>();
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<CoSoBiDichBenhModel>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(CoSoBiDichBenhModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<CoSoBiDichBenhCRUDModel>>($"items/{_collection}/{model.id}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(CoSoBiDichBenhModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<CoSoBiDichBenhCRUDModel>>($"items/{_collection}/{model.id}", new { deleted = true });

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
