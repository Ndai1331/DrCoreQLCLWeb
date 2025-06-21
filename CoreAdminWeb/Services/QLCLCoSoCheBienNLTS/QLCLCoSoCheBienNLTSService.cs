using CoreAdminWeb.Model;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.RequestHttp;
using CoreAdminWeb.Services.BaseServices;
using System.Net;

namespace CoreAdminWeb.Services
{
    public class QLCLCoSoCheBienNLTSService : IBaseService<QLCLCoSoCheBienNLTSModel>
    {
        private readonly string _collection = "QLCLCoSoCheBienNLTS";
        private const string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name"
            + ",province.id,province,province.name,ward.id,ward,ward.name,"
            + "pham_vi_hoat_dong.id,pham_vi_hoat_dong,pham_vi_hoat_dong.name,"
            + "loai_hinh_co_so.id,loai_hinh_co_so,loai_hinh_co_so.name,"
            + "nguyen_lieu_che_bien.id,nguyen_lieu_che_bien,nguyen_lieu_che_bien.name";

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
        private static QLCLCoSoCheBienNLTSCRUDModel MapToCRUDModel(QLCLCoSoCheBienNLTSModel model)
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
                cong_suat_tan_nam = model.cong_suat_tan_nam,
                so_giay_phep = model.so_giay_phep,
                loai_hinh_co_so = model.loai_hinh_co_so?.id,
                pham_vi_hoat_dong = model.pham_vi_hoat_dong,
                nguyen_lieu_che_bien = model.nguyen_lieu_che_bien?.id,
                ngay_cap = model.ngay_cap,
                co_quan_cap_phep = model.co_quan_cap_phep,
                san_luong_du_kien = model.san_luong_du_kien,
                quy_mo = model.quy_mo,
                chung_nhan_attp = model.chung_nhan_attp,
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<QLCLCoSoCheBienNLTSModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<QLCLCoSoCheBienNLTSModel>>>(url);

                return response.IsSuccess
                    ? new RequestHttpResponse<List<QLCLCoSoCheBienNLTSModel>> { Data = response.Data?.Data, Meta = response.Data?.Meta }
                    : new RequestHttpResponse<List<QLCLCoSoCheBienNLTSModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<QLCLCoSoCheBienNLTSModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<QLCLCoSoCheBienNLTSModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<QLCLCoSoCheBienNLTSModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<QLCLCoSoCheBienNLTSModel>>($"items/{_collection}/{id}?fields={Fields}");

                return response.IsSuccess
                    ? new RequestHttpResponse<QLCLCoSoCheBienNLTSModel> { Data = response.Data?.Data, Meta = response.Data?.Meta }
                    : new RequestHttpResponse<QLCLCoSoCheBienNLTSModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<QLCLCoSoCheBienNLTSModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<QLCLCoSoCheBienNLTSModel>> CreateAsync(QLCLCoSoCheBienNLTSModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<QLCLCoSoCheBienNLTSModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = MapToCRUDModel(model);
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<QLCLCoSoCheBienNLTSCRUDModel>>($"items/{_collection}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<QLCLCoSoCheBienNLTSModel> { Errors = response.Errors };
                }

                return new RequestHttpResponse<QLCLCoSoCheBienNLTSModel>
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
                return CreateErrorResponse<QLCLCoSoCheBienNLTSModel>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(QLCLCoSoCheBienNLTSModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<QLCLCoSoCheBienNLTSCRUDModel>>($"items/{_collection}/{model.id}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(QLCLCoSoCheBienNLTSModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<QLCLCoSoCheBienNLTSCRUDModel>>($"items/{_collection}/{model.id}", new { deleted = true });

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
