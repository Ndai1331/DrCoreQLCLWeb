using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.Model.XuatNhapKhauPhanBon;
using CoreAdminWeb.RequestHttp;
using CoreAdminWeb.Services.BaseServices;
using System.Net;

namespace CoreAdminWeb.Services.XuatNhapKhauPhanBons
{
    public class XuatNhapKhauPhanBonChiTietService : IBaseService<XuatNhapKhauPhanBonChiTietModel>
    {
        private readonly string _collection = "XNKPhanBonChiTiet";
        private const string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name"
            + ",phan_bon.id,phan_bon.name"
            + ",phan_bon.loai_phan_bon.id,phan_bon.loai_phan_bon.name"
            + ",phan_bon.don_vi_tinh.id,phan_bon.don_vi_tinh.name"
            + ",xnk_phan_bon.id,xnk_phan_bon.so_chung_tu,xnk_phan_bon.description,xnk_phan_bon.status"
            + ",xnk_phan_bon.ngay_chung_tu,xnk_phan_bon.hinh_thuc,xnk_phan_bon.giay_phep_xnk,xnk_phan_bon.ngay_cap"
            + ",xnk_phan_bon.co_quan_cap,xnk_phan_bon.co_so_san_xuat_phan_bon.id,xnk_phan_bon.co_so_du_dieu_kien_buon_ban_phan_bon.id";

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
        private static XuatNhapKhauPhanBonChiTietCRUDModel MapToCRUDModel(XuatNhapKhauPhanBonChiTietModel model)
        {
            return new()
            {
                dvt = model.dvt,
                description = model.description,
                nuoc_xuat_nhap = model.nuoc_xuat_nhap,
                thanh_phan_ty_le = model.thanh_phan_ty_le,
                so_luong = model.so_luong,
                xnk_phan_bon = model.xnk_phan_bon?.id,
                phan_bon = model.phan_bon?.id,
                status = model.status.ToString(),
                sort = model.sort
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<XuatNhapKhauPhanBonChiTietModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<XuatNhapKhauPhanBonChiTietModel>>>(url);

                return response.IsSuccess
                    ? new RequestHttpResponse<List<XuatNhapKhauPhanBonChiTietModel>> { Data = response.Data?.Data }
                    : new RequestHttpResponse<List<XuatNhapKhauPhanBonChiTietModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<XuatNhapKhauPhanBonChiTietModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<XuatNhapKhauPhanBonChiTietModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<XuatNhapKhauPhanBonChiTietModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<XuatNhapKhauPhanBonChiTietModel>>($"items/{_collection}/{id}?fields={Fields}");

                return response.IsSuccess
                    ? new RequestHttpResponse<XuatNhapKhauPhanBonChiTietModel> { Data = response.Data?.Data }
                    : new RequestHttpResponse<XuatNhapKhauPhanBonChiTietModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<XuatNhapKhauPhanBonChiTietModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<XuatNhapKhauPhanBonChiTietModel>> CreateAsync(XuatNhapKhauPhanBonChiTietModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<XuatNhapKhauPhanBonChiTietModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = MapToCRUDModel(model);
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<XuatNhapKhauPhanBonChiTietCRUDModel>>($"items/{_collection}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<XuatNhapKhauPhanBonChiTietModel> { Errors = response.Errors };
                }

                return new RequestHttpResponse<XuatNhapKhauPhanBonChiTietModel>
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
                return CreateErrorResponse<XuatNhapKhauPhanBonChiTietModel>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(XuatNhapKhauPhanBonChiTietModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<XuatNhapKhauPhanBonChiTietCRUDModel>>($"items/{_collection}/{model.id}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(XuatNhapKhauPhanBonChiTietModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<XuatNhapKhauPhanBonChiTietCRUDModel>>($"items/{_collection}/{model.id}", new { deleted = true });

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
