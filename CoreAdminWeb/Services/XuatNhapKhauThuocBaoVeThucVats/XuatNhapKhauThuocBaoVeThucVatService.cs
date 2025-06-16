using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.Model.XuatNhapKhauThuocBaoVeThucVat;
using CoreAdminWeb.RequestHttp;
using CoreAdminWeb.Services.BaseServices;
using System.Net;

namespace CoreAdminWeb.Services.XuatNhapKhauThuocBaoVeThucVats
{
    public class XuatNhapKhauThuocBaoVeThucVatService : IBaseService<XuatNhapKhauThuocBVTVModel>
    {
        private readonly string _collection = "XNKThuocBVTV";
        private const string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name"
            + ",co_so_kinh_doanh_thuoc_bvtv.id,co_so_kinh_doanh_thuoc_bvtv.name"
            + ",co_so_san_xuat_thuoc_bvtv.id,co_so_san_xuat_thuoc_bvtv.name";

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
        private static XuatNhapKhauThuocBVTVCRUDModel MapToCRUDModel(XuatNhapKhauThuocBVTVModel model)
        {
            return new()
            {
                so_chung_tu = model.so_chung_tu,
                description = model.description,
                co_quan_cap = model.co_quan_cap,
                co_so_san_xuat_thuoc_bvtv = model.co_so_san_xuat_thuoc_bvtv?.id,
                co_so_kinh_doanh_thuoc_bvtv = model.co_so_kinh_doanh_thuoc_bvtv?.id,
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
        public async Task<RequestHttpResponse<List<XuatNhapKhauThuocBVTVModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<XuatNhapKhauThuocBVTVModel>>>(url);

                return response.IsSuccess
                    ? new RequestHttpResponse<List<XuatNhapKhauThuocBVTVModel>> { Data = response.Data?.Data }
                    : new RequestHttpResponse<List<XuatNhapKhauThuocBVTVModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<XuatNhapKhauThuocBVTVModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<XuatNhapKhauThuocBVTVModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<XuatNhapKhauThuocBVTVModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<XuatNhapKhauThuocBVTVModel>>($"items/{_collection}/{id}?fields={Fields}");

                return response.IsSuccess
                    ? new RequestHttpResponse<XuatNhapKhauThuocBVTVModel> { Data = response.Data?.Data }
                    : new RequestHttpResponse<XuatNhapKhauThuocBVTVModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<XuatNhapKhauThuocBVTVModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<XuatNhapKhauThuocBVTVModel>> CreateAsync(XuatNhapKhauThuocBVTVModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<XuatNhapKhauThuocBVTVModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = MapToCRUDModel(model);
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<XuatNhapKhauThuocBVTVModel>>($"items/{_collection}?fields={Fields}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<XuatNhapKhauThuocBVTVModel> { Errors = response.Errors };
                }

                return response.Data ?? new RequestHttpResponse<XuatNhapKhauThuocBVTVModel>();
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<XuatNhapKhauThuocBVTVModel>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(XuatNhapKhauThuocBVTVModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<XuatNhapKhauThuocBVTVCRUDModel>>($"items/{_collection}/{model.id}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(XuatNhapKhauThuocBVTVModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<XuatNhapKhauThuocBVTVCRUDModel>>($"items/{_collection}/{model.id}", new { deleted = true });

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
