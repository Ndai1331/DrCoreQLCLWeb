using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.Model.SanXuatUngDungCongNgheCao;
using CoreAdminWeb.RequestHttp;
using CoreAdminWeb.Services.BaseServices;
using System.Net;

namespace CoreAdminWeb.Services.SanXuatUngDungCongNgheCao
{
    public class SanXuatUngDungCongNgheCaoService : IBaseService<SanXuatUngDungCongNgheCaoModel>
    {
        private readonly string _collection = "SanXuatUngDungCongNgheCao";
        private const string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name"
            + ",province.id,province.name"
            + ",ward.id,ward.name"
            + ",loai_hinh_canh_tac.id,loai_hinh_canh_tac.name"
            + ",co_so_trong_trot_san_xuat.id,co_so_trong_trot_san_xuat.name"
            + ",loai_cay_trong.id,loai_cay_trong.loai_cay_trong.name,loai_cay_trong.dien_tich,loai_cay_trong.san_luong,loai_cay_trong.deleted";

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
        private static SanXuatUngDungCongNgheCaoCRUDModel MapToCRUDModel(SanXuatUngDungCongNgheCaoModel model)
        {
            return new()
            {
                code = model.code,
                name = model.name,
                status = model.status.ToString(),
                sort = model.sort,
                description = model.description,
                deleted = model.deleted,
                province = model.province?.id,
                ward = model.ward?.id,
                dia_diem_trien_khai = model.dia_diem_trien_khai,
                thoi_gian_bat_dau = model.thoi_gian_bat_dau,
                thoi_gian_ket_thuc = model.thoi_gian_ket_thuc,
                cong_nghe_canh_tac = model.cong_nghe_canh_tac,
                cong_nghe_giong_cay_trong = model.cong_nghe_giong_cay_trong,
                muc_tieu = model.muc_tieu,
                co_so_trong_trot_san_xuat = model.co_so_trong_trot_san_xuat?.id,
                mo_hinh_du_an = model.mo_hinh_du_an
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<SanXuatUngDungCongNgheCaoModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<SanXuatUngDungCongNgheCaoModel>>>(url);

                return response.IsSuccess
                    ? new RequestHttpResponse<List<SanXuatUngDungCongNgheCaoModel>> { Data = response.Data?.Data }
                    : new RequestHttpResponse<List<SanXuatUngDungCongNgheCaoModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<SanXuatUngDungCongNgheCaoModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<SanXuatUngDungCongNgheCaoModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<SanXuatUngDungCongNgheCaoModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<SanXuatUngDungCongNgheCaoModel>>($"items/{_collection}/{id}?fields={Fields}");

                return response.IsSuccess
                    ? new RequestHttpResponse<SanXuatUngDungCongNgheCaoModel> { Data = response.Data?.Data }
                    : new RequestHttpResponse<SanXuatUngDungCongNgheCaoModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<SanXuatUngDungCongNgheCaoModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<SanXuatUngDungCongNgheCaoModel>> CreateAsync(SanXuatUngDungCongNgheCaoModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<SanXuatUngDungCongNgheCaoModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = MapToCRUDModel(model);
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<SanXuatUngDungCongNgheCaoModel>>($"items/{_collection}?fields={Fields}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<SanXuatUngDungCongNgheCaoModel> { Errors = response.Errors };
                }

                return response.Data ?? new RequestHttpResponse<SanXuatUngDungCongNgheCaoModel>();
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<SanXuatUngDungCongNgheCaoModel>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(SanXuatUngDungCongNgheCaoModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<SanXuatUngDungCongNgheCaoCRUDModel>>($"items/{_collection}/{model.id}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(SanXuatUngDungCongNgheCaoModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<SanXuatUngDungCongNgheCaoCRUDModel>>($"items/{_collection}/{model.id}", new { deleted = true });

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
