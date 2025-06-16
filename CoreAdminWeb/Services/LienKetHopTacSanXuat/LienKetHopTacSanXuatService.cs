using CoreAdminWeb.Model.LienKetHopTacSanXuat;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.RequestHttp;
using CoreAdminWeb.Services.BaseServices;
using System.Net;

namespace CoreAdminWeb.Services.LienKetHopTacSanXuat
{
    public class LienKetHopTacSanXuatService : IBaseService<LienKetHopTacSanXuatModel>
    {
        private readonly string _collection = "LienKetHopTacSanXuat";
        private const string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name"
            + ",cay_trong.id,cay_trong.name"
            + ",don_vi_tham_gia.dien_tich,don_vi_tham_gia.san_luong"
            + ",hinh_thuc_lien_ket.id,hinh_thuc_lien_ket.name";

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
        private static LienKetHopTacSanXuatCRUDModel MapToCRUDModel(LienKetHopTacSanXuatModel model)
        {
            return new()
            {
                code = model.code,
                name = model.name,
                status = model.status.ToString(),
                sort = model.sort,
                description = model.description,
                deleted = model.deleted,
                cay_trong = model.cay_trong?.id,
                hinh_thuc_lien_ket = model.hinh_thuc_lien_ket?.id,
                don_vi_chu_tri = model.don_vi_chu_tri,
                mo_hinh_hop_tac = model.mo_hinh_hop_tac,
                thoi_gian_tu = model.thoi_gian_tu,
                thoi_gian_den = model.thoi_gian_den,
                muc_tieu = model.muc_tieu
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<LienKetHopTacSanXuatModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<LienKetHopTacSanXuatModel>>>(url);

                return response.IsSuccess
                    ? new RequestHttpResponse<List<LienKetHopTacSanXuatModel>> { Data = response.Data?.Data }
                    : new RequestHttpResponse<List<LienKetHopTacSanXuatModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<LienKetHopTacSanXuatModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<LienKetHopTacSanXuatModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<LienKetHopTacSanXuatModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<LienKetHopTacSanXuatModel>>($"items/{_collection}/{id}?fields={Fields}");

                return response.IsSuccess
                    ? new RequestHttpResponse<LienKetHopTacSanXuatModel> { Data = response.Data?.Data }
                    : new RequestHttpResponse<LienKetHopTacSanXuatModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<LienKetHopTacSanXuatModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<LienKetHopTacSanXuatModel>> CreateAsync(LienKetHopTacSanXuatModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<LienKetHopTacSanXuatModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = MapToCRUDModel(model);
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<LienKetHopTacSanXuatModel>>($"items/{_collection}?fields={Fields}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<LienKetHopTacSanXuatModel> { Errors = response.Errors };
                }

                return response.Data ?? new RequestHttpResponse<LienKetHopTacSanXuatModel>();
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<LienKetHopTacSanXuatModel>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(LienKetHopTacSanXuatModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<LienKetHopTacSanXuatCRUDModel>>($"items/{_collection}/{model.id}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(LienKetHopTacSanXuatModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<LienKetHopTacSanXuatCRUDModel>>($"items/{_collection}/{model.id}", new { deleted = true });

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
