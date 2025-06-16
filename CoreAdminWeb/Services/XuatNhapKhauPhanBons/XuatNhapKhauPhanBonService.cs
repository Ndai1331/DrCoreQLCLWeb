using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.Model.XuatNhapKhauPhanBon;
using CoreAdminWeb.RequestHttp;
using CoreAdminWeb.Services.BaseServices;
using System.Net;

namespace CoreAdminWeb.Services.XuatNhapKhauPhanBons
{
    public class XuatNhapKhauPhanBonService : IBaseService<XuatNhapKhauPhanBonModel>
    {
        private readonly string _collection = "XNKPhanBon";
        private const string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name"
            + ",co_so_du_dieu_kien_buon_ban_phan_bon.id,co_so_du_dieu_kien_buon_ban_phan_bon.name"
            + ",co_so_san_xuat_phan_bon.id,co_so_san_xuat_phan_bon.name";

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
        private static XuatNhapKhauPhanBonCRUDModel MapToCRUDModel(XuatNhapKhauPhanBonModel model)
        {
            return new()
            {
                so_chung_tu = model.so_chung_tu,
                description = model.description,
                co_quan_cap = model.co_quan_cap,
                co_so_du_dieu_kien_buon_ban_phan_bon = model.co_so_du_dieu_kien_buon_ban_phan_bon?.id,
                co_so_san_xuat_phan_bon = model.co_so_san_xuat_phan_bon?.id,
                giay_phep_xnk = model.giay_phep_xnk,
                hinh_thuc = model.hinh_thuc,
                ngay_cap = model.ngay_cap,
                ngay_chung_tu = model.ngay_chung_tu,
                status = model.status.ToString(),
                sort = model.sort
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<XuatNhapKhauPhanBonModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<XuatNhapKhauPhanBonModel>>>(url);

                return response.IsSuccess
                    ? new RequestHttpResponse<List<XuatNhapKhauPhanBonModel>> { Data = response.Data?.Data }
                    : new RequestHttpResponse<List<XuatNhapKhauPhanBonModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<XuatNhapKhauPhanBonModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<XuatNhapKhauPhanBonModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<XuatNhapKhauPhanBonModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<XuatNhapKhauPhanBonModel>>($"items/{_collection}/{id}?fields={Fields}");

                return response.IsSuccess
                    ? new RequestHttpResponse<XuatNhapKhauPhanBonModel> { Data = response.Data?.Data }
                    : new RequestHttpResponse<XuatNhapKhauPhanBonModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<XuatNhapKhauPhanBonModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<XuatNhapKhauPhanBonModel>> CreateAsync(XuatNhapKhauPhanBonModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<XuatNhapKhauPhanBonModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = MapToCRUDModel(model);
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<XuatNhapKhauPhanBonModel>>($"items/{_collection}?fields={Fields}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<XuatNhapKhauPhanBonModel> { Errors = response.Errors };
                }

                return response.Data ?? new RequestHttpResponse<XuatNhapKhauPhanBonModel>();
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<XuatNhapKhauPhanBonModel>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(XuatNhapKhauPhanBonModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<XuatNhapKhauPhanBonCRUDModel>>($"items/{_collection}/{model.id}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(XuatNhapKhauPhanBonModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<XuatNhapKhauPhanBonCRUDModel>>($"items/{_collection}/{model.id}", new { deleted = true });

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
