using CoreAdminWeb.Model;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.RequestHttp;
using CoreAdminWeb.Services.BaseServices;
using System.Net;

namespace CoreAdminWeb.Services
{
    public class QLCLCoSoVatTuNongNghiepService : IBaseService<QLCLCoSoVatTuNongNghiepModel>
    {
        private readonly string _collection = "QLCLCoSoVatTuNongNghiep";
        private const string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name"
            + ",province.id,province,province.name,ward.id,ward,ward.name,loai_hinh_kinh_doanh.id,loai_hinh_kinh_doanh.code,loai_hinh_kinh_doanh.name"
            +",chi_tiets.id,chi_tiets.sort,chi_tiets.deleted,chi_tiets.san_pham.id,chi_tiets.san_pham.name";

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
        private static QLCLCoSoVatTuNongNghiepCRUDModel MapToCRUDModel(QLCLCoSoVatTuNongNghiepModel model)
        {
            return new()
            {
                code = model.code,
                name = model.name,
                description = model.description,
                sort = model.sort,
                status = model.status.ToString(),
                province = model.province?.id,
                ward = model.ward?.id,
                dia_chi = model.dia_chi,
                dien_thoai = model.dien_thoai,
                dai_dien = model.dai_dien,
                so_giay_chung_nhan = model.so_giay_chung_nhan,
                loai_hinh_kinh_doanh = model.loai_hinh_kinh_doanh?.id,
                ngay_cap = model.ngay_cap,
                ngay_het_hieu_luc = model.ngay_het_hieu_luc,
                co_quan_cap = model.co_quan_cap,
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<QLCLCoSoVatTuNongNghiepModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<QLCLCoSoVatTuNongNghiepModel>>>(url);

                return response.IsSuccess
                    ? new RequestHttpResponse<List<QLCLCoSoVatTuNongNghiepModel>> { Data = response.Data?.Data }
                    : new RequestHttpResponse<List<QLCLCoSoVatTuNongNghiepModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<QLCLCoSoVatTuNongNghiepModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<QLCLCoSoVatTuNongNghiepModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<QLCLCoSoVatTuNongNghiepModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<QLCLCoSoVatTuNongNghiepModel>>($"items/{_collection}/{id}?fields={Fields}");

                return response.IsSuccess
                    ? new RequestHttpResponse<QLCLCoSoVatTuNongNghiepModel> { Data = response.Data?.Data }
                    : new RequestHttpResponse<QLCLCoSoVatTuNongNghiepModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<QLCLCoSoVatTuNongNghiepModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<QLCLCoSoVatTuNongNghiepModel>> CreateAsync(QLCLCoSoVatTuNongNghiepModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<QLCLCoSoVatTuNongNghiepModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = MapToCRUDModel(model);
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<QLCLCoSoVatTuNongNghiepCRUDResponseModel>>($"items/{_collection}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<QLCLCoSoVatTuNongNghiepModel> { Errors = response.Errors };
                }

                return new RequestHttpResponse<QLCLCoSoVatTuNongNghiepModel>
                {
                    Data = new()
                    {
                        id = response.Data?.Data?.id ?? 0
                    }
                };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<QLCLCoSoVatTuNongNghiepModel>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(QLCLCoSoVatTuNongNghiepModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<QLCLCoSoVatTuNongNghiepCRUDModel>>($"items/{_collection}/{model.id}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(QLCLCoSoVatTuNongNghiepModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<QLCLCoSoVatTuNongNghiepCRUDModel>>($"items/{_collection}/{model.id}", new { deleted = true });

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
