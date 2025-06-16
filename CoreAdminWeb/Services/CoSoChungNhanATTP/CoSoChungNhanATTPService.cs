using CoreAdminWeb.Model.CoSoChungNhanATTP;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.RequestHttp;
using CoreAdminWeb.Services.BaseServices;
using System.Net;

namespace CoreAdminWeb.Services.CoSoChungNhanATTP
{
    public class CoSoChungNhanATTPService : IBaseService<CoSoChungNhanATTPModel>
    {
        private readonly string _collection = "CoSoChungNhanATTP";
        private const string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name"
            + ",province.id,province.name"
            + ",ward.id,ward.name"
            + ",co_so_trong_trot_san_xuat.id,co_so_trong_trot_san_xuat.code,co_so_trong_trot_san_xuat.name,co_so_trong_trot_san_xuat.dia_chi";

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
        private static CoSoChungNhanATTPCRUDModel MapToCRUDModel(CoSoChungNhanATTPModel model)
        {
            return new()
            {
                code = model.code,
                name = model.name,
                description = model.description,
                status = ((int)model.status).ToString(),
                sort = model.sort,
                deleted = model.deleted,
                province = model.province?.id,
                ward = model.ward?.id,
                so_gcn = model.so_gcn,
                ngay_cap = model.ngay_cap,
                ngay_het_han = model.ngay_het_han,
                co_quan_cap_phep = model.co_quan_cap_phep,
                hinh_thuc_cap = ((int)model.hinh_thuc_cap).ToString(),
                loai_gcn = ((int)model.loai_gcn).ToString(),
                co_so_trong_trot_san_xuat = model.co_so_trong_trot_san_xuat?.id
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<CoSoChungNhanATTPModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<CoSoChungNhanATTPModel>>>(url);

                return response.IsSuccess
                    ? new RequestHttpResponse<List<CoSoChungNhanATTPModel>> { Data = response.Data?.Data }
                    : new RequestHttpResponse<List<CoSoChungNhanATTPModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<CoSoChungNhanATTPModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<CoSoChungNhanATTPModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<CoSoChungNhanATTPModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<CoSoChungNhanATTPModel>>($"items/{_collection}/{id}?fields={Fields}");

                return response.IsSuccess
                    ? new RequestHttpResponse<CoSoChungNhanATTPModel> { Data = response.Data?.Data }
                    : new RequestHttpResponse<CoSoChungNhanATTPModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<CoSoChungNhanATTPModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<CoSoChungNhanATTPModel>> CreateAsync(CoSoChungNhanATTPModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<CoSoChungNhanATTPModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = MapToCRUDModel(model);
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<CoSoChungNhanATTPCRUDModel>>($"items/{_collection}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<CoSoChungNhanATTPModel> { Errors = response.Errors };
                }

                return new RequestHttpResponse<CoSoChungNhanATTPModel>
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
                return CreateErrorResponse<CoSoChungNhanATTPModel>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(CoSoChungNhanATTPModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<CoSoChungNhanATTPCRUDModel>>($"items/{_collection}/{model.id}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(CoSoChungNhanATTPModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<CoSoChungNhanATTPCRUDModel>>($"items/{_collection}/{model.id}", new { deleted = true });

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
