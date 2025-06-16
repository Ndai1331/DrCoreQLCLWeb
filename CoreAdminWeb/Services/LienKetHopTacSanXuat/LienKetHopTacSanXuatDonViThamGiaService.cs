using CoreAdminWeb.Enums;
using CoreAdminWeb.Model.LienKetHopTacSanXuat;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.RequestHttp;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text.Json;

namespace CoreAdminWeb.Services.LienKetHopTacSanXuat
{
    public class LienKetHopTacSanXuatDonViThamGiaService : ILienKetHopTacSanXuatDonViThamGiaService
    {
        private readonly string _collection = "LienKetHopTacSanXuatDonViThamGia";
        private const string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name"
            + ",lien_ket_hop_tac_san_xuat.id"
            + ",co_so_trong_trot_san_xuat.id,co_so_trong_trot_san_xuat.name";

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
        private static LienKetHopTacSanXuatDonViThamGiaCRUDModel MapToCRUDModel(LienKetHopTacSanXuatDonViThamGiaModel model)
        {
            return new()
            {
                status = model.status.ToString(),
                description = model.description,
                sort = model.sort,
                lien_ket_hop_tac_san_xuat = model.lien_ket_hop_tac_san_xuat?.id,
                co_so_trong_trot_san_xuat = model.co_so_trong_trot_san_xuat?.id,
                vai_tro = model.vai_tro,
                dien_tich = model.dien_tich,
                san_luong = model.san_luong
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<LienKetHopTacSanXuatDonViThamGiaModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<LienKetHopTacSanXuatDonViThamGiaModel>>>(url);

                return response.IsSuccess
                    ? new RequestHttpResponse<List<LienKetHopTacSanXuatDonViThamGiaModel>> { Data = response.Data?.Data }
                    : new RequestHttpResponse<List<LienKetHopTacSanXuatDonViThamGiaModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<LienKetHopTacSanXuatDonViThamGiaModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<LienKetHopTacSanXuatDonViThamGiaModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<LienKetHopTacSanXuatDonViThamGiaModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<LienKetHopTacSanXuatDonViThamGiaModel>>($"items/{_collection}/{id}?fields={Fields}");

                return response.IsSuccess
                    ? new RequestHttpResponse<LienKetHopTacSanXuatDonViThamGiaModel> { Data = response.Data?.Data }
                    : new RequestHttpResponse<LienKetHopTacSanXuatDonViThamGiaModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<LienKetHopTacSanXuatDonViThamGiaModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<LienKetHopTacSanXuatDonViThamGiaModel>> CreateAsync(LienKetHopTacSanXuatDonViThamGiaModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<LienKetHopTacSanXuatDonViThamGiaModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = MapToCRUDModel(model);
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<LienKetHopTacSanXuatDonViThamGiaModel>>($"items/{_collection}?fields={Fields}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<LienKetHopTacSanXuatDonViThamGiaModel> { Errors = response.Errors };
                }

                return response.Data ?? new RequestHttpResponse<LienKetHopTacSanXuatDonViThamGiaModel>();
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<LienKetHopTacSanXuatDonViThamGiaModel>(ex);
            }
        }

        public async Task<RequestHttpResponse<List<LienKetHopTacSanXuatDonViThamGiaModel>>> CreateAsync(List<LienKetHopTacSanXuatDonViThamGiaModel> model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<List<LienKetHopTacSanXuatDonViThamGiaModel>>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = model.Select(c => MapToCRUDModel(c)).ToList();
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<List<LienKetHopTacSanXuatDonViThamGiaModel>>>($"items/{_collection}?fields={Fields}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<List<LienKetHopTacSanXuatDonViThamGiaModel>> { Errors = response.Errors };
                }

                return response.Data ?? new RequestHttpResponse<List<LienKetHopTacSanXuatDonViThamGiaModel>>();
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<LienKetHopTacSanXuatDonViThamGiaModel>>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(LienKetHopTacSanXuatDonViThamGiaModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<LienKetHopTacSanXuatDonViThamGiaCRUDModel>>($"items/{_collection}/{model.id}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> UpdateAsync(List<LienKetHopTacSanXuatDonViThamGiaModel> model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<List<LienKetHopTacSanXuatDonViThamGiaModel>>>($"items/{_collection}?fields={Fields}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(LienKetHopTacSanXuatDonViThamGiaModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<LienKetHopTacSanXuatDonViThamGiaCRUDModel>>($"items/{_collection}/{model.id}", new { deleted = true });

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(List<LienKetHopTacSanXuatDonViThamGiaModel> model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<List<LienKetHopTacSanXuatDonViThamGiaModel>>>($"items/{_collection}?fields={Fields}", model.Select(c => new { id = c.id, deleted = true }));

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
