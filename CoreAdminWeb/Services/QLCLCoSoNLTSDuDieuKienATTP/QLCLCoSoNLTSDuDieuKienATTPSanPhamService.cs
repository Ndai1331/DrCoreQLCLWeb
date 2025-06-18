using CoreAdminWeb.Model;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.RequestHttp;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text.Json;
namespace CoreAdminWeb.Services
{

    public interface IQLCLCoSoNLTSDuDieuKienATTPSanPhamService
    {
        Task<RequestHttpResponse<List<QLCLCoSoNLTSDuDieuKienATTPSanPhamModel>>> GetAllAsync(string query);
        Task<RequestHttpResponse<QLCLCoSoNLTSDuDieuKienATTPSanPhamModel>> GetByIdAsync(string id);
        Task<RequestHttpResponse<List<QLCLCoSoNLTSDuDieuKienATTPSanPhamModel>>> CreateAsync(List<QLCLCoSoNLTSDuDieuKienATTPSanPhamModel> model);
        Task<RequestHttpResponse<bool>> UpdateAsync(List<QLCLCoSoNLTSDuDieuKienATTPSanPhamModel> model);
        Task<RequestHttpResponse<bool>> DeleteAsync(List<QLCLCoSoNLTSDuDieuKienATTPSanPhamModel> model);
    }

    public class QLCLCoSoNLTSDuDieuKienATTPSanPhamService : IQLCLCoSoNLTSDuDieuKienATTPSanPhamService
    {
        private readonly string _collection = "QLCLCoSoNLTSDuDieuKienATTPSanPham";
        private const string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name"
            + ",co_so_nlts_du_dieu_kien_attp.id,"
            + "san_pham.id, san_pham.name,san_pham.code";

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
        private static QLCLCoSoNLTSDuDieuKienATTPSanPhamCRUDModel MapToCRUDModel(QLCLCoSoNLTSDuDieuKienATTPSanPhamModel model)
        {
            return new()
            {
                co_so_nlts_du_dieu_kien_attp = model.co_so_nlts_du_dieu_kien_attp?.id,
                san_pham = model.san_pham?.id,
                deleted = false
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<QLCLCoSoNLTSDuDieuKienATTPSanPhamModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<QLCLCoSoNLTSDuDieuKienATTPSanPhamModel>>>(url);

                return response.IsSuccess
                    ? new RequestHttpResponse<List<QLCLCoSoNLTSDuDieuKienATTPSanPhamModel>> { Data = response.Data?.Data }
                    : new RequestHttpResponse<List<QLCLCoSoNLTSDuDieuKienATTPSanPhamModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<QLCLCoSoNLTSDuDieuKienATTPSanPhamModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<QLCLCoSoNLTSDuDieuKienATTPSanPhamModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<QLCLCoSoNLTSDuDieuKienATTPSanPhamModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<QLCLCoSoNLTSDuDieuKienATTPSanPhamModel>>($"items/{_collection}/{id}?fields={Fields}");

                return response.IsSuccess
                    ? new RequestHttpResponse<QLCLCoSoNLTSDuDieuKienATTPSanPhamModel> { Data = response.Data?.Data }
                    : new RequestHttpResponse<QLCLCoSoNLTSDuDieuKienATTPSanPhamModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<QLCLCoSoNLTSDuDieuKienATTPSanPhamModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<QLCLCoSoNLTSDuDieuKienATTPSanPhamModel>> CreateAsync(QLCLCoSoNLTSDuDieuKienATTPSanPhamModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<QLCLCoSoNLTSDuDieuKienATTPSanPhamModel>
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
                    return new RequestHttpResponse<QLCLCoSoNLTSDuDieuKienATTPSanPhamModel> { Errors = response.Errors };
                }

                return new RequestHttpResponse<QLCLCoSoNLTSDuDieuKienATTPSanPhamModel>
                {
                    Data = new()
                    {
                    }
                };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<QLCLCoSoNLTSDuDieuKienATTPSanPhamModel>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(QLCLCoSoNLTSDuDieuKienATTPSanPhamModel model)
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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(QLCLCoSoNLTSDuDieuKienATTPSanPhamModel model)
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



        public async Task<RequestHttpResponse<List<QLCLCoSoNLTSDuDieuKienATTPSanPhamModel>>> CreateAsync(List<QLCLCoSoNLTSDuDieuKienATTPSanPhamModel> model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<List<QLCLCoSoNLTSDuDieuKienATTPSanPhamModel>>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = model.Select(c => MapToCRUDModel(c)).ToList();
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<List<QLCLCoSoNLTSDuDieuKienATTPSanPhamModel>>>($"items/{_collection}?fields={Fields}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<List<QLCLCoSoNLTSDuDieuKienATTPSanPhamModel>> { Errors = response.Errors };
                }

                return response.Data ?? new RequestHttpResponse<List<QLCLCoSoNLTSDuDieuKienATTPSanPhamModel>>();
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<QLCLCoSoNLTSDuDieuKienATTPSanPhamModel>>(ex);
            }
        }


        public async Task<RequestHttpResponse<bool>> UpdateAsync(List<QLCLCoSoNLTSDuDieuKienATTPSanPhamModel> model)
        {
            if (model == null || model.Any(c => c.id == 0) || !model.Any())
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
                var updateModel = model.Select(c =>
                {
                    string jsonStr = JsonSerializer.Serialize(MapToCRUDModel(c));
                    JObject jObject = JObject.Parse(jsonStr);
                    dynamic dynamicObject = jObject;
                    dynamicObject.id = c.id;

                    return dynamicObject;
                }).ToList();
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<List<QLCLCoSoNLTSDuDieuKienATTPSanPhamModel>>>($"items/{_collection}?fields={Fields}", updateModel);

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

        public async Task<RequestHttpResponse<bool>> DeleteAsync(List<QLCLCoSoNLTSDuDieuKienATTPSanPhamModel> model)
        {
            if (model == null || model.Any(c => c.id == 0) || !model.Any())
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<List<QLCLCoSoNLTSDuDieuKienATTPSanPhamModel>>>($"items/{_collection}?fields={Fields}", model.Select(c => new { id = c.id, deleted = true }));

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
