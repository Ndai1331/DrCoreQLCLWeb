using CoreAdminWeb.Model;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.RequestHttp;
using CoreAdminWeb.Services.BaseServices;
using System.Net;

namespace CoreAdminWeb.Services
{
    public class QLCLChuoiCungUngATTPService : IBaseService<QLCLChuoiCungUngATTPModel>
    {
        private readonly string _collection = "QLCLChuoiCungUngATTP";
        private const string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name"
            + ",province_san_xuat.id,province_san_xuat,province_san_xuat.name,ward_san_xuat.id,ward_san_xuat,ward_san_xuat.name"
            + ",province_kinh_doanh.id,province_kinh_doanh,province_kinh_doanh.name,ward_kinh_doanh.id,ward_kinh_doanh,ward_kinh_doanh.name"
            +",chi_tiets.id,chi_tiets.sort,chi_tiets.deleted,chi_tiets.loai_co_so,chi_tiets.co_so_che_bien_nlts.id,chi_tiets.co_so_che_bien_nlts.name,chi_tiets.co_so_nlts_du_dieu_kien_attp.id,chi_tiets.co_so_nlts_du_dieu_kien_attp.name";

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
        private static QLCLChuoiCungUngATTPCRUDModel MapToCRUDModel(QLCLChuoiCungUngATTPModel model)
        {
            return new()
            {
                code = model.code,
                name = model.name,
                description = model.description,
                sort = model.sort,
                status = model.status.ToString(),
                province_san_xuat = model.province_san_xuat?.id,
                ward_san_xuat = model.ward_san_xuat?.id,
                dia_chi_san_xuat = model.dia_chi_san_xuat,
                province_kinh_doanh = model.province_kinh_doanh?.id,
                ward_kinh_doanh = model.ward_kinh_doanh?.id,
                dia_chi_kinh_doanh = model.dia_chi_kinh_doanh,
                so_xac_nhan = model.so_xac_nhan,
                ngay_chung_nhan = model.ngay_chung_nhan,
                co_quan_xac_nhan = model.co_quan_xac_nhan,
                san_pham = model.san_pham,
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<QLCLChuoiCungUngATTPModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<QLCLChuoiCungUngATTPModel>>>(url);

                return response.IsSuccess
                    ? new RequestHttpResponse<List<QLCLChuoiCungUngATTPModel>> { Data = response.Data?.Data, Meta = response.Data?.Meta }
                    : new RequestHttpResponse<List<QLCLChuoiCungUngATTPModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<QLCLChuoiCungUngATTPModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<QLCLChuoiCungUngATTPModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<QLCLChuoiCungUngATTPModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<QLCLChuoiCungUngATTPModel>>($"items/{_collection}/{id}?fields={Fields}");

                return response.IsSuccess
                    ? new RequestHttpResponse<QLCLChuoiCungUngATTPModel> { Data = response.Data?.Data, Meta = response.Data?.Meta }
                    : new RequestHttpResponse<QLCLChuoiCungUngATTPModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<QLCLChuoiCungUngATTPModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<QLCLChuoiCungUngATTPModel>> CreateAsync(QLCLChuoiCungUngATTPModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<QLCLChuoiCungUngATTPModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = MapToCRUDModel(model);
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<QLCLChuoiCungUngATTPCRUDResponseModel>>($"items/{_collection}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<QLCLChuoiCungUngATTPModel> { Errors = response.Errors };
                }

                return new RequestHttpResponse<QLCLChuoiCungUngATTPModel>
                {
                    Data = new()
                    {
                        id = response.Data?.Data?.id ?? 0
                    }
                };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<QLCLChuoiCungUngATTPModel>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(QLCLChuoiCungUngATTPModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<QLCLChuoiCungUngATTPCRUDModel>>($"items/{_collection}/{model.id}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(QLCLChuoiCungUngATTPModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<QLCLChuoiCungUngATTPCRUDModel>>($"items/{_collection}/{model.id}", new { deleted = true });

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
