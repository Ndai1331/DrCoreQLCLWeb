using CoreAdminWeb.Model;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.RequestHttp;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text.Json;
namespace CoreAdminWeb.Services
{

    public interface IQLCLPhatTrienThiTruongSanPhamService
    {
        Task<RequestHttpResponse<List<QLCLPhatTrienThiTruongSanPhamModel>>> GetAllAsync(string query);
        Task<RequestHttpResponse<QLCLPhatTrienThiTruongSanPhamModel>> GetByIdAsync(string id);
        Task<RequestHttpResponse<List<QLCLPhatTrienThiTruongSanPhamModel>>> CreateAsync(List<QLCLPhatTrienThiTruongSanPhamModel> model);
        Task<RequestHttpResponse<bool>> UpdateAsync(List<QLCLPhatTrienThiTruongSanPhamModel> model);
        Task<RequestHttpResponse<bool>> DeleteAsync(List<QLCLPhatTrienThiTruongSanPhamModel> model);
    }

    public class QLCLPhatTrienThiTruongSanPhamService : IQLCLPhatTrienThiTruongSanPhamService
    {
        private readonly string _collection = "QLCLPhatTrienThiTruongSanPham";
        private const string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name"
            + ",phat_trien_thi_truong.id"
            + ",san_pham.id,san_pham.name,san_pham.code";

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
        private static QLCLPhatTrienThiTruongSanPhamCRUDModel MapToCRUDModel(QLCLPhatTrienThiTruongSanPhamModel model)
        {
            return new()
            {
                phat_trien_thi_truong = model.phat_trien_thi_truong?.id,
                san_pham = model.san_pham?.id,
                so_luong = model.so_luong,
                description = model.description,
                deleted = false
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<QLCLPhatTrienThiTruongSanPhamModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<QLCLPhatTrienThiTruongSanPhamModel>>>(url);

                return response.IsSuccess
                    ? new RequestHttpResponse<List<QLCLPhatTrienThiTruongSanPhamModel>> { Data = response.Data?.Data, Meta = response.Data?.Meta }
                    : new RequestHttpResponse<List<QLCLPhatTrienThiTruongSanPhamModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<QLCLPhatTrienThiTruongSanPhamModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<QLCLPhatTrienThiTruongSanPhamModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<QLCLPhatTrienThiTruongSanPhamModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<QLCLPhatTrienThiTruongSanPhamModel>>($"items/{_collection}/{id}?fields={Fields}");

                return response.IsSuccess
                    ? new RequestHttpResponse<QLCLPhatTrienThiTruongSanPhamModel> { Data = response.Data?.Data, Meta = response.Data?.Meta }
                    : new RequestHttpResponse<QLCLPhatTrienThiTruongSanPhamModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<QLCLPhatTrienThiTruongSanPhamModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<QLCLPhatTrienThiTruongSanPhamModel>> CreateAsync(QLCLPhatTrienThiTruongSanPhamModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<QLCLPhatTrienThiTruongSanPhamModel>
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
                    return new RequestHttpResponse<QLCLPhatTrienThiTruongSanPhamModel> { Errors = response.Errors };
                }

                return new RequestHttpResponse<QLCLPhatTrienThiTruongSanPhamModel>
                {
                    Data = new()
                    {
                    }
                };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<QLCLPhatTrienThiTruongSanPhamModel>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(QLCLPhatTrienThiTruongSanPhamModel model)
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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(QLCLPhatTrienThiTruongSanPhamModel model)
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



        public async Task<RequestHttpResponse<List<QLCLPhatTrienThiTruongSanPhamModel>>> CreateAsync(List<QLCLPhatTrienThiTruongSanPhamModel> model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<List<QLCLPhatTrienThiTruongSanPhamModel>>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = model.Select(c => MapToCRUDModel(c)).ToList();
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<List<QLCLPhatTrienThiTruongSanPhamModel>>>($"items/{_collection}?fields={Fields}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<List<QLCLPhatTrienThiTruongSanPhamModel>> { Errors = response.Errors };
                }

                return response.Data ?? new RequestHttpResponse<List<QLCLPhatTrienThiTruongSanPhamModel>>();
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<QLCLPhatTrienThiTruongSanPhamModel>>(ex);
            }
        }


        public async Task<RequestHttpResponse<bool>> UpdateAsync(List<QLCLPhatTrienThiTruongSanPhamModel> model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<List<QLCLPhatTrienThiTruongSanPhamModel>>>($"items/{_collection}?fields={Fields}", updateModel);

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

        public async Task<RequestHttpResponse<bool>> DeleteAsync(List<QLCLPhatTrienThiTruongSanPhamModel> model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<List<QLCLPhatTrienThiTruongSanPhamModel>>>($"items/{_collection}?fields={Fields}", model.Select(c => new { id = c.id, deleted = true }));

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
