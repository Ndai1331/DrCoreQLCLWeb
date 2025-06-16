using CoreAdminWeb.Model;
using CoreAdminWeb.Model.CayTrongDuocPhepGieoTrongKhuVuc;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.RequestHttp;
using CoreAdminWeb.Services.BaseServices;
using System.Net;

namespace CoreAdminWeb.Services.CayTrongDuocPhepGieoTrongKhuVuc
{
    public class CayTrongDuocPhepGieoTrongKhuVucService : IBaseService<CayTrongDuocPhepGieoTrongKhuVucModel>
    {
        private readonly string _collection = "CayTrongDuocPhepGieoTrongKhuVuc";
        private const string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name"
            + ",cay_giong_cay_trong.id,cay_giong_cay_trong.code,cay_giong_cay_trong.name,cay_giong_cay_trong.loai_cay_trong.id,cay_giong_cay_trong.loai_cay_trong.name,cay_giong_cay_trong.nguon_goc"
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
        private static CayTrongDuocPhepGieoTrongKhuVucCRUDModel MapToCRUDModel(CayTrongDuocPhepGieoTrongKhuVucModel model)
        {
            return new()
            {
                dieu_kien_khi_hau = model.dieu_kien_khi_hau,
                dieu_kien_dat_dai = model.dieu_kien_dat_dai,
                thoi_vu_gieo_trong_khuyen_nghi = model.thoi_vu_gieo_trong_khuyen_nghi,
                hieu_qua_kinh_te = model.hieu_qua_kinh_te,
                cay_giong_cay_trong = model.cay_giong_cay_trong?.id,
                province = model.province?.id,
                ward = model.ward?.id,
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<CayTrongDuocPhepGieoTrongKhuVucModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<CayTrongDuocPhepGieoTrongKhuVucModel>>>(url);

                return response.IsSuccess
                    ? new RequestHttpResponse<List<CayTrongDuocPhepGieoTrongKhuVucModel>> { Data = response.Data?.Data }
                    : new RequestHttpResponse<List<CayTrongDuocPhepGieoTrongKhuVucModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<CayTrongDuocPhepGieoTrongKhuVucModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<CayTrongDuocPhepGieoTrongKhuVucModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<CayTrongDuocPhepGieoTrongKhuVucModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<CayTrongDuocPhepGieoTrongKhuVucModel>>($"items/{_collection}/{id}?fields={Fields}");

                return response.IsSuccess
                    ? new RequestHttpResponse<CayTrongDuocPhepGieoTrongKhuVucModel> { Data = response.Data?.Data }
                    : new RequestHttpResponse<CayTrongDuocPhepGieoTrongKhuVucModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<CayTrongDuocPhepGieoTrongKhuVucModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<CayTrongDuocPhepGieoTrongKhuVucModel>> CreateAsync(CayTrongDuocPhepGieoTrongKhuVucModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<CayTrongDuocPhepGieoTrongKhuVucModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = MapToCRUDModel(model);
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<CayTrongDuocPhepGieoTrongKhuVucCRUDModel>>($"items/{_collection}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<CayTrongDuocPhepGieoTrongKhuVucModel> { Errors = response.Errors };
                }

                return new RequestHttpResponse<CayTrongDuocPhepGieoTrongKhuVucModel>
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
                return CreateErrorResponse<CayTrongDuocPhepGieoTrongKhuVucModel>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(CayTrongDuocPhepGieoTrongKhuVucModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<CayTrongDuocPhepGieoTrongKhuVucCRUDModel>>($"items/{_collection}/{model.id}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(CayTrongDuocPhepGieoTrongKhuVucModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<CayTrongDuocPhepGieoTrongKhuVucCRUDModel>>($"items/{_collection}/{model.id}", new { deleted = true });

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
