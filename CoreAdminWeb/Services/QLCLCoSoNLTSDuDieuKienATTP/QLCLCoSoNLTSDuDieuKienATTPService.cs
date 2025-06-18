using CoreAdminWeb.Model;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.RequestHttp;
using CoreAdminWeb.Services.BaseServices;
using System.Net;

namespace CoreAdminWeb.Services
{
    public class QLCLCoSoNLTSDuDieuKienATTPService : IBaseService<QLCLCoSoNLTSDuDieuKienATTPModel>
    {
        private readonly string _collection = "QLCLCoSoNLTSDuDieuKienATTP";
        private const string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name"
            + ",province.id,province,province.name,ward.id,ward,ward.name,loai_hinh_kinh_doanh.id,loai_hinh_kinh_doanh.code,loai_hinh_kinh_doanh.name";

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
        private static QLCLCoSoNLTSDuDieuKienATTPCRUDModel MapToCRUDModel(QLCLCoSoNLTSDuDieuKienATTPModel model)
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
                ngay_tham_dinh = model.ngay_tham_dinh,
                co_quan_cap = model.co_quan_cap,
                xu_ly_ket_qua = model.xu_ly_ket_qua,
                he_thong_quan_ly_chat_luong = model.he_thong_quan_ly_chat_luong,
                ket_qua_tham_dinh = model.ket_qua_tham_dinh,
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<QLCLCoSoNLTSDuDieuKienATTPModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<QLCLCoSoNLTSDuDieuKienATTPModel>>>(url);

                return response.IsSuccess
                    ? new RequestHttpResponse<List<QLCLCoSoNLTSDuDieuKienATTPModel>> { Data = response.Data?.Data }
                    : new RequestHttpResponse<List<QLCLCoSoNLTSDuDieuKienATTPModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<QLCLCoSoNLTSDuDieuKienATTPModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<QLCLCoSoNLTSDuDieuKienATTPModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<QLCLCoSoNLTSDuDieuKienATTPModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<QLCLCoSoNLTSDuDieuKienATTPModel>>($"items/{_collection}/{id}?fields={Fields}");

                return response.IsSuccess
                    ? new RequestHttpResponse<QLCLCoSoNLTSDuDieuKienATTPModel> { Data = response.Data?.Data }
                    : new RequestHttpResponse<QLCLCoSoNLTSDuDieuKienATTPModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<QLCLCoSoNLTSDuDieuKienATTPModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<QLCLCoSoNLTSDuDieuKienATTPModel>> CreateAsync(QLCLCoSoNLTSDuDieuKienATTPModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<QLCLCoSoNLTSDuDieuKienATTPModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = MapToCRUDModel(model);
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<QLCLCoSoNLTSDuDieuKienATTPCRUDResponseModel>>($"items/{_collection}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<QLCLCoSoNLTSDuDieuKienATTPModel> { Errors = response.Errors };
                }

                return new RequestHttpResponse<QLCLCoSoNLTSDuDieuKienATTPModel>
                {
                    Data = new()
                    {
                        id = response.Data?.Data?.id ?? 0
                    }
                };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<QLCLCoSoNLTSDuDieuKienATTPModel>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(QLCLCoSoNLTSDuDieuKienATTPModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<QLCLCoSoNLTSDuDieuKienATTPCRUDModel>>($"items/{_collection}/{model.id}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(QLCLCoSoNLTSDuDieuKienATTPModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<QLCLCoSoNLTSDuDieuKienATTPCRUDModel>>($"items/{_collection}/{model.id}", new { deleted = true });

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
