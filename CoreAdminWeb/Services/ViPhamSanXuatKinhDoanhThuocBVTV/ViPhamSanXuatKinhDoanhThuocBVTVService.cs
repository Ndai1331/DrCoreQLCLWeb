using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.RequestHttp;
using System.Net;
using CoreAdminWeb.Enums;
using CoreAdminWeb.Model;

namespace CoreAdminWeb.Services
{
    /// <summary>
    /// Service for managing fertilizer production facilities
    /// </summary>
    public class ViPhamSanXuatKinhDoanhThuocBVTVService : IBaseService<ViPhamSanXuatKinhDoanhThuocBVTVModel>
    {
        private readonly string _collection = "ViPhamSXKDThuocBVTV";
        private const string Fields = "*,user_created.last_name," +
                                      "user_created.first_name,user_updated.last_name,user_updated.first_name,"
            + "co_so_san_xuat_thuoc_bvtv.id, co_so_san_xuat_thuoc_bvtv.name, co_so_kinh_doanh_thuoc_bvtv.id,co_so_kinh_doanh_thuoc_bvtv.name";

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
        private static ViPhamSanXuatKinhDoanhThuocBVTVCRUDModel MapToCRUDModel(ViPhamSanXuatKinhDoanhThuocBVTVModel model)
        {
            return new()
            {
                code = model.code,
                name = model.name,
                description = model.description,
                status = model.status,
                sort = model.sort,
                ngay_phat_hien = model.ngay_phat_hien,
                ngay_xu_ly = model.ngay_xu_ly,
                don_vi_kiem_tra = model.don_vi_kiem_tra,
                dia_diem_vi_pham = model.dia_diem_vi_pham,
                loai_to_chuc = model.loai_to_chuc == null ? (model.co_so_san_xuat_thuoc_bvtv != null ? LoaiToChuc.CoSoSanXuat : LoaiToChuc.CoSoKinhDoanh) : model.loai_to_chuc,
                co_so_san_xuat_thuoc_bvtv = model.co_so_san_xuat_thuoc_bvtv?.id,
                co_so_kinh_doanh_thuoc_bvtv = model.co_so_kinh_doanh_thuoc_bvtv?.id,
                doi_tuong_vi_pham = model.doi_tuong_vi_pham,
                noi_dung_vi_pham = model.noi_dung_vi_pham,
                hinh_thuc_xu_ly = model.hinh_thuc_xu_ly
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<ViPhamSanXuatKinhDoanhThuocBVTVModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<ViPhamSanXuatKinhDoanhThuocBVTVModel>>>(url);
                
                return response.IsSuccess 
                    ? new RequestHttpResponse<List<ViPhamSanXuatKinhDoanhThuocBVTVModel>> { Data = response.Data.Data }
                    : new RequestHttpResponse<List<ViPhamSanXuatKinhDoanhThuocBVTVModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<ViPhamSanXuatKinhDoanhThuocBVTVModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<ViPhamSanXuatKinhDoanhThuocBVTVModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<ViPhamSanXuatKinhDoanhThuocBVTVModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<ViPhamSanXuatKinhDoanhThuocBVTVModel>>($"items/{_collection}/{id}?fields={Fields}");
                
                return response.IsSuccess
                    ? new RequestHttpResponse<ViPhamSanXuatKinhDoanhThuocBVTVModel> { Data = response.Data.Data }
                    : new RequestHttpResponse<ViPhamSanXuatKinhDoanhThuocBVTVModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<ViPhamSanXuatKinhDoanhThuocBVTVModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<ViPhamSanXuatKinhDoanhThuocBVTVModel>> CreateAsync(ViPhamSanXuatKinhDoanhThuocBVTVModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<ViPhamSanXuatKinhDoanhThuocBVTVModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = MapToCRUDModel(model);
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<ViPhamSanXuatKinhDoanhThuocBVTVCRUDModel>>($"items/{_collection}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<ViPhamSanXuatKinhDoanhThuocBVTVModel> { Errors = response.Errors };
                }

                return new RequestHttpResponse<ViPhamSanXuatKinhDoanhThuocBVTVModel>
                {
                    Data = new()
                    {
                        code = response.Data.Data.code,
                        name = response.Data.Data.name
                    }
                };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<ViPhamSanXuatKinhDoanhThuocBVTVModel>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(ViPhamSanXuatKinhDoanhThuocBVTVModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<ViPhamSanXuatKinhDoanhThuocBVTVCRUDModel>>($"items/{_collection}/{model.id}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(ViPhamSanXuatKinhDoanhThuocBVTVModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<ViPhamSanXuatKinhDoanhThuocBVTVCRUDModel>>($"items/{_collection}/{model.id}", new { deleted = true });

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