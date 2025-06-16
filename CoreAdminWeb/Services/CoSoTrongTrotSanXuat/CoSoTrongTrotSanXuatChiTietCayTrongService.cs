using CoreAdminWeb.Enums;
using CoreAdminWeb.Model.CoSoTrongTrotSanXuat;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.RequestHttp;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text.Json;

namespace CoreAdminWeb.Services.CoSoTrongTrotSanXuat
{
    public class CoSoTrongTrotSanXuatChiTietCayTrongService : ICoSoTrongTrotSanXuatChiTietCayTrongService
    {
        private readonly string _collection = "CoSoTrongTrotSanXuatChiTietCayTrong";
        private const string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name"
            + ",co_so_trong_trot_san_xuat.id,co_so_trong_trot_san_xuat.code"
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
        private static CoSoTrongTrotSanXuatChiTietCayTrongCRUDModel MapToCRUDModel(CoSoTrongTrotSanXuatChiTietCayTrongModel model)
        {
            return new()
            {
                co_so_trong_trot_san_xuat = model.co_so_trong_trot_san_xuat?.id,
                cay_giong_cay_trong = model.cay_giong_cay_trong?.id,
                status = model.status.ToString(),
                dien_tich = model.dien_tich,
                description = model.description,
                sort = model.sort,
                nang_suat_binh_quan = model.nang_suat_binh_quan,
                san_luong_binh_quan = model.san_luong_binh_quan
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<CoSoTrongTrotSanXuatChiTietCayTrongModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<CoSoTrongTrotSanXuatChiTietCayTrongModel>>>(url);

                return response.IsSuccess
                    ? new RequestHttpResponse<List<CoSoTrongTrotSanXuatChiTietCayTrongModel>> { Data = response.Data?.Data }
                    : new RequestHttpResponse<List<CoSoTrongTrotSanXuatChiTietCayTrongModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<CoSoTrongTrotSanXuatChiTietCayTrongModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<CoSoTrongTrotSanXuatChiTietCayTrongModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<CoSoTrongTrotSanXuatChiTietCayTrongModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<CoSoTrongTrotSanXuatChiTietCayTrongModel>>($"items/{_collection}/{id}?fields={Fields}");

                return response.IsSuccess
                    ? new RequestHttpResponse<CoSoTrongTrotSanXuatChiTietCayTrongModel> { Data = response.Data?.Data }
                    : new RequestHttpResponse<CoSoTrongTrotSanXuatChiTietCayTrongModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<CoSoTrongTrotSanXuatChiTietCayTrongModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<CoSoTrongTrotSanXuatChiTietCayTrongModel>> CreateAsync(CoSoTrongTrotSanXuatChiTietCayTrongModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<CoSoTrongTrotSanXuatChiTietCayTrongModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = MapToCRUDModel(model);
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<CoSoTrongTrotSanXuatChiTietCayTrongModel>>($"items/{_collection}?fields={Fields}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<CoSoTrongTrotSanXuatChiTietCayTrongModel> { Errors = response.Errors };
                }

                return response.Data ?? new RequestHttpResponse<CoSoTrongTrotSanXuatChiTietCayTrongModel>();
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<CoSoTrongTrotSanXuatChiTietCayTrongModel>(ex);
            }
        }

        public async Task<RequestHttpResponse<List<CoSoTrongTrotSanXuatChiTietCayTrongModel>>> CreateAsync(List<CoSoTrongTrotSanXuatChiTietCayTrongModel> model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<List<CoSoTrongTrotSanXuatChiTietCayTrongModel>>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = model.Select(c => MapToCRUDModel(c)).ToList();
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<List<CoSoTrongTrotSanXuatChiTietCayTrongModel>>>($"items/{_collection}?fields={Fields}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<List<CoSoTrongTrotSanXuatChiTietCayTrongModel>> { Errors = response.Errors };
                }

                return response.Data ?? new RequestHttpResponse<List<CoSoTrongTrotSanXuatChiTietCayTrongModel>>();
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<CoSoTrongTrotSanXuatChiTietCayTrongModel>>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(CoSoTrongTrotSanXuatChiTietCayTrongModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<CoSoTrongTrotSanXuatChiTietCayTrongCRUDModel>>($"items/{_collection}/{model.id}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> UpdateAsync(List<CoSoTrongTrotSanXuatChiTietCayTrongModel> model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<List<CoSoTrongTrotSanXuatChiTietCayTrongModel>>>($"items/{_collection}?fields={Fields}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(CoSoTrongTrotSanXuatChiTietCayTrongModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<CoSoTrongTrotSanXuatChiTietCayTrongCRUDModel>>($"items/{_collection}/{model.id}", new { deleted = true });

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(List<CoSoTrongTrotSanXuatChiTietCayTrongModel> model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<List<CoSoTrongTrotSanXuatChiTietCayTrongModel>>>($"items/{_collection}?fields={Fields}", model.Select(c => new { id = c.id, deleted = true }));

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
