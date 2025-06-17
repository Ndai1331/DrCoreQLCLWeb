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

    public interface IQLCLTinhHinhSXKDNLTSNguyenLieuService
    {
        Task<RequestHttpResponse<List<QLCLTinhHinhSXKDNLTSNguyenLieuModel>>> GetAllAsync(string query);
        Task<RequestHttpResponse<QLCLTinhHinhSXKDNLTSNguyenLieuModel>> GetByIdAsync(string id);
        Task<RequestHttpResponse<List<QLCLTinhHinhSXKDNLTSNguyenLieuModel>>> CreateAsync(List<QLCLTinhHinhSXKDNLTSNguyenLieuModel> model);
        Task<RequestHttpResponse<bool>> UpdateAsync(List<QLCLTinhHinhSXKDNLTSNguyenLieuModel> model);
        Task<RequestHttpResponse<bool>> DeleteAsync(List<QLCLTinhHinhSXKDNLTSNguyenLieuModel> model);
    }

    public class QLCLTinhHinhSXKDNLTSNguyenLieuService : IQLCLTinhHinhSXKDNLTSNguyenLieuService
    {
        private readonly string _collection = "QLCLTinhHinhSXKDNLTSNguyenLieu";
        private const string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name"
            + ",tinh_hinh_san_xuat_kinh_doanh_nlts.id,"
            + "nguyen_lieu.id, nguyen_lieu.name,nguyen_lieu.code";

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
        private static QLCLTinhHinhSXKDNLTSNguyenLieuCRUDModel MapToCRUDModel(QLCLTinhHinhSXKDNLTSNguyenLieuModel model)
        {
            return new()
            {
                tinh_hinh_san_xuat_kinh_doanh_nlts = model.tinh_hinh_san_xuat_kinh_doanh_nlts?.id,
                nguyen_lieu = model.nguyen_lieu?.id,
                khoi_luong_tan = model.khoi_luong_tan,
                sort = model.sort,
                deleted = false
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<QLCLTinhHinhSXKDNLTSNguyenLieuModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<QLCLTinhHinhSXKDNLTSNguyenLieuModel>>>(url);

                return response.IsSuccess
                    ? new RequestHttpResponse<List<QLCLTinhHinhSXKDNLTSNguyenLieuModel>> { Data = response.Data?.Data }
                    : new RequestHttpResponse<List<QLCLTinhHinhSXKDNLTSNguyenLieuModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<QLCLTinhHinhSXKDNLTSNguyenLieuModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<QLCLTinhHinhSXKDNLTSNguyenLieuModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<QLCLTinhHinhSXKDNLTSNguyenLieuModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<QLCLTinhHinhSXKDNLTSNguyenLieuModel>>($"items/{_collection}/{id}?fields={Fields}");

                return response.IsSuccess
                    ? new RequestHttpResponse<QLCLTinhHinhSXKDNLTSNguyenLieuModel> { Data = response.Data?.Data }
                    : new RequestHttpResponse<QLCLTinhHinhSXKDNLTSNguyenLieuModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<QLCLTinhHinhSXKDNLTSNguyenLieuModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<QLCLTinhHinhSXKDNLTSNguyenLieuModel>> CreateAsync(QLCLTinhHinhSXKDNLTSNguyenLieuModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<QLCLTinhHinhSXKDNLTSNguyenLieuModel>
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
                    return new RequestHttpResponse<QLCLTinhHinhSXKDNLTSNguyenLieuModel> { Errors = response.Errors };
                }

                return new RequestHttpResponse<QLCLTinhHinhSXKDNLTSNguyenLieuModel>
                {
                    Data = new()
                    {
                    }
                };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<QLCLTinhHinhSXKDNLTSNguyenLieuModel>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(QLCLTinhHinhSXKDNLTSNguyenLieuModel model)
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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(QLCLTinhHinhSXKDNLTSNguyenLieuModel model)
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



        public async Task<RequestHttpResponse<List<QLCLTinhHinhSXKDNLTSNguyenLieuModel>>> CreateAsync(List<QLCLTinhHinhSXKDNLTSNguyenLieuModel> model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<List<QLCLTinhHinhSXKDNLTSNguyenLieuModel>>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = model.Select(c => MapToCRUDModel(c)).ToList();
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<List<QLCLTinhHinhSXKDNLTSNguyenLieuModel>>>($"items/{_collection}?fields={Fields}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<List<QLCLTinhHinhSXKDNLTSNguyenLieuModel>> { Errors = response.Errors };
                }

                return response.Data ?? new RequestHttpResponse<List<QLCLTinhHinhSXKDNLTSNguyenLieuModel>>();
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<QLCLTinhHinhSXKDNLTSNguyenLieuModel>>(ex);
            }
        }


        public async Task<RequestHttpResponse<bool>> UpdateAsync(List<QLCLTinhHinhSXKDNLTSNguyenLieuModel> model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<List<QLCLTinhHinhSXKDNLTSNguyenLieuModel>>>($"items/{_collection}?fields={Fields}", updateModel);

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

        public async Task<RequestHttpResponse<bool>> DeleteAsync(List<QLCLTinhHinhSXKDNLTSNguyenLieuModel> model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<List<QLCLTinhHinhSXKDNLTSNguyenLieuModel>>>($"items/{_collection}?fields={Fields}", model.Select(c => new { id = c.id, deleted = true }));

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
