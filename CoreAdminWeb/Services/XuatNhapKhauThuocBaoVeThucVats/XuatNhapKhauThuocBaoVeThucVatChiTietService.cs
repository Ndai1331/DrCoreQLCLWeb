using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.Model.XuatNhapKhauPhanBon;
using CoreAdminWeb.RequestHttp;
using CoreAdminWeb.Services.BaseServices;
using System.Net;

namespace CoreAdminWeb.Services.XuatNhapKhauPhanBons
{
    public class XuatNhapKhauThuocBaoVeThucVatChiTietService : IBaseService<XuatNhapKhauThuocBVTVChiTietModel>
    {
        private readonly string _collection = "XNKThuocBVTVChiTiet";
        private const string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name"
            + ",thuoc_bvtv.id,thuoc_bvtv.name"
            + ",thuoc_bvtv.loai_thuoc_bvtv.id,thuoc_bvtv.loai_thuoc_bvtv.name"
            + ",thuoc_bvtv.don_vi_tinh.id,thuoc_bvtv.don_vi_tinh.name"
            + ",xnk_thuoc_bvtv.id,xnk_thuoc_bvtv.so_chung_tu,xnk_thuoc_bvtv.description,xnk_thuoc_bvtv.status"
            + ",xnk_thuoc_bvtv.ngay_chung_tu,xnk_thuoc_bvtv.hinh_thuc,xnk_thuoc_bvtv.giay_phep_xnk,xnk_thuoc_bvtv.ngay_cap"
            + ",xnk_thuoc_bvtv.co_quan_cap,xnk_thuoc_bvtv.co_so_san_xuat_thuoc_bvtv.id,xnk_thuoc_bvtv.co_so_kinh_doanh_thuoc_bvtv.id";

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
        private static XuatNhapKhauThuocBVTVChiTietCRUDModel MapToCRUDModel(XuatNhapKhauThuocBVTVChiTietModel model)
        {
            return new()
            {
                dvt = model.dvt,
                description = model.description,
                nuoc_xuat_nhap = model.nuoc_xuat_nhap,
                thanh_phan_ty_le = model.thanh_phan_ty_le,
                so_luong = model.so_luong,
                xnk_thuoc_bvtv = model.xnk_thuoc_bvtv?.id,
                thuoc_bvtv = model.thuoc_bvtv?.id,
                status = model.status.ToString(),
                sort = model.sort
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<XuatNhapKhauThuocBVTVChiTietModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<XuatNhapKhauThuocBVTVChiTietModel>>>(url);

                return response.IsSuccess
                    ? new RequestHttpResponse<List<XuatNhapKhauThuocBVTVChiTietModel>> { Data = response.Data?.Data }
                    : new RequestHttpResponse<List<XuatNhapKhauThuocBVTVChiTietModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<XuatNhapKhauThuocBVTVChiTietModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<XuatNhapKhauThuocBVTVChiTietModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<XuatNhapKhauThuocBVTVChiTietModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<XuatNhapKhauThuocBVTVChiTietModel>>($"items/{_collection}/{id}?fields={Fields}");

                return response.IsSuccess
                    ? new RequestHttpResponse<XuatNhapKhauThuocBVTVChiTietModel> { Data = response.Data?.Data }
                    : new RequestHttpResponse<XuatNhapKhauThuocBVTVChiTietModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<XuatNhapKhauThuocBVTVChiTietModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<XuatNhapKhauThuocBVTVChiTietModel>> CreateAsync(XuatNhapKhauThuocBVTVChiTietModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<XuatNhapKhauThuocBVTVChiTietModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = MapToCRUDModel(model);
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<XuatNhapKhauThuocBVTVChiTietCRUDModel>>($"items/{_collection}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<XuatNhapKhauThuocBVTVChiTietModel> { Errors = response.Errors };
                }

                return new RequestHttpResponse<XuatNhapKhauThuocBVTVChiTietModel>
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
                return CreateErrorResponse<XuatNhapKhauThuocBVTVChiTietModel>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(XuatNhapKhauThuocBVTVChiTietModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<XuatNhapKhauThuocBVTVChiTietCRUDModel>>($"items/{_collection}/{model.id}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(XuatNhapKhauThuocBVTVChiTietModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<XuatNhapKhauThuocBVTVChiTietCRUDModel>>($"items/{_collection}/{model.id}", new { deleted = true });

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
