using CoreAdminWeb.Model.DuBaoDichBenh;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.RequestHttp;
using CoreAdminWeb.Services.BaseServices;
using System.Net;

namespace CoreAdminWeb.Services.DuBaoDichBenh
{
    public class DuBaoDichBenhService : IBaseService<DuBaoDichBenhModel>
    {
        private readonly string _collection = "DuBaoDichBenh";
        private const string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name"
            + ",vi_sinh_vat_gay_hai.id,vi_sinh_vat_gay_hai.name"
            + ",chi_tiet.loai_cay_trong.id,chi_tiet.loai_cay_trong.name";

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
        private static DuBaoDichBenhCRUDModel MapToCRUDModel(DuBaoDichBenhModel model)
        {
            return new()
            {
                code = model.code,
                name = model.name,
                description = model.description,
                status = model.status.ToString(),
                sort = model.sort,
                du_lieu_khi_hau = model.du_lieu_khi_hau,
                tinh_trang_sinh_truong = model.tinh_trang_sinh_truong,
                bien_dong_quan_the = model.bien_dong_quan_the,
                du_bao_ngan_han = model.du_bao_ngan_han,
                muc_do_nguy_co = model.muc_do_nguy_co?.ToString(),
                vi_sinh_vat_gay_hai = model.vi_sinh_vat_gay_hai?.id,
                bien_phap_phong_tru = model.bien_phap_phong_tru,
                ngay_du_bao = model.ngay_du_bao,
                tu_ngay = model.tu_ngay,
                den_ngay = model.den_ngay,
                du_bao_dai_han = model.du_bao_dai_han,
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<DuBaoDichBenhModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<DuBaoDichBenhModel>>>(url);

                return response.IsSuccess
                    ? new RequestHttpResponse<List<DuBaoDichBenhModel>> { Data = response.Data?.Data }
                    : new RequestHttpResponse<List<DuBaoDichBenhModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<DuBaoDichBenhModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<DuBaoDichBenhModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<DuBaoDichBenhModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<DuBaoDichBenhModel>>($"items/{_collection}/{id}?fields={Fields}");

                return response.IsSuccess
                    ? new RequestHttpResponse<DuBaoDichBenhModel> { Data = response.Data?.Data }
                    : new RequestHttpResponse<DuBaoDichBenhModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<DuBaoDichBenhModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<DuBaoDichBenhModel>> CreateAsync(DuBaoDichBenhModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<DuBaoDichBenhModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = MapToCRUDModel(model);
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<DuBaoDichBenhCRUDModel>>($"items/{_collection}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<DuBaoDichBenhModel> { Errors = response.Errors };
                }

                return new RequestHttpResponse<DuBaoDichBenhModel>
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
                return CreateErrorResponse<DuBaoDichBenhModel>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(DuBaoDichBenhModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<DuBaoDichBenhCRUDModel>>($"items/{_collection}/{model.id}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(DuBaoDichBenhModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<DuBaoDichBenhCRUDModel>>($"items/{_collection}/{model.id}", new { deleted = true });

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
