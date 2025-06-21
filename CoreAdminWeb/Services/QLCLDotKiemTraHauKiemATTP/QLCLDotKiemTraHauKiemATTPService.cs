using CoreAdminWeb.Model;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.RequestHttp;
using CoreAdminWeb.Services.BaseServices;
using System.Net;

namespace CoreAdminWeb.Services
{
    public class QLCLDotKiemTraHauKiemATTPService : IBaseService<QLCLDotKiemTraHauKiemATTPModel>
    {
        private readonly string _collection = "QLCLDotKiemTraHauKiemATTP";
        private const string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name";

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
        private static QLCLDotKiemTraHauKiemATTPCRUDModel MapToCRUDModel(QLCLDotKiemTraHauKiemATTPModel model)
        {
            return new()
            {
                bat_dau_tu = model.bat_dau_tu,
                den = model.den,
                so_quyet_dinh = model.so_quyet_dinh,
                don_vi_chu_tri = model.don_vi_chu_tri,
                noi_dung_kiem_tra = model.noi_dung_kiem_tra,
                status = model.status.ToString(),
                code = model.code,
                name = model.name,
                deleted = model.deleted,
                description = model.description,
                sort = model.sort
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<QLCLDotKiemTraHauKiemATTPModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<QLCLDotKiemTraHauKiemATTPModel>>>(url);

                return response.IsSuccess
                    ? new RequestHttpResponse<List<QLCLDotKiemTraHauKiemATTPModel>> { Data = response.Data?.Data, Meta = response.Data?.Meta }
                    : new RequestHttpResponse<List<QLCLDotKiemTraHauKiemATTPModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<QLCLDotKiemTraHauKiemATTPModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<QLCLDotKiemTraHauKiemATTPModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<QLCLDotKiemTraHauKiemATTPModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<QLCLDotKiemTraHauKiemATTPModel>>($"items/{_collection}/{id}?fields={Fields}");

                return response.IsSuccess
                    ? new RequestHttpResponse<QLCLDotKiemTraHauKiemATTPModel> { Data = response.Data?.Data, Meta = response.Data?.Meta }
                    : new RequestHttpResponse<QLCLDotKiemTraHauKiemATTPModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<QLCLDotKiemTraHauKiemATTPModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<QLCLDotKiemTraHauKiemATTPModel>> CreateAsync(QLCLDotKiemTraHauKiemATTPModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<QLCLDotKiemTraHauKiemATTPModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = MapToCRUDModel(model);
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<QLCLDotKiemTraHauKiemATTPCRUDModel>>($"items/{_collection}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<QLCLDotKiemTraHauKiemATTPModel> { Errors = response.Errors };
                }

                return new RequestHttpResponse<QLCLDotKiemTraHauKiemATTPModel>
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
                return CreateErrorResponse<QLCLDotKiemTraHauKiemATTPModel>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(QLCLDotKiemTraHauKiemATTPModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<QLCLDotKiemTraHauKiemATTPCRUDModel>>($"items/{_collection}/{model.id}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(QLCLDotKiemTraHauKiemATTPModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<QLCLDotKiemTraHauKiemATTPCRUDModel>>($"items/{_collection}/{model.id}", new { deleted = true });

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
