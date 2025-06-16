using CoreAdminWeb.Model.CoSoBuonBan;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.RequestHttp;
using CoreAdminWeb.Services.BaseServices;
using System.Net;

namespace CoreAdminWeb.Services.CoSoBuonBans
{
    public class CoSoBuonBanService : IBaseService<CoSoDuDieuKienBuonBanPhanBonModel>
    {
        private readonly string _collection = "CoSoDuDieuKienBuonBanPhanBon";
        private const string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name"
            + ",province.id,province.name"
            + ",ward.id,ward.name"
            + ",loai_hinh_kinh_doanh.id,loai_hinh_kinh_doanh.name";

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
        private static CoSoDuDieuKienBuonBanPhanBonCRUDModel MapToCRUDModel(CoSoDuDieuKienBuonBanPhanBonModel model)
        {
            return new()
            {
                code = model.code,
                name = model.name,
                description = model.description,
                status = model.status.ToString(),
                sort = model.sort,
                dia_chi = model.dia_chi,
                dien_thoai = model.dien_thoai,
                email = model.email,
                so_cccd = model.so_cccd,
                nguoi_dai_dien = model.nguoi_dai_dien,
                so_giay_phep_kinh_doanh = model.so_giay_phep_kinh_doanh,
                ngay_cap = model.ngay_cap,
                co_quan_cap_phep = model.co_quan_cap_phep,
                nhom_co_so = model.nhom_co_so,
                province = model.province?.id ?? 0,
                ward = model.ward?.id ?? 0,
                loai_hinh_kinh_doanh = model.loai_hinh_kinh_doanh?.id
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<CoSoDuDieuKienBuonBanPhanBonModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<CoSoDuDieuKienBuonBanPhanBonModel>>>(url);

                return response.IsSuccess
                    ? new RequestHttpResponse<List<CoSoDuDieuKienBuonBanPhanBonModel>> { Data = response.Data?.Data }
                    : new RequestHttpResponse<List<CoSoDuDieuKienBuonBanPhanBonModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<CoSoDuDieuKienBuonBanPhanBonModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<CoSoDuDieuKienBuonBanPhanBonModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<CoSoDuDieuKienBuonBanPhanBonModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<CoSoDuDieuKienBuonBanPhanBonModel>>($"items/{_collection}/{id}?fields={Fields}");

                return response.IsSuccess
                    ? new RequestHttpResponse<CoSoDuDieuKienBuonBanPhanBonModel> { Data = response.Data?.Data }
                    : new RequestHttpResponse<CoSoDuDieuKienBuonBanPhanBonModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<CoSoDuDieuKienBuonBanPhanBonModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<CoSoDuDieuKienBuonBanPhanBonModel>> CreateAsync(CoSoDuDieuKienBuonBanPhanBonModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<CoSoDuDieuKienBuonBanPhanBonModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = MapToCRUDModel(model);
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<CoSoDuDieuKienBuonBanPhanBonCRUDModel>>($"items/{_collection}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<CoSoDuDieuKienBuonBanPhanBonModel> { Errors = response.Errors };
                }

                return new RequestHttpResponse<CoSoDuDieuKienBuonBanPhanBonModel>
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
                return CreateErrorResponse<CoSoDuDieuKienBuonBanPhanBonModel>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(CoSoDuDieuKienBuonBanPhanBonModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<CoSoDuDieuKienBuonBanPhanBonCRUDModel>>($"items/{_collection}/{model.id}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(CoSoDuDieuKienBuonBanPhanBonModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<CoSoDuDieuKienBuonBanPhanBonCRUDModel>>($"items/{_collection}/{model.id}", new { deleted = true });

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
