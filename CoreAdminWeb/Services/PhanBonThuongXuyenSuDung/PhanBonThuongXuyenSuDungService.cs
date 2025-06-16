using CoreAdminWeb.Model.RequestHttps;
using CoreAdminWeb.RequestHttp;
using CoreAdminWeb.Services.BaseServices;
using System.Net;
using CoreAdminWeb.Model;

namespace CoreAdminWeb.Services.PhanBonThuongXuyenSuDungs
{
    public class PhanBonThuongXuyenSuDungService : IBaseService<PhanBonThuongXuyenSuDungModel>
    {
        private readonly string _collection = "PhanBonThuongXuyenSuDung";
        private const string Fields = "*,user_created.last_name,user_created.first_name,user_updated.last_name,user_updated.first_name"
            + ",province.id,province.name"
            + ",ward.id,ward.name"
            + ",phan_bon.id,phan_bon.name";

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
        private static PhanBonThuongXuyenSuDungCRUDModel MapToCRUDModel(PhanBonThuongXuyenSuDungModel model)
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
                phan_bon = model.phan_bon?.id,
                province = model.province?.id,
                ward = model.ward?.id
                
            };
        }

        /// <summary>
        /// Gets all fertilizer production facilities
        /// </summary>
        public async Task<RequestHttpResponse<List<PhanBonThuongXuyenSuDungModel>>> GetAllAsync(string query)
        {
            try
            {
                string url = $"items/{_collection}?fields={Fields}&{query}";
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<List<PhanBonThuongXuyenSuDungModel>>>(url);

                return response.IsSuccess
                    ? new RequestHttpResponse<List<PhanBonThuongXuyenSuDungModel>> { Data = response.Data?.Data }
                    : new RequestHttpResponse<List<PhanBonThuongXuyenSuDungModel>> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<List<PhanBonThuongXuyenSuDungModel>>(ex);
            }
        }

        /// <summary>
        /// Gets a fertilizer production facility by ID
        /// </summary>
        public async Task<RequestHttpResponse<PhanBonThuongXuyenSuDungModel>> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new RequestHttpResponse<PhanBonThuongXuyenSuDungModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "ID không được để trống" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var response = await RequestClient.GetAPIAsync<RequestHttpResponse<PhanBonThuongXuyenSuDungModel>>($"items/{_collection}/{id}?fields={Fields}");

                return response.IsSuccess
                    ? new RequestHttpResponse<PhanBonThuongXuyenSuDungModel> { Data = response.Data?.Data }
                    : new RequestHttpResponse<PhanBonThuongXuyenSuDungModel> { Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<PhanBonThuongXuyenSuDungModel>(ex);
            }
        }

        /// <summary>
        /// Creates a new fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<PhanBonThuongXuyenSuDungModel>> CreateAsync(PhanBonThuongXuyenSuDungModel model)
        {
            if (model == null)
            {
                return new RequestHttpResponse<PhanBonThuongXuyenSuDungModel>
                {
                    Errors = new List<ErrorResponse> { new() { Message = "Vui lòng nhập đầy đủ thông tin" } },
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var createModel = MapToCRUDModel(model);
                var response = await RequestClient.PostAPIAsync<RequestHttpResponse<PhanBonThuongXuyenSuDungCRUDModel>>($"items/{_collection}", createModel);

                if (!response.IsSuccess)
                {
                    return new RequestHttpResponse<PhanBonThuongXuyenSuDungModel> { Errors = response.Errors };
                }

                return new RequestHttpResponse<PhanBonThuongXuyenSuDungModel>
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
                return CreateErrorResponse<PhanBonThuongXuyenSuDungModel>(ex);
            }
        }

        /// <summary>
        /// Updates an existing fertilizer production facility
        /// </summary>
        public async Task<RequestHttpResponse<bool>> UpdateAsync(PhanBonThuongXuyenSuDungModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<PhanBonThuongXuyenSuDungCRUDModel>>($"items/{_collection}/{model.id}", updateModel);

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
        public async Task<RequestHttpResponse<bool>> DeleteAsync(PhanBonThuongXuyenSuDungModel model)
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
                var response = await RequestClient.PatchAPIAsync<RequestHttpResponse<PhanBonThuongXuyenSuDungCRUDModel>>($"items/{_collection}/{model.id}", new { deleted = true });

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
