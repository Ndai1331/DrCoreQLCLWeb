using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.RequestHttp;
using CoreAdminWeb.Services.BaseServices;
using System.Net;
using CoreAdminWeb.Model;

namespace CoreAdminWeb.Services.ThuocBVTVThuongXuyenSuDungs
{
    public class ThuocBVTVThuongXuyenSuDungService : IBaseService<ThuocBVTVThuongXuyenSuDungModel>
    {
        private readonly string _collection = "ThuocBVTVThuongXuyenSuDung";
        private const string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name"
            + ",province.id,province.name"
            + ",ward.id,ward.name"
            + ",thuoc_bvtv.id,thuoc_bvtv.name";

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
        private static ThuocBVTVThuongXuyenSuDungCRUDModel MapToCRUDModel(ThuocBVTVThuongXuyenSuDungModel model)
        {
            return new()
            {
                code = model.code,
                name = model.name,
                description = model.description,
                status = model.status.ToString(),
                sort = model.sort,
                khu_vuc_su_dung = model.khu_vuc_su_dung,
                lieu_luong_trung_binh = model.lieu_luong_trung_binh,
                thoi_diem_bon_phan = model.thoi_diem_bon_phan,
                hinh_thuc_bon_phan = model.hinh_thuc_bon_phan,
                loai_cay_trong_ap_dung = model.loai_cay_trong_ap_dung,
                thuoc_bvtv = model.thuoc_bvtv?.id,
                province = model.province?.id,
                ward = model.ward?.id
                
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<ThuocBVTVThuongXuyenSuDungModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<ThuocBVTVThuongXuyenSuDungModel>>>(url);

                return response.IsSuccess
                    ? new RequestHttpResponse<List<ThuocBVTVThuongXuyenSuDungModel>> { Data = response.Data?.Data }
                    : new RequestHttpResponse<List<ThuocBVTVThuongXuyenSuDungModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<ThuocBVTVThuongXuyenSuDungModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<ThuocBVTVThuongXuyenSuDungModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<ThuocBVTVThuongXuyenSuDungModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<ThuocBVTVThuongXuyenSuDungModel>>($"items/{_collection}/{id}?fields={Fields}");

                return response.IsSuccess
                    ? new RequestHttpResponse<ThuocBVTVThuongXuyenSuDungModel> { Data = response.Data?.Data }
                    : new RequestHttpResponse<ThuocBVTVThuongXuyenSuDungModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<ThuocBVTVThuongXuyenSuDungModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<ThuocBVTVThuongXuyenSuDungModel>> CreateAsync(ThuocBVTVThuongXuyenSuDungModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<ThuocBVTVThuongXuyenSuDungModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = MapToCRUDModel(model);
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<ThuocBVTVThuongXuyenSuDungCRUDModel>>($"items/{_collection}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<ThuocBVTVThuongXuyenSuDungModel> { Errors = response.Errors };
                }

                return new RequestHttpResponse<ThuocBVTVThuongXuyenSuDungModel>
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
                return CreateErrorResponse<ThuocBVTVThuongXuyenSuDungModel>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(ThuocBVTVThuongXuyenSuDungModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<ThuocBVTVThuongXuyenSuDungCRUDModel>>($"items/{_collection}/{model.id}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(ThuocBVTVThuongXuyenSuDungModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<ThuocBVTVThuongXuyenSuDungCRUDModel>>($"items/{_collection}/{model.id}", new { deleted = true });

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
