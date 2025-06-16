using CoreAdminWeb.Model;
using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.RequestHttp;
using CoreAdminWeb.Services.BaseServices;
using System.Net;

namespace CoreAdminWeb.Services
{
    public class CoSoViPhamTrongLinhVucCheBienService : IBaseService<CoSoViPhamTrongLinhVucCheBienModel>
    {
        private readonly string _collection = "ViPhamTrongLinhVuc";
        private const string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name"
            + ",co_so_vi_pham.id,co_so_vi_pham.code,co_so_vi_pham.name";

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
        private static CoSoViPhamTrongLinhVucCheBienCRUDModel MapToCRUDModel(CoSoViPhamTrongLinhVucCheBienModel model)
        {
            return new()
            {
                code = model.code,
                name = model.name,
                description = model.description,
                sort = model.sort,
                status = model.status,
                ngay_phat_hien = model.ngay_phat_hien,
                ngay_xu_ly = model.ngay_xu_ly,
                don_vi_kiem_tra_xu_ly = model.don_vi_kiem_tra_xu_ly,
                dia_diem_vi_pham = model.dia_diem_vi_pham,
                co_so_vi_pham = model.co_so_vi_pham?.id,
                doi_tuong_vi_pham = model.doi_tuong_vi_pham,
                noi_dung_vi_pham = model.noi_dung_vi_pham,
                hinh_thuc_xu_ly = model.hinh_thuc_xu_ly,
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<CoSoViPhamTrongLinhVucCheBienModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<CoSoViPhamTrongLinhVucCheBienModel>>>(url);

                return response.IsSuccess
                    ? new RequestHttpResponse<List<CoSoViPhamTrongLinhVucCheBienModel>> { Data = response.Data?.Data }
                    : new RequestHttpResponse<List<CoSoViPhamTrongLinhVucCheBienModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<CoSoViPhamTrongLinhVucCheBienModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<CoSoViPhamTrongLinhVucCheBienModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<CoSoViPhamTrongLinhVucCheBienModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<CoSoViPhamTrongLinhVucCheBienModel>>($"items/{_collection}/{id}?fields={Fields}");

                return response.IsSuccess
                    ? new RequestHttpResponse<CoSoViPhamTrongLinhVucCheBienModel> { Data = response.Data?.Data }
                    : new RequestHttpResponse<CoSoViPhamTrongLinhVucCheBienModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<CoSoViPhamTrongLinhVucCheBienModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<CoSoViPhamTrongLinhVucCheBienModel>> CreateAsync(CoSoViPhamTrongLinhVucCheBienModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<CoSoViPhamTrongLinhVucCheBienModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = MapToCRUDModel(model);
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<CoSoViPhamTrongLinhVucCheBienCRUDModel>>($"items/{_collection}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<CoSoViPhamTrongLinhVucCheBienModel> { Errors = response.Errors };
                }

                return new RequestHttpResponse<CoSoViPhamTrongLinhVucCheBienModel>
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
                return CreateErrorResponse<CoSoViPhamTrongLinhVucCheBienModel>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(CoSoViPhamTrongLinhVucCheBienModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<CoSoViPhamTrongLinhVucCheBienCRUDModel>>($"items/{_collection}/{model.id}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(CoSoViPhamTrongLinhVucCheBienModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<CoSoViPhamTrongLinhVucCheBienCRUDModel>>($"items/{_collection}/{model.id}", new { deleted = true });

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
