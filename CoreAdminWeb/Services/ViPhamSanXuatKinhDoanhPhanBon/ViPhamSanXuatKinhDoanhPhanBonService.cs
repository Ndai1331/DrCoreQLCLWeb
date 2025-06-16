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
    public class ViPhamSanXuatKinhDoanhPhanBonService : IBaseService<ViPhamSanXuatKinhDoanhPhanBonModel>
    {
        private readonly string _collection = "ViPhamSanXuatKinhDoanhPhanBon";
        private const string Fields = "*,user_created.last_name," +
                                      "user_created.first_name,user_updated.last_name,user_updated.first_name,"
            + "co_so_san_xuat_phan_bon.id, co_so_san_xuat_phan_bon.name, co_so_du_dieu_kien_buon_ban_phan_bon.id,co_so_du_dieu_kien_buon_ban_phan_bon.name";

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
        private static ViPhamSanXuatKinhDoanhPhanBonCRUDModel MapToCRUDModel(ViPhamSanXuatKinhDoanhPhanBonModel model)
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
                loai_to_chuc = model.loai_to_chuc == null ? (model.co_so_san_xuat_phan_bon != null ? LoaiToChuc.CoSoSanXuat : LoaiToChuc.CoSoKinhDoanh) : model.loai_to_chuc,
                co_so_san_xuat_phan_bon = model.co_so_san_xuat_phan_bon?.id,
                co_so_du_dieu_kien_buon_ban_phan_bon = model.co_so_du_dieu_kien_buon_ban_phan_bon?.id,
                doi_tuong_vi_pham = model.doi_tuong_vi_pham,
                noi_dung_vi_pham = model.noi_dung_vi_pham,
                hinh_thuc_xu_ly = model.hinh_thuc_xu_ly
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<ViPhamSanXuatKinhDoanhPhanBonModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<ViPhamSanXuatKinhDoanhPhanBonModel>>>(url);
                
                return response.IsSuccess 
                    ? new RequestHttpResponse<List<ViPhamSanXuatKinhDoanhPhanBonModel>> { Data = response.Data.Data }
                    : new RequestHttpResponse<List<ViPhamSanXuatKinhDoanhPhanBonModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<ViPhamSanXuatKinhDoanhPhanBonModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<ViPhamSanXuatKinhDoanhPhanBonModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<ViPhamSanXuatKinhDoanhPhanBonModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<ViPhamSanXuatKinhDoanhPhanBonModel>>($"items/{_collection}/{id}?fields={Fields}");
                
                return response.IsSuccess
                    ? new RequestHttpResponse<ViPhamSanXuatKinhDoanhPhanBonModel> { Data = response.Data.Data }
                    : new RequestHttpResponse<ViPhamSanXuatKinhDoanhPhanBonModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<ViPhamSanXuatKinhDoanhPhanBonModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<ViPhamSanXuatKinhDoanhPhanBonModel>> CreateAsync(ViPhamSanXuatKinhDoanhPhanBonModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<ViPhamSanXuatKinhDoanhPhanBonModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = MapToCRUDModel(model);
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<ViPhamSanXuatKinhDoanhPhanBonCRUDModel>>($"items/{_collection}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<ViPhamSanXuatKinhDoanhPhanBonModel> { Errors = response.Errors };
                }

                return new RequestHttpResponse<ViPhamSanXuatKinhDoanhPhanBonModel>
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
                return CreateErrorResponse<ViPhamSanXuatKinhDoanhPhanBonModel>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(ViPhamSanXuatKinhDoanhPhanBonModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<ViPhamSanXuatKinhDoanhPhanBonCRUDModel>>($"items/{_collection}/{model.id}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(ViPhamSanXuatKinhDoanhPhanBonModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<ViPhamSanXuatKinhDoanhPhanBonCRUDModel>>($"items/{_collection}/{model.id}", new { deleted = true });

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