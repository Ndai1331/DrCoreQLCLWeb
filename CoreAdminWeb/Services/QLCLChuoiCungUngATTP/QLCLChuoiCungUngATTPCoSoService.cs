using CoreAdminWeb.Model;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.Services.Http;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text.Json;
namespace CoreAdminWeb.Services
{

    public interface IQLCLChuoiCungUngATTPCoSoService
    {
        Task<RequestHttpResponse<List<QLCLChuoiCungUngATTPCoSoModel>>> GetAllAsync(string query);
        Task<RequestHttpResponse<QLCLChuoiCungUngATTPCoSoModel>> GetByIdAsync(string id);
        Task<RequestHttpResponse<List<QLCLChuoiCungUngATTPCoSoModel>>> CreateAsync(List<QLCLChuoiCungUngATTPCoSoModel> model);
        Task<RequestHttpResponse<bool>> UpdateAsync(List<QLCLChuoiCungUngATTPCoSoModel> model);
        Task<RequestHttpResponse<bool>> DeleteAsync(List<QLCLChuoiCungUngATTPCoSoModel> model);
    }

    public class QLCLChuoiCungUngATTPCoSoService(IHttpClientService _httpClientService) : IQLCLChuoiCungUngATTPCoSoService
    {
        private readonly string _collection = "QLCLChuoiCungUngATTPCoSo";
        private const string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name"
            + ",chuoi_cung_ung.id,chuoi_cung_ung.name,chuoi_cung_ung.code"
            + ",co_so_che_bien_nlts.id,co_so_che_bien_nlts.name,co_so_che_bien_nlts.code"
            + ",co_so_nlts_du_dieu_kien_attp.id,co_so_nlts_du_dieu_kien_attp.name,co_so_nlts_du_dieu_kien_attp.code";

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
        private static QLCLChuoiCungUngATTPCoSoCRUDModel MapToCRUDModel(QLCLChuoiCungUngATTPCoSoModel model)
        {
            return new()
            {
                chuoi_cung_ung = model.chuoi_cung_ung?.id,
                loai_co_so = model.loai_co_so,
                co_so_che_bien_nlts = model.co_so_che_bien_nlts?.id,
                co_so_nlts_du_dieu_kien_attp = model.co_so_nlts_du_dieu_kien_attp?.id,
                deleted = false
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<QLCLChuoiCungUngATTPCoSoModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await _httpClientService.GetAPIAsync<RequestHttpResponse<List<QLCLChuoiCungUngATTPCoSoModel>>>(url);

                return response.IsSuccess
                    ? new RequestHttpResponse<List<QLCLChuoiCungUngATTPCoSoModel>> { Data = response.Data?.Data, Meta = response.Data?.Meta }
                    : new RequestHttpResponse<List<QLCLChuoiCungUngATTPCoSoModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<QLCLChuoiCungUngATTPCoSoModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<QLCLChuoiCungUngATTPCoSoModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<QLCLChuoiCungUngATTPCoSoModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await _httpClientService.GetAPIAsync<RequestHttpResponse<QLCLChuoiCungUngATTPCoSoModel>>($"items/{_collection}/{id}?fields={Fields}");

                return response.IsSuccess
                    ? new RequestHttpResponse<QLCLChuoiCungUngATTPCoSoModel> { Data = response.Data?.Data, Meta = response.Data?.Meta }
                    : new RequestHttpResponse<QLCLChuoiCungUngATTPCoSoModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<QLCLChuoiCungUngATTPCoSoModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<QLCLChuoiCungUngATTPCoSoModel>> CreateAsync(QLCLChuoiCungUngATTPCoSoModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<QLCLChuoiCungUngATTPCoSoModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = MapToCRUDModel(model);
                var response = await _httpClientService.PostAPIAsync<RequestHttpResponse<QLCLCoSoCheBienNLTSCRUDModel>>($"items/{_collection}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<QLCLChuoiCungUngATTPCoSoModel> { Errors = response.Errors };
                }

                return new RequestHttpResponse<QLCLChuoiCungUngATTPCoSoModel>
                {
                    Data = new()
                    {
                    }
                };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<QLCLChuoiCungUngATTPCoSoModel>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(QLCLChuoiCungUngATTPCoSoModel model)
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
                var response = await _httpClientService.PatchAPIAsync<RequestHttpResponse<QLCLCoSoCheBienNLTSCRUDModel>>($"items/{_collection}/{model.id}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(QLCLChuoiCungUngATTPCoSoModel model)
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
                var response = await _httpClientService.PatchAPIAsync<RequestHttpResponse<QLCLCoSoCheBienNLTSCRUDModel>>($"items/{_collection}/{model.id}", new { deleted = true });

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



        public async Task<RequestHttpResponse<List<QLCLChuoiCungUngATTPCoSoModel>>> CreateAsync(List<QLCLChuoiCungUngATTPCoSoModel> model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<List<QLCLChuoiCungUngATTPCoSoModel>>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = model.Select(c => MapToCRUDModel(c)).ToList();
                var response = await _httpClientService.PostAPIAsync<RequestHttpResponse<List<QLCLChuoiCungUngATTPCoSoModel>>>($"items/{_collection}?fields={Fields}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<List<QLCLChuoiCungUngATTPCoSoModel>> { Errors = response.Errors };
                }

                return response.Data ?? new RequestHttpResponse<List<QLCLChuoiCungUngATTPCoSoModel>>();
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<QLCLChuoiCungUngATTPCoSoModel>>(ex);
            }
        }


        public async Task<RequestHttpResponse<bool>> UpdateAsync(List<QLCLChuoiCungUngATTPCoSoModel> model)
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
                var response = await _httpClientService.PatchAPIAsync<RequestHttpResponse<List<QLCLChuoiCungUngATTPCoSoModel>>>($"items/{_collection}?fields={Fields}", updateModel);

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

        public async Task<RequestHttpResponse<bool>> DeleteAsync(List<QLCLChuoiCungUngATTPCoSoModel> model)
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
                var response = await _httpClientService.PatchAPIAsync<RequestHttpResponse<List<QLCLChuoiCungUngATTPCoSoModel>>>($"items/{_collection}?fields={Fields}", model.Select(c => new { id = c.id, deleted = true }));

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
