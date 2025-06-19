using CoreAdminWeb.Model;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.RequestHttp;
using CoreAdminWeb.Services.BaseServices;
using System.Net;

namespace CoreAdminWeb.Services
{
    public class QLCLKiemTraHauKiemATTPService : IBaseService<QLCLKiemTraHauKiemATTPModel>
    {
        private readonly string _collection = "QLCLKiemTraHauKiemATTP";
        private const string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name"
            + ",province.id,province,province.name,ward.id,ward,ward.name,co_so.id,co_so.code,co_so.name"
            + ",chi_tiet.id, chi_tiet.san_pham.id,  chi_tiet.san_pham.name, dot_kiem_tra.id,dot_kiem_tra.code,dot_kiem_tra.name";

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
        private static QLCLKiemTraHauKiemATTPCRUDModel MapToCRUDModel(QLCLKiemTraHauKiemATTPModel model)
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
                dia_chi_san_xuat_kinh_doanh = model.dia_chi_san_xuat_kinh_doanh,
                loai_hinh_kiem_tra = model.loai_hinh_kiem_tra,
                hinh_thuc_xet_nghiem = model.hinh_thuc_xet_nghiem,
                ngay_kiem_tra = model.ngay_kiem_tra,
                co_quan_kiem_tra = model.co_quan_kiem_tra,
                noi_dung_kiem_tra = model.noi_dung_kiem_tra,
                ket_qua_kiem_tra = model.ket_qua_kiem_tra,
                tinh_hinh_vi_pham = model.tinh_hinh_vi_pham,
                co_so = model.co_so?.id,
                dot_kiem_tra = model.dot_kiem_tra?.id,
                bien_phap_xu_ly = model.bien_phap_xu_ly,
                deleted = false,
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<QLCLKiemTraHauKiemATTPModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<QLCLKiemTraHauKiemATTPModel>>>(url);

                return response.IsSuccess
                    ? new RequestHttpResponse<List<QLCLKiemTraHauKiemATTPModel>> { Data = response.Data?.Data }
                    : new RequestHttpResponse<List<QLCLKiemTraHauKiemATTPModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<QLCLKiemTraHauKiemATTPModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<QLCLKiemTraHauKiemATTPModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<QLCLKiemTraHauKiemATTPModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<QLCLKiemTraHauKiemATTPModel>>($"items/{_collection}/{id}?fields={Fields}");

                return response.IsSuccess
                    ? new RequestHttpResponse<QLCLKiemTraHauKiemATTPModel> { Data = response.Data?.Data }
                    : new RequestHttpResponse<QLCLKiemTraHauKiemATTPModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<QLCLKiemTraHauKiemATTPModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<QLCLKiemTraHauKiemATTPModel>> CreateAsync(QLCLKiemTraHauKiemATTPModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<QLCLKiemTraHauKiemATTPModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = MapToCRUDModel(model);
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<QLCLKiemTraHauKiemATTPCRUDResponseModel>>($"items/{_collection}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<QLCLKiemTraHauKiemATTPModel> { Errors = response.Errors };
                }

                return new RequestHttpResponse<QLCLKiemTraHauKiemATTPModel>
                {
                    Data = new()
                    {
                        id = response.Data?.Data?.id ?? 0
                    }
                };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<QLCLKiemTraHauKiemATTPModel>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(QLCLKiemTraHauKiemATTPModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<QLCLKiemTraHauKiemATTPCRUDModel>>($"items/{_collection}/{model.id}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(QLCLKiemTraHauKiemATTPModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<QLCLKiemTraHauKiemATTPCRUDModel>>($"items/{_collection}/{model.id}", new { deleted = true });

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
