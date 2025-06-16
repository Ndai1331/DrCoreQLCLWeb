using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.Services.BaseServices;
using CoreAdminWeb.RequestHttp;
using System.Net;
using CoreAdminWeb.Model;

namespace CoreAdminWeb.Services
{
    /// <summary>
    /// Service for managing fertilizer production facilities
    /// </summary>
    public class QuanLyCoSoSanXuatThuocBVTVService : IBaseService<QuanLyCoSoSanXuatThuocBVTVModel>
    {
        private readonly string _collection = "CoSoSanXuatThuocBVTV";
        private const string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name,"
            + "province.name,ward.name,loai_hinh_kinh_doanh.name,"
            + "province.id,ward.id,loai_hinh_kinh_doanh.id";

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
        private static QuanLyCoSoSanXuatThuocBVTVCRUDModel MapToCRUDModel(QuanLyCoSoSanXuatThuocBVTVModel model)
        {
            return new()
            {
                code = model.code,
                name = model.name,
                description = model.description,
                status = model.status.ToString(),
                sort = model.sort,
                province = model.province?.id,
                ward = model.ward?.id,
                loai_hinh_kinh_doanh = model.loai_hinh_kinh_doanh?.id,
                loai_hinh_san_xuat = model.loai_hinh_san_xuat,
                dia_chi = model.dia_chi,
                dien_thoai = model.dien_thoai,
                email = model.email,
                so_cccd = model.so_cccd,
                nguoi_dai_dien = model.nguoi_dai_dien,
                pham_vi_phan_phoi = model.pham_vi_phan_phoi,
                cong_suat_thiet_ke = model.cong_suat_thiet_ke,
                so_gcn_du_dieu_kien = model.so_gcn_du_dieu_kien,
                ngay_cap = model.ngay_cap,
                co_quan_cap_phep = model.co_quan_cap_phep,
                chung_nhan_tieu_chuan_chat_luong = model.chung_nhan_tieu_chuan_chat_luong,
                nguon_nguyen_lieu_dau_vao = model.nguon_nguyen_lieu_dau_vao
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<QuanLyCoSoSanXuatThuocBVTVModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<QuanLyCoSoSanXuatThuocBVTVModel>>>(url);
                
                return response.IsSuccess 
                    ? new RequestHttpResponse<List<QuanLyCoSoSanXuatThuocBVTVModel>> { Data = response.Data.Data }
                    : new RequestHttpResponse<List<QuanLyCoSoSanXuatThuocBVTVModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<QuanLyCoSoSanXuatThuocBVTVModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<QuanLyCoSoSanXuatThuocBVTVModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<QuanLyCoSoSanXuatThuocBVTVModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<QuanLyCoSoSanXuatThuocBVTVModel>>($"items/{_collection}/{id}?fields={Fields}");
                
                return response.IsSuccess
                    ? new RequestHttpResponse<QuanLyCoSoSanXuatThuocBVTVModel> { Data = response.Data.Data }
                    : new RequestHttpResponse<QuanLyCoSoSanXuatThuocBVTVModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<QuanLyCoSoSanXuatThuocBVTVModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<QuanLyCoSoSanXuatThuocBVTVModel>> CreateAsync(QuanLyCoSoSanXuatThuocBVTVModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<QuanLyCoSoSanXuatThuocBVTVModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = MapToCRUDModel(model);
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<QuanLyCoSoSanXuatThuocBVTVCRUDModel>>($"items/{_collection}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<QuanLyCoSoSanXuatThuocBVTVModel> { Errors = response.Errors };
                }

                return new RequestHttpResponse<QuanLyCoSoSanXuatThuocBVTVModel>
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
                return CreateErrorResponse<QuanLyCoSoSanXuatThuocBVTVModel>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(QuanLyCoSoSanXuatThuocBVTVModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<QuanLyCoSoSanXuatThuocBVTVCRUDModel>>($"items/{_collection}/{model.id}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(QuanLyCoSoSanXuatThuocBVTVModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<QuanLyCoSoSanXuatThuocBVTVCRUDModel>>($"items/{_collection}/{model.id}", new { deleted = true });

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