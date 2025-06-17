using CoreAdminWeb.Model;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.RequestHttp;
using CoreAdminWeb.Services.BaseServices;
using Newtonsoft.Json.Linq;
using System.Dynamic;
using System.Net;
using System.Text.Json;
namespace CoreAdminWeb.Services
{

    public interface IQLCLTinhHinhSXKDNLTSSanPhamService
    {
        Task<RequestHttpResponse<List<QLCLTinhHinhSXKDNLTSSanPhamModel>>> GetAllAsync(string query);
        Task<RequestHttpResponse<QLCLTinhHinhSXKDNLTSSanPhamModel>> GetByIdAsync(string id);
        Task<RequestHttpResponse<List<QLCLTinhHinhSXKDNLTSSanPhamModel>>> CreateAsync(List<QLCLTinhHinhSXKDNLTSSanPhamModel> model);
        Task<RequestHttpResponse<bool>> UpdateAsync(List<QLCLTinhHinhSXKDNLTSSanPhamModel> model);
        Task<RequestHttpResponse<bool>> DeleteAsync(List<QLCLTinhHinhSXKDNLTSSanPhamModel> model);
    }

    public class QLCLTinhHinhSXKDNLTSSanPhamService : IQLCLTinhHinhSXKDNLTSSanPhamService
    {
        private readonly string _collection = "QLCLTinhHinhSXKDNLTSSanPham";
        private const string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name"
            + ",tinh_hinh_san_xuat_kinh_doanh_nlts.id,"
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
        private static QLCLTinhHinhSXKDNLTSSanPhamCRUDModel MapToCRUDModel(QLCLTinhHinhSXKDNLTSSanPhamModel model)
        {
            return new()
            {
                tinh_hinh_san_xuat_kinh_doanh_nlts = model.tinh_hinh_san_xuat_kinh_doanh_nlts?.id,
                san_pham = model.san_pham?.id,
                san_luong_tan = model.san_luong_tan,
                sort = model.sort,
                deleted = false
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<QLCLTinhHinhSXKDNLTSSanPhamModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<QLCLTinhHinhSXKDNLTSSanPhamModel>>>(url);

                return response.IsSuccess
                    ? new RequestHttpResponse<List<QLCLTinhHinhSXKDNLTSSanPhamModel>> { Data = response.Data?.Data }
                    : new RequestHttpResponse<List<QLCLTinhHinhSXKDNLTSSanPhamModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<QLCLTinhHinhSXKDNLTSSanPhamModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<QLCLTinhHinhSXKDNLTSSanPhamModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<QLCLTinhHinhSXKDNLTSSanPhamModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<QLCLTinhHinhSXKDNLTSSanPhamModel>>($"items/{_collection}/{id}?fields={Fields}");

                return response.IsSuccess
                    ? new RequestHttpResponse<QLCLTinhHinhSXKDNLTSSanPhamModel> { Data = response.Data?.Data }
                    : new RequestHttpResponse<QLCLTinhHinhSXKDNLTSSanPhamModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<QLCLTinhHinhSXKDNLTSSanPhamModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<QLCLTinhHinhSXKDNLTSSanPhamModel>> CreateAsync(QLCLTinhHinhSXKDNLTSSanPhamModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<QLCLTinhHinhSXKDNLTSSanPhamModel>
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
                    return new RequestHttpResponse<QLCLTinhHinhSXKDNLTSSanPhamModel> { Errors = response.Errors };
                }

                return new RequestHttpResponse<QLCLTinhHinhSXKDNLTSSanPhamModel>
                {
                    Data = new()
                    {
                    }
                };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<QLCLTinhHinhSXKDNLTSSanPhamModel>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(QLCLTinhHinhSXKDNLTSSanPhamModel model)
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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(QLCLTinhHinhSXKDNLTSSanPhamModel model)
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



        public async Task<RequestHttpResponse<List<QLCLTinhHinhSXKDNLTSSanPhamModel>>> CreateAsync(List<QLCLTinhHinhSXKDNLTSSanPhamModel> model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<List<QLCLTinhHinhSXKDNLTSSanPhamModel>>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = model.Select(c => MapToCRUDModel(c)).ToList();
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<List<QLCLTinhHinhSXKDNLTSSanPhamModel>>>($"items/{_collection}?fields={Fields}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<List<QLCLTinhHinhSXKDNLTSSanPhamModel>> { Errors = response.Errors };
                }

                return response.Data ?? new RequestHttpResponse<List<QLCLTinhHinhSXKDNLTSSanPhamModel>>();
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<QLCLTinhHinhSXKDNLTSSanPhamModel>>(ex);
            }
        }


        public async Task<RequestHttpResponse<bool>> UpdateAsync(List<QLCLTinhHinhSXKDNLTSSanPhamModel> model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<List<QLCLTinhHinhSXKDNLTSSanPhamModel>>>($"items/{_collection}?fields={Fields}", updateModel);

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

        public async Task<RequestHttpResponse<bool>> DeleteAsync(List<QLCLTinhHinhSXKDNLTSSanPhamModel> model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<List<QLCLTinhHinhSXKDNLTSSanPhamModel>>>($"items/{_collection}?fields={Fields}", model.Select(c => new { id = c.id, deleted = true }));

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
