using CoreAdminWeb.Model;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.RequestHttp;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text.Json;
namespace CoreAdminWeb.Services
{

    public interface IQLCLKiemTraHauKiemATTPChiTietService
    {
        Task<RequestHttpResponse<List<QLCLKiemTraHauKiemATTPChiTietModel>>> GetAllAsync(string query);
        Task<RequestHttpResponse<QLCLKiemTraHauKiemATTPChiTietModel>> GetByIdAsync(string id);
        Task<RequestHttpResponse<List<QLCLKiemTraHauKiemATTPChiTietModel>>> CreateAsync(List<QLCLKiemTraHauKiemATTPChiTietModel> model);
        Task<RequestHttpResponse<bool>> UpdateAsync(List<QLCLKiemTraHauKiemATTPChiTietModel> model);
        Task<RequestHttpResponse<bool>> DeleteAsync(List<QLCLKiemTraHauKiemATTPChiTietModel> model);
    }

    public class QLCLKiemTraHauKiemATTPChiTietService : IQLCLKiemTraHauKiemATTPChiTietService
    {
        private readonly string _collection = "QLCLKiemTraHauKiemATTPChiTiet";
        private const string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name"
            + ",kiem_tra_hau_kiem_attp.id,"
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
        private static QLCLKiemTraHauKiemATTPChiTietCRUDModel MapToCRUDModel(QLCLKiemTraHauKiemATTPChiTietModel model)
        {
            return new()
            {
                kiem_tra_hau_kiem_attp = model.kiem_tra_hau_kiem_attp?.id,
                san_pham = model.san_pham?.id,
                so_luong_mau = model.so_luong_mau,
                loai_xet_nghiem = model.loai_xet_nghiem,
                mau_goc = model.mau_goc,
                chi_tieu = model.chi_tieu,
                so_mau_khong_dat = model.so_mau_khong_dat,
                chi_tieu_vi_pham = model.chi_tieu_vi_pham,
                muc_phat_hien = model.muc_phat_hien,
                deleted = false
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<QLCLKiemTraHauKiemATTPChiTietModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<QLCLKiemTraHauKiemATTPChiTietModel>>>(url);

                return response.IsSuccess
                    ? new RequestHttpResponse<List<QLCLKiemTraHauKiemATTPChiTietModel>> { Data = response.Data?.Data }
                    : new RequestHttpResponse<List<QLCLKiemTraHauKiemATTPChiTietModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<QLCLKiemTraHauKiemATTPChiTietModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<QLCLKiemTraHauKiemATTPChiTietModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<QLCLKiemTraHauKiemATTPChiTietModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<QLCLKiemTraHauKiemATTPChiTietModel>>($"items/{_collection}/{id}?fields={Fields}");

                return response.IsSuccess
                    ? new RequestHttpResponse<QLCLKiemTraHauKiemATTPChiTietModel> { Data = response.Data?.Data }
                    : new RequestHttpResponse<QLCLKiemTraHauKiemATTPChiTietModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<QLCLKiemTraHauKiemATTPChiTietModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<QLCLKiemTraHauKiemATTPChiTietModel>> CreateAsync(QLCLKiemTraHauKiemATTPChiTietModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<QLCLKiemTraHauKiemATTPChiTietModel>
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
                    return new RequestHttpResponse<QLCLKiemTraHauKiemATTPChiTietModel> { Errors = response.Errors };
                }

                return new RequestHttpResponse<QLCLKiemTraHauKiemATTPChiTietModel>
                {
                    Data = new()
                    {
                    }
                };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<QLCLKiemTraHauKiemATTPChiTietModel>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(QLCLKiemTraHauKiemATTPChiTietModel model)
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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(QLCLKiemTraHauKiemATTPChiTietModel model)
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



        public async Task<RequestHttpResponse<List<QLCLKiemTraHauKiemATTPChiTietModel>>> CreateAsync(List<QLCLKiemTraHauKiemATTPChiTietModel> model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<List<QLCLKiemTraHauKiemATTPChiTietModel>>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = model.Select(c => MapToCRUDModel(c)).ToList();
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<List<QLCLKiemTraHauKiemATTPChiTietModel>>>($"items/{_collection}?fields={Fields}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<List<QLCLKiemTraHauKiemATTPChiTietModel>> { Errors = response.Errors };
                }

                return response.Data ?? new RequestHttpResponse<List<QLCLKiemTraHauKiemATTPChiTietModel>>();
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<QLCLKiemTraHauKiemATTPChiTietModel>>(ex);
            }
        }


        public async Task<RequestHttpResponse<bool>> UpdateAsync(List<QLCLKiemTraHauKiemATTPChiTietModel> model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<List<QLCLKiemTraHauKiemATTPChiTietModel>>>($"items/{_collection}?fields={Fields}", updateModel);

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

        public async Task<RequestHttpResponse<bool>> DeleteAsync(List<QLCLKiemTraHauKiemATTPChiTietModel> model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<List<QLCLKiemTraHauKiemATTPChiTietModel>>>($"items/{_collection}?fields={Fields}", model.Select(c => new { id = c.id, deleted = true }));

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
