using CoreAdminWeb.Model.CoSoTrongTrotSanXuat;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.RequestHttp;
using CoreAdminWeb.Services.BaseServices;
using System.Net;

namespace CoreAdminWeb.Services.CoSoTrongTrotSanXuat
{
    public class CoSoTrongTrotSanXuatService : IBaseService<CoSoTrongTrotSanXuatModel>
    {
        private readonly string _collection = "CoSoTrongTrotSanXuat";
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
        private static CoSoTrongTrotSanXuatCRUDModel MapToCRUDModel(CoSoTrongTrotSanXuatModel model)
        {
            return new()
            {
                province = model.province?.id,
                ward = model.ward?.id,
                status = model.status.ToString(),
                sort = model.sort,
                dia_chi = model.dia_chi,
                dien_thoai = model.dien_thoai,
                email = model.email,
                so_cccd = model.so_cccd,
                nguoi_dai_dien = model.nguoi_dai_dien,
                so_giay_phep_kinh_doanh = model.so_giay_phep_kinh_doanh,
                so_gcn_du_dieu_kien = model.so_gcn_du_dieu_kien,
                ngay_cap = model.ngay_cap,
                co_quan_cap_phep = model.co_quan_cap_phep,
                dien_tich_san_xuat = model.dien_tich_san_xuat,
                nang_suat_du_kien = model.nang_suat_du_kien,
                cong_nghe_canh_tac = model.cong_nghe_canh_tac,
                code = model.code,
                name = model.name,
                description = model.description
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<CoSoTrongTrotSanXuatModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<CoSoTrongTrotSanXuatModel>>>(url);

                return response.IsSuccess
                    ? new RequestHttpResponse<List<CoSoTrongTrotSanXuatModel>> { Data = response.Data?.Data }
                    : new RequestHttpResponse<List<CoSoTrongTrotSanXuatModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<CoSoTrongTrotSanXuatModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<CoSoTrongTrotSanXuatModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<CoSoTrongTrotSanXuatModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<CoSoTrongTrotSanXuatModel>>($"items/{_collection}/{id}?fields={Fields}");

                return response.IsSuccess
                    ? new RequestHttpResponse<CoSoTrongTrotSanXuatModel> { Data = response.Data?.Data }
                    : new RequestHttpResponse<CoSoTrongTrotSanXuatModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<CoSoTrongTrotSanXuatModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<CoSoTrongTrotSanXuatModel>> CreateAsync(CoSoTrongTrotSanXuatModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<CoSoTrongTrotSanXuatModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = MapToCRUDModel(model);
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<CoSoTrongTrotSanXuatModel>>($"items/{_collection}?fields={Fields}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<CoSoTrongTrotSanXuatModel> { Errors = response.Errors };
                }

                return response.Data ?? new RequestHttpResponse<CoSoTrongTrotSanXuatModel>();
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<CoSoTrongTrotSanXuatModel>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(CoSoTrongTrotSanXuatModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<CoSoTrongTrotSanXuatCRUDModel>>($"items/{_collection}/{model.id}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(CoSoTrongTrotSanXuatModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<CoSoTrongTrotSanXuatCRUDModel>>($"items/{_collection}/{model.id}", new { deleted = true });

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
