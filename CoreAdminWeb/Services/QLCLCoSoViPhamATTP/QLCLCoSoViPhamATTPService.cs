using CoreAdminWeb.Model;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.RequestHttp;
using CoreAdminWeb.Services.BaseServices;
using System.Net;

namespace CoreAdminWeb.Services
{
    public class QLCLCoSoViPhamATTPService : IBaseService<QLCLCoSoViPhamATTPModel>
    {
        private readonly string _collection = "QLCLCoSoViPhamATTP";
        private const string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name"
            + ",co_so_che_bien_nlts.id,co_so_che_bien_nlts.code,co_so_che_bien_nlts.name,co_so_che_bien_nlts.dia_chi"
            + ",co_so_nlts_du_dieu_kien_attp.id,co_so_nlts_du_dieu_kien_attp.code,co_so_nlts_du_dieu_kien_attp.name,co_so_nlts_du_dieu_kien_attp.dia_chi"
            + ",hanh_vi_vi_pham.id,hanh_vi_vi_pham,hanh_vi_vi_pham.name"
            + ",hinh_thuc_xu_phat.id,hinh_thuc_xu_phat.name"
            + ",don_vi_tinh.id,don_vi_tinh.name";

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
        private static QLCLCoSoViPhamATTPCRUDModel MapToCRUDModel(QLCLCoSoViPhamATTPModel model)
        {
            return new()
            {
                code = model.code,
                name = model.name,
                description = model.description,
                sort = model.sort,
                status = model.status,
                co_so_che_bien_nlts = model.co_so_che_bien_nlts?.id,
                co_so_nlts_du_dieu_kien_attp = model.co_so_nlts_du_dieu_kien_attp?.id,
                noi_dung_vi_pham = model.noi_dung_vi_pham,
                san_pham_vi_pham = model.san_pham_vi_pham,
                hanh_vi_vi_pham = model.hanh_vi_vi_pham?.id,
                huong_xu_ly = model.huong_xu_ly,
                phat_tien = model.phat_tien,
                hinh_thuc_xu_phat = model.hinh_thuc_xu_phat?.id,
                khac_phuc = model.khac_phuc,
                so_luong = model.so_luong,
                don_vi_tinh = model.don_vi_tinh?.id,
                gia_tri_tang_vat = model.gia_tri_tang_vat,
                xu_ly_khac = model.xu_ly_khac,
                ngay_ghi_nhan = model.ngay_ghi_nhan,
                ngay_xu_ly = model.ngay_xu_ly,
                co_quan_xu_ly = model.co_quan_xu_ly,
                loai_co_so = model.loai_co_so
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<QLCLCoSoViPhamATTPModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<QLCLCoSoViPhamATTPModel>>>(url);

                return response.IsSuccess
                    ? new RequestHttpResponse<List<QLCLCoSoViPhamATTPModel>> { Data = response.Data?.Data }
                    : new RequestHttpResponse<List<QLCLCoSoViPhamATTPModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<QLCLCoSoViPhamATTPModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<QLCLCoSoViPhamATTPModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<QLCLCoSoViPhamATTPModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<QLCLCoSoViPhamATTPModel>>($"items/{_collection}/{id}?fields={Fields}");

                return response.IsSuccess
                    ? new RequestHttpResponse<QLCLCoSoViPhamATTPModel> { Data = response.Data?.Data }
                    : new RequestHttpResponse<QLCLCoSoViPhamATTPModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<QLCLCoSoViPhamATTPModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<QLCLCoSoViPhamATTPModel>> CreateAsync(QLCLCoSoViPhamATTPModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<QLCLCoSoViPhamATTPModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = MapToCRUDModel(model);
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<QLCLCoSoViPhamATTPCRUDModel>>($"items/{_collection}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<QLCLCoSoViPhamATTPModel> { Errors = response.Errors };
                }

                return new RequestHttpResponse<QLCLCoSoViPhamATTPModel>
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
                return CreateErrorResponse<QLCLCoSoViPhamATTPModel>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(QLCLCoSoViPhamATTPModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<QLCLCoSoViPhamATTPCRUDModel>>($"items/{_collection}/{model.id}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(QLCLCoSoViPhamATTPModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<QLCLCoSoViPhamATTPCRUDModel>>($"items/{_collection}/{model.id}", new { deleted = true });

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
