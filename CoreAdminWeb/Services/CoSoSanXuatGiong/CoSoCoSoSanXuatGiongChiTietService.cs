using CoreAdminWeb.Enums;
using CoreAdminWeb.Model;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.RequestHttp;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text.Json;

namespace CoreAdminWeb.Services
{
    public class CoSoSanXuatGiongChiTietService : ICoSoSanXuatGiongChiTietService
    {
        private readonly string _collection = "CoSoSanXuatGiongChiTiet";
        private const string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name"
            + ",co_so_san_xuat_giong.id,co_so_san_xuat_giong.name"
            + ",cay_giong_cay_trong.id,cay_giong_cay_trong.name";

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
        private static CoSoSanXuatGiongChiTietCRUDModel MapToCRUDModel(CoSoSanXuatGiongChiTietModel model)
        {
            return new()
            {
                co_so_san_xuat_giong = model.co_so_san_xuat_giong?.id,
                cay_giong_cay_trong = model.cay_giong_cay_trong?.id,
                status = model.status.ToString(),
                san_luong_du_kien = model.san_luong_du_kien,
                description = model.description,
                sort = model.sort
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<CoSoSanXuatGiongChiTietModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<CoSoSanXuatGiongChiTietModel>>>(url);

                return response.IsSuccess
                    ? new RequestHttpResponse<List<CoSoSanXuatGiongChiTietModel>> { Data = response.Data?.Data }
                    : new RequestHttpResponse<List<CoSoSanXuatGiongChiTietModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<CoSoSanXuatGiongChiTietModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<CoSoSanXuatGiongChiTietModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<CoSoSanXuatGiongChiTietModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<CoSoSanXuatGiongChiTietModel>>($"items/{_collection}/{id}?fields={Fields}");

                return response.IsSuccess
                    ? new RequestHttpResponse<CoSoSanXuatGiongChiTietModel> { Data = response.Data?.Data }
                    : new RequestHttpResponse<CoSoSanXuatGiongChiTietModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<CoSoSanXuatGiongChiTietModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<CoSoSanXuatGiongChiTietModel>> CreateAsync(CoSoSanXuatGiongChiTietModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<CoSoSanXuatGiongChiTietModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = MapToCRUDModel(model);
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<CoSoSanXuatGiongChiTietModel>>($"items/{_collection}?fields={Fields}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<CoSoSanXuatGiongChiTietModel> { Errors = response.Errors };
                }

                return response.Data ?? new RequestHttpResponse<CoSoSanXuatGiongChiTietModel>();
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<CoSoSanXuatGiongChiTietModel>(ex);
            }
        }

        public async Task<RequestHttpResponse<List<CoSoSanXuatGiongChiTietModel>>> CreateAsync(List<CoSoSanXuatGiongChiTietModel> model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<List<CoSoSanXuatGiongChiTietModel>>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = model.Select(c => MapToCRUDModel(c)).ToList();
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<List<CoSoSanXuatGiongChiTietModel>>>($"items/{_collection}?fields={Fields}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<List<CoSoSanXuatGiongChiTietModel>> { Errors = response.Errors };
                }

                return response.Data ?? new RequestHttpResponse<List<CoSoSanXuatGiongChiTietModel>>();
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<CoSoSanXuatGiongChiTietModel>>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(CoSoSanXuatGiongChiTietModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<CoSoSanXuatGiongChiTietCRUDModel>>($"items/{_collection}/{model.id}", updateModel);

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
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(List<CoSoSanXuatGiongChiTietModel> model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<List<CoSoSanXuatGiongChiTietModel>>>($"items/{_collection}?fields={Fields}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(CoSoSanXuatGiongChiTietModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<CoSoSanXuatGiongChiTietCRUDModel>>($"items/{_collection}/{model.id}", new { deleted = true });

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(List<CoSoSanXuatGiongChiTietModel> model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<List<CoSoSanXuatGiongChiTietModel>>>($"items/{_collection}?fields={Fields}", model.Select(c => new { id = c.id, deleted = true }));

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
