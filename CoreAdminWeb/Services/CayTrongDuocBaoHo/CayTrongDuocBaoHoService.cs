using CoreAdminWeb.Model;
using CoreAdminWeb.Model.CayTrongDuocBaoHo;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.RequestHttp;
using CoreAdminWeb.Services.BaseServices;
using System.Net;

namespace CoreAdminWeb.Services.CayTrongDuocBaoHo
{
    public class CayTrongDuocBaoHoService : IBaseService<CayTrongDuocBaoHoModel>
    {
        private readonly string _collection = "CayTrongDuocBaoHo";
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
        private static CayTrongDuocBaoHoCRUDModel MapToCRUDModel(CayTrongDuocBaoHoModel model)
        {
            return new()
            {
                to_chuc_ca_nhan = model.to_chuc_ca_nhan,
                dia_chi = model.dia_chi,
                ma_so_bao_ho = model.ma_so_bao_ho,
                pham_vi_bao_ho = model.pham_vi_bao_ho,
                ngay_cap = model.ngay_cap,
                ngay_het_han = model.ngay_het_han,
                cay_giong_cay_trong = model.cay_giong_cay_trong?.id,
                province = model.province?.id,
                ward = model.ward?.id,
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<CayTrongDuocBaoHoModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<CayTrongDuocBaoHoModel>>>(url);

                return response.IsSuccess
                    ? new RequestHttpResponse<List<CayTrongDuocBaoHoModel>> { Data = response.Data?.Data }
                    : new RequestHttpResponse<List<CayTrongDuocBaoHoModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<CayTrongDuocBaoHoModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<CayTrongDuocBaoHoModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<CayTrongDuocBaoHoModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<CayTrongDuocBaoHoModel>>($"items/{_collection}/{id}?fields={Fields}");

                return response.IsSuccess
                    ? new RequestHttpResponse<CayTrongDuocBaoHoModel> { Data = response.Data?.Data }
                    : new RequestHttpResponse<CayTrongDuocBaoHoModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<CayTrongDuocBaoHoModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<CayTrongDuocBaoHoModel>> CreateAsync(CayTrongDuocBaoHoModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<CayTrongDuocBaoHoModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = MapToCRUDModel(model);
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<CayTrongDuocBaoHoCRUDModel>>($"items/{_collection}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<CayTrongDuocBaoHoModel> { Errors = response.Errors };
                }

                return new RequestHttpResponse<CayTrongDuocBaoHoModel>
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
                return CreateErrorResponse<CayTrongDuocBaoHoModel>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(CayTrongDuocBaoHoModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<CayTrongDuocBaoHoCRUDModel>>($"items/{_collection}/{model.id}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(CayTrongDuocBaoHoModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<CayTrongDuocBaoHoCRUDModel>>($"items/{_collection}/{model.id}", new { deleted = true });

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
